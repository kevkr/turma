using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using turma.Controls;
using turma.Models.TuringMachine;
using turma.Models.Simulation;
using turma.ViewModels;
using Brushes = Avalonia.Media.Brushes;
using Point = Avalonia.Point;
using Avalonia.Threading;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;

namespace turma.Views
{
    public partial class ToolboxDiagram : UserControl
    {
        private Canvas field;
        public readonly static int CIRCLE_DIAMETER = 50;
        public readonly static int CIRCLE_RADIUS = CIRCLE_DIAMETER / 2;

        private readonly static int ARROWHEAD_ANGLE = 20; //Arrow Head Angle in Degrees
        private readonly static int ARROWHEAD_LENGTH = 15; //Arrow Head Length

        private bool MessageBoxDeletePathExists;

        private MainWindowViewModel? Mwvm { get; set; }
        public List<CustomMovableBorder> CustomMovableBorders = new List<CustomMovableBorder>();
        public Dictionary<Path, Tuple<CustomMovableBorder, CustomMovableBorder>> arrowDict = new Dictionary<Path, Tuple<CustomMovableBorder, CustomMovableBorder>>();
        private HashSet<Tuple<State, State>> stateTupleSet = new HashSet<Tuple<State, State>>();

        private ToggleButton EndstateBtn;
        private ToggleButton NormalstateBtn;
        private ToggleButton TransitionSBtn;
        private ToggleButton DeleteBtn;
        public ToggleButton MoveBtn;

        private Polyline DrawLineForArrow = new Polyline();
        private Point MousePosition = new Point();
        private Point? StartPoint;
        private CustomMovableBorder? FirstStateClicked;
        private Path? LastPathClicked = null;


        public ToolboxDiagram()
        {
            InitializeComponent();
            field = this.Find<Canvas>("Field");
            loadStates();
            loadTransitions();
        }

        public ToolboxDiagram(MainWindowViewModel mainWindowViewModel)
        {
            Mwvm = mainWindowViewModel;
            Mwvm.TuringMachineDefinitionChanged += new MainWindowViewModel.TuringMachineDefinitionChangedEventHandler(HandleDefinitionChanged);
            Mwvm.SimulationContext.CurrentStateChanged += new SimulationContext.CurrentStateChangedHandler(HandleCurrentStateChanged);
            Mwvm.SimulationContext.CurrentTransitionChanged += new SimulationContext.CurrentTransitionChangedHandler(HandleCurrentTransitionChanged);
            Mwvm.PropertyChanged += Mwvm_PropertyChanged;
            InitializeComponent();
            field = this.Find<Canvas>("Field");
            EndstateBtn = this.Find<ToggleButton>("EndState");
            NormalstateBtn = this.Find<ToggleButton>("NormalState");
            TransitionSBtn = this.Find<ToggleButton>("Transition");
            DeleteBtn = this.Find<ToggleButton>("DeleteState");
            MoveBtn = this.Find<ToggleButton>("MoveState");
            MoveBtn.IsChecked = true;

            loadStates();
            loadTransitions();

            DrawLineForArrow.Points = new List<Point>();
            field.Children.Add(DrawLineForArrow);

        }

        private void Mwvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SimulationContext" && Mwvm != null)
            {
                Mwvm.SimulationContext.CurrentStateChanged += new SimulationContext.CurrentStateChangedHandler(HandleCurrentStateChanged);
                Mwvm.SimulationContext.CurrentTransitionChanged += new SimulationContext.CurrentTransitionChangedHandler(HandleCurrentTransitionChanged);
                foreach (CustomMovableBorder customMovableBorder in CustomMovableBorders)
                {
                    Dispatcher.UIThread.Post(() => { customMovableBorder.BorderThickness = new Thickness(2.5); customMovableBorder.BorderBrush = Brushes.Black; customMovableBorder.InvalidateVisual(); }, DispatcherPriority.Normal);
                }
                foreach (Path p in arrowDict.Keys)
                {
                    Dispatcher.UIThread.Post(() => { p.Stroke = Brushes.Black; p.StrokeThickness = 2.5; p.InvalidateVisual(); }, DispatcherPriority.Normal);

                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        //The state counters have been removed, because the user should name the states. After clicking on the field to create a new state
        //There should be a small textbox inside the circle where the user can type the name of state and confirm with enter. If the user doesn't 
        //type a new the state should get a random name. 

        private void loadStates()
        {
            int counter = 0;

            if (Mwvm == null) return;

            List<CustomMovableBorder> oldCustomMovableBorders = new List<CustomMovableBorder>();
            oldCustomMovableBorders.AddRange(CustomMovableBorders);

            //List of all states
            field.Children.RemoveAll(CustomMovableBorders);
            CustomMovableBorders.Clear();
            //Create CustomMovableBorder for every state in Mwvm.TuringMachine.StateSet.AllStates
            foreach (State state in Mwvm.TuringMachine.StateSet.AllStates)
            {
                CustomMovableBorder? existingCustomMovableBorder = oldCustomMovableBorders.Find(cmb => cmb.AssociatedState.Equals(state));
                if (existingCustomMovableBorder == null)
                {
                    //Create and configure CustomMovableBorder 
                    CustomMovableBorder NormalState = new CustomMovableBorder(state, this);

                    Label stateLabel = new Label();
                    stateLabel.Width = CIRCLE_DIAMETER;
                    stateLabel.Height = CIRCLE_DIAMETER;
                    stateLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    stateLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    stateLabel.Content = state.Name;
                    stateLabel.VerticalAlignment = VerticalAlignment.Center;

                    Canvas customMovableBorderContent = new Canvas();
                    customMovableBorderContent.Width = CIRCLE_DIAMETER;
                    customMovableBorderContent.Height = CIRCLE_DIAMETER;
                    if (Mwvm.TuringMachine.StateSet.StartingState != null && state.Equals(Mwvm.TuringMachine.StateSet.StartingState))
                    {
                        customMovableBorderContent.Children.Add(createStartingStateMarker());
                    }

                    if (Mwvm.TuringMachine.StateSet.AcceptingStates.Contains(state))
                    {
                        customMovableBorderContent.Children.Add(createEndStateMarker());
                    }
                    Canvas.SetTop(stateLabel, 0);
                    Canvas.SetLeft(stateLabel, 0);

                    customMovableBorderContent.Children.Add(stateLabel);

                    NormalState.Name = state.Name;
                    NormalState.Background = Brushes.LightBlue;
                    NormalState.BorderBrush = Brushes.Black;
                    NormalState.BorderThickness = new Thickness(2.5);
                    NormalState.CornerRadius = new CornerRadius(CIRCLE_RADIUS);
                    NormalState.Width = CIRCLE_DIAMETER;
                    NormalState.Height = CIRCLE_DIAMETER;

                    NormalState.PointerReleased += new EventHandler<PointerReleasedEventArgs>(InputElement_OnPointerReleased);
                    //NormalState.AddHandler(PointerReleasedEvent, InputElement_OnPointerReleased, handledEventsToo: true);

                    getStatePosition(ref NormalState, ref counter);

                    //Store the CustomMovableBorders in a list
                    CustomMovableBorders.Add(NormalState);


                    NormalState.Child = customMovableBorderContent;
                    field.Children.Add(NormalState);
                }
                else
                {
                    Label stateLabel = new Label();
                    stateLabel.Width = CIRCLE_DIAMETER;
                    stateLabel.Height = CIRCLE_DIAMETER;
                    stateLabel.VerticalContentAlignment = VerticalAlignment.Center;
                    stateLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                    stateLabel.Content = state.Name;
                    stateLabel.VerticalAlignment = VerticalAlignment.Center;

                    Canvas customMovableBorderContent = new Canvas();
                    customMovableBorderContent.Children.Add(stateLabel);
                    customMovableBorderContent.Width = CIRCLE_DIAMETER;
                    customMovableBorderContent.Height = CIRCLE_DIAMETER;
                    if (Mwvm.TuringMachine.StateSet.StartingState != null && state.Equals(Mwvm.TuringMachine.StateSet.StartingState))
                    {
                        customMovableBorderContent.Children.Add(createStartingStateMarker());
                    }

                    if (Mwvm.TuringMachine.StateSet.AcceptingStates.Contains(state))
                    {
                        customMovableBorderContent.Children.Add(createEndStateMarker());
                    }
                    existingCustomMovableBorder.Child = customMovableBorderContent;

                    counter++;
                    if (!CustomMovableBorders.Contains(existingCustomMovableBorder))
                    {
                        CustomMovableBorders.Add(existingCustomMovableBorder);
                    }
                    if (!field.Children.Contains(existingCustomMovableBorder))
                    {
                        field.Children.Add(existingCustomMovableBorder);
                    }
                }
            }
        }

        public void getStatePosition(ref CustomMovableBorder NormalState, ref int counter)
        {
            Point position;
            if (counter < 0) throw new ArgumentOutOfRangeException(nameof(counter));
            if (counter < 1)
            {
                position = new Point(40, 125);
                Canvas.SetLeft(NormalState, position.X); //x-coord.
                Canvas.SetTop(NormalState, position.Y);  //y-coord.
                counter++;
            }
            else
            {
                int posY = counter % 4;
                if (posY == 0) posY = 4;
                posY = posY * 60 - 20;

                int posX;
                if (counter % 4 == 0)
                {
                    posX = counter / 4;
                }
                else
                {
                    posX = (counter / 4) + 1;
                }

                posX *= 90;

                position = new Point(posX, posY);
                Canvas.SetLeft(NormalState, position.X); //x-coord.
                Canvas.SetTop(NormalState, position.Y);  //y-coord.
                counter++;
            }
        }

        public void loadTransitions()
        {
            if (Mwvm == null) return;

            stateTupleSet = new HashSet<Tuple<State, State>>();

            foreach (Transition t in Mwvm.TuringMachine.Transitions)
            {
                if (t.ConditionState != null && t.InstructionState != null)
                {
                    Tuple<State, State> tuple = new Tuple<State, State>(t.ConditionState, t.InstructionState);
                    stateTupleSet.Add(tuple);
                }
            }

            fillArrowDict();

        }

        public void fillArrowDict()
        {
            field.Children.RemoveAll(arrowDict.Keys);
            Tuple<CustomMovableBorder, CustomMovableBorder>? lastPathClickedTuple = null;
            if (LastPathClicked != null && arrowDict.ContainsKey(LastPathClicked))
            {
                lastPathClickedTuple = arrowDict[LastPathClicked]; 
            } 
            arrowDict.Clear();
            foreach (Tuple<State, State> tuple in stateTupleSet)
            {

                CustomMovableBorder? b1 = CustomMovableBorders.Find(x => x.AssociatedState.Equals(tuple.Item1));
                CustomMovableBorder? b2 = CustomMovableBorders.Find(x => x.AssociatedState.Equals(tuple.Item2));
                if (b1 != null && b2 != null)
                {
                    Tuple<CustomMovableBorder, CustomMovableBorder> cmbs = new Tuple<CustomMovableBorder, CustomMovableBorder>(b1, b2);
                    Path arrowLine = MakeArrowLine();
                    arrowLine.PointerReleased += new EventHandler<PointerReleasedEventArgs>(pathPressedHandler);
                    if (lastPathClickedTuple != null && lastPathClickedTuple.Item1.AssociatedState.Equals(b1.AssociatedState) && lastPathClickedTuple.Item2.AssociatedState.Equals(b2.AssociatedState))
                    {
                        arrowLine.Stroke = Brushes.Orange;
                        LastPathClicked = arrowLine;
                    }
                    arrowDict.Add(arrowLine, cmbs);
                }
            }
        }


        public static Path MakeArrowLine()
        {
            Path arrowLine = new Path();
            arrowLine.StrokeThickness = 2.5;
            arrowLine.Stroke = Brushes.Black;
            return arrowLine;
        }

        public void updatePaths()
        {
            field.Children.RemoveAll(arrowDict.Keys);
            foreach (Path p in arrowDict.Keys)
            {
                CustomMovableBorder? b1 = arrowDict[p].Item1;
                CustomMovableBorder? b2 = arrowDict[p].Item2;
                StreamGeometry sg = new StreamGeometry();
                StreamGeometryContext ctx = sg.Open();
                if (!b1.AssociatedState.Equals(b2.AssociatedState))
                {
                    Point start = getPointOnLineByDistance(b1.centerPos, b2.centerPos, 25, false);
                    Point end = getPointOnLineByDistance(b1.centerPos, b2.centerPos, 25, true);
                    ctx.BeginFigure(start, false);
                    Point control = new Point((start.X+end.X)/2,(start.Y+end.Y)/2);
                    if (start.X >= end.X)
                    {
                        control = getOffsetPointOnLine(start, end, control, 30);
                    }
                    else
                    {
                        control = getOffsetPointOnLine(start, end, control, -30);
                    }
                    ctx.QuadraticBezierTo(control, end);
                    ctx.EndFigure(false);
                    ctx.BeginFigure(end, false);

                    double deltaX = end.X - control.X;
                    double deltaY = end.Y - control.Y;
                    double theta = Math.Atan2(deltaY, deltaX);
                    double phi = (Math.PI / 180) * ARROWHEAD_ANGLE;
                    double x, y = 0;
                    double rho = theta + phi;
                    for (int i = 0; i < 2; i++)
                    {
                        x = end.X - ARROWHEAD_LENGTH * Math.Cos(rho);
                        y = end.Y - ARROWHEAD_LENGTH * Math.Sin(rho);
                        ctx.LineTo(new Point(x, y));
                        ctx.EndFigure(false);
                        ctx.BeginFigure(end, false);
                        rho = theta - phi;
                    }
                    ctx.LineTo(end);
                    ctx.EndFigure(false);
                    ctx.Dispose();
                }
                else
                {
                    Point start = new Point(b1.centerPos.X, b1.centerPos.Y - CIRCLE_RADIUS);
                    Point end = new Point(b1.centerPos.X + 3, b1.centerPos.Y - CIRCLE_RADIUS);

                    ctx.BeginFigure(start, false);
                    Point control1 = new Point(b1.centerPos.X - CIRCLE_RADIUS , b1.centerPos.Y - CIRCLE_RADIUS - CIRCLE_RADIUS * 2);
                    Point control2 = new Point(b1.centerPos.X + CIRCLE_RADIUS, b1.centerPos.Y - CIRCLE_RADIUS - CIRCLE_RADIUS * 2);
                    ctx.CubicBezierTo(control1,control2,end);
                    ctx.EndFigure(false);
                    ctx.BeginFigure(end, false);

                    double deltaX = end.X - control2.X;
                    double deltaY = end.Y - control2.Y;
                    double theta = Math.Atan2(deltaY, deltaX);
                    double phi = (Math.PI / 180) * ARROWHEAD_ANGLE;
                    double x, y = 0;
                    double rho = theta + phi;
                    for (int i = 0; i < 2; i++)
                    {
                        x = end.X - ARROWHEAD_LENGTH * Math.Cos(rho);
                        y = end.Y - ARROWHEAD_LENGTH * Math.Sin(rho);
                        ctx.LineTo(new Point(x, y));
                        ctx.EndFigure(false);
                        ctx.BeginFigure(end, false);
                        rho = theta - phi;
                    }
                    ctx.LineTo(end);
                    ctx.EndFigure(false);
                    ctx.Dispose();
                }
                p.Data = sg;
                field.Children.Add(p);
            }
        }

        private async void InputElement_OnPointerReleased(object? sender, PointerEventArgs e)
        {
            CustomMovableBorder? StateBorder = sender as CustomMovableBorder;
            CustomMovableBorder SecondPointClicked;
            if (StateBorder is null || Mwvm is null)
            {
                return;
            }

            if (DeleteBtn.IsChecked == true)
            {
                if (StateBorder != null)
                {
                    Task<MessageBox.MessageBoxResult> m;
                    await(m = MessageBox.Show(App.MainWindow, "Möchten Sie wirklich den ausgewählten Zustand löschen? Dabei werden auch alle Übergänge gelöscht, welche mit diesem Zustand verbunden sind.", "Löschen bestätigen", MessageBox.MessageBoxButtons.OkCancel));
                    if (m.Result == MessageBox.MessageBoxResult.Ok)
                    {
                        deleteStateHelper(StateBorder.AssociatedState);
                        field.Children.Remove(StateBorder);
                    }
                }
            }

            if (EndstateBtn.IsChecked == true)
            {
                if (StateBorder != null)
                {
                    Canvas content = (Canvas)StateBorder.Child;
                    if (!Mwvm.TuringMachine.StateSet.AcceptingStates.Contains(StateBorder.AssociatedState))
                    {
                        content.Children.Add(createEndStateMarker());
                        Mwvm.TuringMachine.StateSet.AcceptingStates.Add(StateBorder.AssociatedState);
                        Mwvm.syncFromModel();
                    }
                    else
                    {
                        List<Control> toRemove = new List<Control>();
                        foreach (Control c in content.Children)
                        {
                            if (c is Border)
                            {
                                toRemove.Add(c);
                            }
                        }
                        content.Children.RemoveAll(toRemove);
                        Mwvm.TuringMachine.StateSet.AcceptingStates.Remove(StateBorder.AssociatedState);
                        Mwvm.syncFromModel();
                    }
                }
            }

            if (TransitionSBtn.IsChecked == true)
            {
                if (StartPoint == null && StateBorder != null)
                {
                    StartPoint = StateBorder.centerPos;
                    FirstStateClicked = StateBorder;
                }
                else if (StartPoint != null && StateBorder != null && FirstStateClicked != null)
                {
                    SecondPointClicked = StateBorder;
                    Transitions_ViaClick(FirstStateClicked, SecondPointClicked);
                    StartPoint = null;
                }
            }
        }

        private async void pathPressedHandler(object? sender, RoutedEventArgs e)
        {
            if (sender != null && Mwvm != null)
            {
                Path p = (Path)sender;
                
                if(p != LastPathClicked){
                    p.Stroke= Brushes.Orange;
                    if (LastPathClicked!= null)
                    {
                        LastPathClicked.Stroke = Brushes.Black;
                    }
                }
                
                if (arrowDict.ContainsKey(p))
                {
                    Tuple<State, State> t = new Tuple<State, State>(arrowDict[p].Item1.AssociatedState, arrowDict[p].Item2.AssociatedState);
                    if (DeleteBtn.IsChecked == true && MessageBoxDeletePathExists == false)
                    {
                        MessageBoxDeletePathExists = true;
                        Task<MessageBox.MessageBoxResult> m;
                        await(m = MessageBox.Show(App.MainWindow, "Möchten Sie wirklich den ausgewählten Pfeil löschen? Dabei werden auch alle Übergänge gelöscht, welche mit dem Pfeil verbunden sind.", "Löschen bestätigen", MessageBox.MessageBoxButtons.OkCancel));
                        if (m.Result == MessageBox.MessageBoxResult.Ok)
                        {
                            deleteTransitionsHelper(t);
                            field.Children.Remove(p);
                            MessageBoxDeletePathExists = false;
                        }
                        else
                        {
                            MessageBoxDeletePathExists = false;
                        }
                    }

                    Mwvm.raiseTransitionClicked(t);
                }
                LastPathClicked = p;
            }

        }

        private void Transitions_ViaClick(CustomMovableBorder FirstStateClicked, CustomMovableBorder SecondPointClicked)
        {
            DrawLineForArrow.Points = new List<Point>();
            DrawLineForArrow.InvalidateVisual();
            Tuple<State,State> tuple = new Tuple<State, State>(FirstStateClicked.AssociatedState, SecondPointClicked.AssociatedState);

            if (!doTransitionsExistForTuple(tuple)) {

                Tuple<CustomMovableBorder, CustomMovableBorder> cmbs = new Tuple<CustomMovableBorder, CustomMovableBorder>(FirstStateClicked, SecondPointClicked);
                Path arrowLine = MakeArrowLine();
                arrowLine.PointerReleased += new EventHandler<PointerReleasedEventArgs>(pathPressedHandler);
                arrowDict.Add(arrowLine, cmbs);

                this.FirstStateClicked = null;

                if (Mwvm != null)
                {
                    Transition createdTransition = new Transition(FirstStateClicked.AssociatedState, SecondPointClicked.AssociatedState, new char[Mwvm.TuringMachine.TapeCount], new char[Mwvm.TuringMachine.TapeCount], new Transition.Direction[Mwvm.TuringMachine.TapeCount], "");
                    Mwvm.TuringMachine.Transitions.Add(createdTransition);
                    pathPressedHandler(arrowLine, new RoutedEventArgs());
                }
            }
        }

        /* Check for pre-existing transitions */
        private bool doTransitionsExistForTuple(Tuple<State,State> tuple)
        {
            bool result = false;
            if (Mwvm != null)
            {
                foreach (Transition transition in Mwvm.TuringMachine.Transitions)
                {
                    if (transition.ConditionState != null && transition.InstructionState != null && transition.ConditionState.Equals(tuple.Item1) && transition.InstructionState.Equals(tuple.Item2))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private void Toggle_Click(object? sender, RoutedEventArgs e)
        {
            if (EndstateBtn == sender)
            {
                if (NormalstateBtn.IsChecked == true)
                {
                    NormalstateBtn.IsChecked = false;
                }
                if (TransitionSBtn.IsChecked == true)
                {
                    TransitionSBtn.IsChecked = false;
                }
                if (DeleteBtn.IsChecked == true)
                {
                    DeleteBtn.IsChecked = false;
                }
                if (MoveBtn.IsChecked == true)
                {
                    MoveBtn.IsChecked = false;
                }

                EndstateBtn.IsChecked = true;
            }
            if (NormalstateBtn == sender)
            {
                if (EndstateBtn.IsChecked == true)
                {
                    EndstateBtn.IsChecked = false;
                }
                if (TransitionSBtn.IsChecked == true)
                {
                    TransitionSBtn.IsChecked = false;
                }
                if (DeleteBtn.IsChecked == true)
                {
                    DeleteBtn.IsChecked = false;
                }
                if (MoveBtn.IsChecked == true)
                {
                    MoveBtn.IsChecked = false;
                }

                NormalstateBtn.IsChecked = true;
            }
            if (TransitionSBtn == sender)
            {
                if (EndstateBtn.IsChecked == true)
                {
                    EndstateBtn.IsChecked = false;
                }
                if (NormalstateBtn.IsChecked == true)
                {
                    NormalstateBtn.IsChecked = false;
                }
                if (DeleteBtn.IsChecked == true)
                {
                    DeleteBtn.IsChecked = false;
                }
                if (MoveBtn.IsChecked == true)
                {
                    MoveBtn.IsChecked = false;
                }

                TransitionSBtn.IsChecked = true;

            }
            if (DeleteBtn == sender)
            {
                if (EndstateBtn.IsChecked == true)
                {
                    EndstateBtn.IsChecked = false;
                }
                if (TransitionSBtn.IsChecked == true)
                {
                    TransitionSBtn.IsChecked = false;
                }
                if (NormalstateBtn.IsChecked == true)
                {
                    NormalstateBtn.IsChecked = false;
                }
                if (MoveBtn.IsChecked == true)
                {
                    MoveBtn.IsChecked = false;
                }

                DeleteBtn.IsChecked = true;

            }

            if (MoveBtn == sender)
            {
                if (EndstateBtn.IsChecked == true)
                {
                    EndstateBtn.IsChecked = false;
                }
                if (TransitionSBtn.IsChecked == true)
                {
                    TransitionSBtn.IsChecked = false;
                }
                if (NormalstateBtn.IsChecked == true)
                {
                    NormalstateBtn.IsChecked = false;
                }
                if (DeleteBtn.IsChecked == true)
                {
                    DeleteBtn.IsChecked = false;
                }
                MoveBtn.IsChecked = true;
            }
        }

        private void Field_OnPointerMoved(object sender, PointerEventArgs e)
        {

            if (TransitionSBtn.IsChecked == true && FirstStateClicked != null)
            {   //So State wont move
                List<Point> arrowPoints = new List<Point>();
                MousePosition = e.GetPosition(field);
                Point AnchorPoint = getPointOnLineByDistance(FirstStateClicked.centerPos, MousePosition, CIRCLE_RADIUS, false);
                arrowPoints.Add(AnchorPoint);
                arrowPoints.Add(MousePosition);

                double deltaX = MousePosition.X - AnchorPoint.X;
                double deltaY = MousePosition.Y - AnchorPoint.Y;

                double theta = Math.Atan2(deltaY, deltaX);
                double phi = (Math.PI / 180) * ARROWHEAD_ANGLE;
                double x, y = 0;
                double rho = theta + phi;
                for (int i = 0; i < 2; i++)
                {
                    x = MousePosition.X - ARROWHEAD_LENGTH * Math.Cos(rho);
                    y = MousePosition.Y - ARROWHEAD_LENGTH * Math.Sin(rho);
                    arrowPoints.Add(new Point(x, y));
                    rho = theta - phi;
                }

                arrowPoints.Add(MousePosition);
                DrawLineForArrow.Points = arrowPoints;
                DrawLineForArrow.StrokeThickness = 2.5;
                DrawLineForArrow.Stroke = Brushes.Black;
                DrawLineForArrow.InvalidateVisual();
            }
        }

        private void Field_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (Mwvm is null)
                return;
            if (NormalstateBtn.IsChecked == true)
            {
                Point StatePosition;
                String generatedStateName = generateUnusedStateName();
                State state = new State(generatedStateName);

                CustomMovableBorder NormalState = new CustomMovableBorder(state, this);

                Label stateLabel = new Label();
                stateLabel.Width = CIRCLE_DIAMETER;
                stateLabel.Height = CIRCLE_DIAMETER;
                stateLabel.VerticalContentAlignment = VerticalAlignment.Center;
                stateLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                stateLabel.Content = state.Name;
                stateLabel.VerticalAlignment = VerticalAlignment.Center;

                Canvas customMovableBorderContent = new Canvas();
                customMovableBorderContent.Width = CIRCLE_DIAMETER;
                customMovableBorderContent.Height = CIRCLE_DIAMETER;
                Canvas.SetTop(stateLabel, 0);
                Canvas.SetLeft(stateLabel, 0);

                customMovableBorderContent.Children.Add(stateLabel);

                NormalState.Name = state.Name;
                NormalState.Background = Brushes.LightBlue;
                NormalState.BorderBrush = Brushes.Black;
                NormalState.BorderThickness = new Thickness(2.5);
                NormalState.CornerRadius = new CornerRadius(CIRCLE_RADIUS);
                NormalState.Width = CIRCLE_DIAMETER;
                NormalState.Height = CIRCLE_DIAMETER;

                NormalState.PointerReleased += new EventHandler<PointerReleasedEventArgs>(InputElement_OnPointerReleased);

                StatePosition = e.GetPosition(field);
                Canvas.SetLeft(NormalState, StatePosition.X - CIRCLE_RADIUS); //x-coord.
                Canvas.SetTop(NormalState, StatePosition.Y - CIRCLE_RADIUS); //y-coord.

                NormalState.Child = customMovableBorderContent;
                CustomMovableBorders.Add(NormalState);
                field.Children.Add(NormalState);
                Mwvm.TuringMachine.StateSet.AllStates.Add(state);
                Mwvm.syncFromModel();
            }

            if (TransitionSBtn.IsChecked == true && e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                FirstStateClicked = null;
                StartPoint = null;
                DrawLineForArrow.Points = new List<Point>();
                DrawLineForArrow.InvalidateVisual();
            }
            
            if (NormalstateBtn.IsChecked == false && e.GetCurrentPoint(this).Properties.IsRightButtonPressed)
            {
                if (LastPathClicked != null)
                LastPathClicked.Stroke= Brushes.Black;
                LastPathClicked = null;
                
                State State1 = new State("");
                State State2 = new State("");
                Tuple<State, State> t = new Tuple<State, State>(State1, State2);
                Mwvm.raiseTransitionClicked(t);
            }

        }

        private string generateUnusedStateName()
        {
            if (Mwvm is null)
            {
                return "";
            }
            Random rnd = new Random();
            string generatedName = "q" + Mwvm.TuringMachine.StateSet.AllStates.Count;
            while (Mwvm.TuringMachine.StateSet.AllStates.Contains(new State(generatedName)))
            {
                generatedName = "q" + rnd.Next(10000);
            }
            return generatedName;
        }


        private void SetTogglesToFalse(object? sender, PointerPressedEventArgs e)
        {
            EndstateBtn.IsChecked = false;
            NormalstateBtn.IsChecked = false;
            TransitionSBtn.IsChecked = false;
            DeleteBtn.IsChecked = false;
            MoveBtn.IsChecked = false;
        }

        public void HandleDefinitionChanged(object? sender, EventArgs e)
        {
            loadStates();
            loadTransitions();
            InvalidateVisual();
        }

        public void HandleCurrentTransitionChanged(Transition? transition)
        {
            if (transition != null)
            {
                foreach (Path p in arrowDict.Keys)
                {
                    if (arrowDict[p].Item1.AssociatedState.Equals(transition.ConditionState) && arrowDict[p].Item2.AssociatedState.Equals(transition.InstructionState))
                    {
                        Dispatcher.UIThread.Post(() => { p.Stroke = Brushes.DeepPink; p.StrokeThickness = 2.5; p.InvalidateVisual(); }, DispatcherPriority.Normal);
                    }
                    else
                    {
                        Dispatcher.UIThread.Post(() => { p.Stroke = Brushes.Black; p.StrokeThickness = 2.5; p.InvalidateVisual(); }, DispatcherPriority.Normal);
                    }
                }
            }
        }

        public void HandleCurrentStateChanged(State? state)
        {
            foreach (CustomMovableBorder customMovableBorder in CustomMovableBorders)
            {
                if (customMovableBorder.AssociatedState.Equals(state))
                {
                    Dispatcher.UIThread.Post(() => { customMovableBorder.BorderThickness = new Thickness(2.5); customMovableBorder.BorderBrush = Brushes.DeepPink; customMovableBorder.InvalidateVisual(); }, DispatcherPriority.Normal);
                }
                else
                {
                    Dispatcher.UIThread.Post(() => { customMovableBorder.BorderThickness = new Thickness(2.5); customMovableBorder.BorderBrush = Brushes.Black; customMovableBorder.InvalidateVisual(); }, DispatcherPriority.Normal);
                }
            }
        }

        private Point getOffsetPointOnLine(Point start, Point end, Point onLine, double offset)
        {
            double dX = Math.Abs(start.X - end.X);
            double dY = Math.Abs(start.Y - end.Y);
            double nX = -dY;
            double nY = dX;
            double nd = Math.Sqrt(Math.Pow(nX,2)+Math.Pow(nY,2)) ;
            nX /= nd;
            nY /= nd;
            double resultX = onLine.X + (offset * nX);
            double resultY = onLine.Y + (offset * nY);
            return new Point(resultX, resultY);
        }

        private Point getPointOnLineByDistance(Point start, Point end, double distance, bool reverse)
        {
            double lineDistance = getDistanceBetweenTwoPoints(start,end);
            if (reverse)
            {
                distance = lineDistance - distance;
            }
            double ratio = distance / lineDistance;
            Debug.WriteLineIf(ratio >= 1 || ratio <= 0, ratio);

            double resultX = (1 - ratio) * start.X + ratio * end.X;
            double resultY = (1 - ratio) * start.Y + ratio * end.Y;
            return new Point(resultX, resultY);
        }

        private double getDistanceBetweenTwoPoints(Point start, Point end)
        {
            return Math.Sqrt(Math.Pow(((end.X - start.X)), 2) + Math.Pow((start.Y - end.Y), 2));
        }

        public override void Render(DrawingContext context)
        {
            updatePaths();
            base.Render(context);

        }

        public void Canvas_KeyDown(object Sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (TransitionSBtn.IsChecked.HasValue && TransitionSBtn.IsChecked.Value)
                {
                    FirstStateClicked = null;
                    StartPoint = null;
                    DrawLineForArrow.Points = new List<Point>();
                    DrawLineForArrow.InvalidateVisual();
                }
            }
        }

        private Polyline createStartingStateMarker()
        {
            Polyline polyline = new Polyline();
            List<Point> points = new List<Point>();
            points.Add(new Point(-CIRCLE_RADIUS, CIRCLE_RADIUS));
            points.Add(new Point(0, CIRCLE_RADIUS));
            points.Add(new Point(0 - 5, CIRCLE_RADIUS - 3));
            points.Add(new Point(0 - 5, CIRCLE_RADIUS + 3));
            points.Add(new Point(0, CIRCLE_RADIUS));
            polyline.Points = points;
            polyline.StrokeThickness = 2.5;
            polyline.Stroke = Brushes.Black;
            polyline.Fill = Brushes.Black;

            return polyline;
        }

        private Border createEndStateMarker()
        {

            Border border = new Border();
            border.BorderThickness = new Thickness(2.5);
            border.BorderBrush = Brushes.Black;
            border.Width = CIRCLE_DIAMETER - 8;
            border.Height = CIRCLE_DIAMETER - 8;
            border.CornerRadius = new CornerRadius(100);
            Canvas.SetTop(border, 3.55);
            Canvas.SetLeft(border, 3.55);

            return border;
        }

        private void deleteStateHelper(State s)
        {
            if (Mwvm is null)
                return;
            StateSet stateSet = Mwvm.TuringMachine.StateSet;
            stateSet.AllStates.Remove(s);
            if (stateSet.StartingState != null && stateSet.StartingState.Equals(s))
            {
                stateSet.StartingState = null;
            }
            if (stateSet.AcceptingStates.Contains(s))
            {
                stateSet.AcceptingStates.Remove(s);
            }
            List<Transition> markedForDeletion = new List<Transition>();
            foreach (Transition t in Mwvm.TuringMachine.Transitions)
            {
                if (t.ConditionState != null && t.ConditionState.Equals(s))
                {
                    markedForDeletion.Add(t);
                }
                if (t.InstructionState != null && t.InstructionState.Equals(s))
                {
                    markedForDeletion.Add(t);
                }
            }
            Mwvm.TuringMachine.Transitions.RemoveAll(x => markedForDeletion.Contains(x));
            Mwvm.syncFromModel();

        }

        private void deleteTransitionsHelper(Tuple<State, State> tuple)
        {
            if (Mwvm is null)
                return;
            List<Transition> markedForDeletion = new List<Transition>();
            foreach (Transition t in Mwvm.TuringMachine.Transitions)
            {
                if (tuple.Item1.Equals(t.ConditionState) && tuple.Item2.Equals(t.InstructionState))
                {
                    markedForDeletion.Add(t);
                }
            }
            Mwvm.TuringMachine.Transitions.RemoveAll(x => markedForDeletion.Contains(x));
            Mwvm.syncFromModel();
        }
    }

}