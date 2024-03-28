using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using Avalonia.Media;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using turma.Models.Simulation;
using turma.Models.TuringMachine;
using turma.ViewModels;
using Avalonia.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using Avalonia.Threading;
using MsgBox;

namespace turma.Views
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel Mwvm { get; set; }

        private MenuItem saveFileBtn;
        private Menu menu;
        private Button startBtn;
        private Button stopBtn;
        private Button pauseBtn;
        private Button stepBackBtn;
        private Button stepForwardBtn;
        private Button slowSpeedBtn;
        private Button normalSpeedBtn;
        private Button fastSpeedBtn;
        private Button loadInputWordBtn;

        private Panel sidebarPanel;

        private StackPanel inputPanel;

        private bool inputWordLoaded;

        private string inputWord { get; set; }

        private TextBlock acceptedTB;

        private TableView tableView;
        private ToolboxDiagram diagrammToolboxView;

        private TextBox inputWordTextbox;

        public MainWindow()
        {
            Mwvm = new MainWindowViewModel();
            Mwvm.SimulationContext.CurrentSimulationStateChanged += new SimulationContext.CurrentSimulationStateChangedHandler(HandleSimulationStateChanged);
            Mwvm.PropertyChanged += Mwvm_PropertyChanged;
            DataContext = Mwvm;
            InitializeComponent();
            saveFileBtn = this.Find<MenuItem>("SaveFileBtn");
            menu = this.Find<Menu>("Main");

            inputPanel = this.Find<StackPanel>("InputPanel");

            startBtn = this.Find<Button>("StartButton");
            stopBtn = this.Find<Button>("StopButton");
            pauseBtn = this.Find<Button>("PauseButton");
            stepBackBtn = this.Find<Button>("StepBackButton");
            stepForwardBtn = this.Find<Button>("StepForwardButton");
            slowSpeedBtn = this.Find<Button>("SlowButton");
            normalSpeedBtn = this.Find<Button>("NormalButton");
            fastSpeedBtn = this.Find<Button>("FastButton");
            loadInputWordBtn = this.Find<Button>("LoadInputWordButton");
            inputWordTextbox = this.Find<TextBox>("InputWordTextBox");

            sidebarPanel = this.Find<Panel>("SidebarPanel");
            Mwvm.PropertyChanged += new PropertyChangedEventHandler(tapeCount_PropertyChanged);

            acceptedTB = this.Find<TextBlock>("IsStateAccepted");

            initTapes(Mwvm.TuringMachine.TapeCount);
            addTable(Mwvm);
            addSidebar(Mwvm);          
            addDiagramm(Mwvm);
        }

        public void addTable(MainWindowViewModel Mwvm)
        {
            tableView = new TableView(Mwvm);
            var table = this.Find<StackPanel>("TablePanel");

            table.Children.Add(tableView);
        }

        public void addSidebar(MainWindowViewModel Mwvm)
        {
            var sidebarView = new SidebarView(Mwvm);
            var table = this.Find<Panel>("SidebarPanel");
            table.Children.Add(sidebarView);
        }


        private void InitializeComponent(bool loadXaml = true, bool attachDevTools = true)
        {
            if (loadXaml)
            {
                AvaloniaXamlLoader.Load(this);
            }

            // This will be added only if you install Avalonia.Diagnostics.
#if DEBUG
            if (attachDevTools)
            {
                this.AttachDevTools();
            }
#endif
        }

        public void initTapes(int tapeCount)
        {
            var tapePanel = this.Find<StackPanel>("TapePanel");
            Debug.WriteLine(tapePanel.Height);
            if (tapePanel.Children != null)
            {
                tapePanel.Children.Clear();
                for (int i = 0; i < tapeCount; i++)
                {
                    var tapeView = new TapeView(Mwvm.SimulationContext, i, this.Width);
                    ClientSizeProperty.Changed.Subscribe(x => { 
                        if(this.Width - tapeView.Width >= 10) 
                        {
                            tapeView.InitializeComponent(this.Width);
                            tapeView.updateTape();
                        }
                    });
                    tapePanel.Children.Add(tapeView);
                }
                tapePanel.InvalidateMeasure();
            }
        }

        public async void TransformBtn_Click(object? sender, RoutedEventArgs args)
        {
            await Mwvm.transfromTMDialog();
        }

        public async void ClearBtn_Click(object? sender, RoutedEventArgs args)
        {
            await clear();
            
        }
        public async Task clear()
        {
            Task<MessageBox.MessageBoxResult> m;
            await (m = MessageBox.Show(App.MainWindow, "Definition der Turingmaschine löschen?", "Löschen bestätigen", MessageBox.MessageBoxButtons.OkCancel));
            if (m.Result == MessageBox.MessageBoxResult.Ok)
            {
                Mwvm = new MainWindowViewModel();
                Mwvm.SimulationContext.CurrentSimulationStateChanged += new SimulationContext.CurrentSimulationStateChangedHandler(HandleSimulationStateChanged);
                Mwvm.PropertyChanged += Mwvm_PropertyChanged;
                DataContext = Mwvm;
                Mwvm.PropertyChanged += new PropertyChangedEventHandler(tapeCount_PropertyChanged);

                var tableView = new TableView(Mwvm);
                var table = this.Find<StackPanel>("TablePanel");
                table.Children.Clear();
                table.Children.Add(tableView);

                var sidebarView = new SidebarView(Mwvm);
                var sidebar = this.Find<Panel>("SidebarPanel");
                sidebar.Children.Clear();
                sidebar.Children.Add(sidebarView);

                diagrammToolboxView = new ToolboxDiagram(Mwvm);
                var diagram = this.Find<Panel>("ToolboxDiagram");
                diagram.Children.Clear();
                diagram.Children.Add(diagrammToolboxView);

                initTapes(Mwvm.TuringMachine.TapeCount);
                inputWordTextbox.Text = "";
                inputWord = "";
            }
        }




        public async void OpenFileBtn_Click(object? sender, RoutedEventArgs args)
        {
            if (!Mwvm.SimulationContext.isSimulationActive())
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filters.Add(new FileDialogFilter() { Extensions = { "yml" } });
                string[]? dialogResult = await openFileDialog.ShowAsync(this);
                if (dialogResult != null && dialogResult.Length > 0)
                {
                    saveFileBtn.IsEnabled = true;
                    inputWord = "";
                    inputWordTextbox.Text = "";
                    Mwvm.LoadFile(dialogResult[0]);
                }
            }
            else
            {
                MessageBox.Show(App.MainWindow, "Es kann keine Turing Maschine eingelesen werden, während die Simulation läuft.", "Error", MessageBox.MessageBoxButtons.Ok);
            }
        }


        public async void SaveAsFileBtn_Click(object? sender, RoutedEventArgs args)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialFileName = "turing-machine.yml";
            saveFileDialog.Filters.Add(new FileDialogFilter() { Extensions = { "yml" } });
            string? dialogResult = await saveFileDialog.ShowAsync(this);
            if (dialogResult != null)
            {
                saveFileBtn.IsEnabled = true;
                Mwvm.Save(dialogResult);
            }
        }

        public void SaveFileBtn_Click(object? sender, RoutedEventArgs args)
        {
            Mwvm.Save();
        }

        public void CloseItem_Click(object? sender, RoutedEventArgs args)
        {
            Close();
        }

        public void ShowTable_Click(object? sender, RoutedEventArgs args)
        {
            Mwvm.ShowTable = true;
            TableView tv = (TableView)this.Find<StackPanel>("TablePanel").Children[0];
            tv.InitializeComponent();

        }

        public void Edit_Click(object? sender, RoutedEventArgs args)
        {
            if (Mwvm.isTuringMachineDefinitionValid())
            {
                Mwvm.IsTransformable = true;
            }
            else Mwvm.IsTransformable = false;
        }

        public void ShowDiagram_Click(object? sender, RoutedEventArgs args)
        {
            Mwvm.raiseDefChanged();
            Mwvm.ShowTable = false;
        }

        public void Start_Click(object sender, RoutedEventArgs e)
        {
            var start = (Button)sender;
            if (Mwvm.startSimulation())
            {
                start.Background = Brushes.LightGreen;
                start.BorderBrush = Brushes.Black;
                stopBtn.BorderBrush = Brushes.Transparent;
                pauseBtn.BorderBrush = Brushes.Transparent;
                pauseBtn.IsEnabled = true;
                stepForwardBtn.BorderBrush = Brushes.Transparent;
                stepBackBtn.BorderBrush = Brushes.Transparent;

                stepForwardBtn.IsEnabled = false;
                stepBackBtn.IsEnabled = false;

                sidebarPanel.IsEnabled = false;
            }
        }
        public void StepForward_Click(object sender, RoutedEventArgs e)
        {
            var stepForward = (Button)sender;
            stepForward.BorderBrush = Brushes.LightGreen;
            stopBtn.BorderBrush = Brushes.Transparent;
            pauseBtn.BorderBrush = Brushes.Transparent;
            startBtn.BorderBrush = Brushes.Transparent;
            stepBackBtn.BorderBrush = Brushes.Transparent;

            Mwvm.stepForward();

        }
        public void StepBack_Click(object sender, RoutedEventArgs e)
        {
            var stepBack = (Button)sender;
            stepBack.BorderBrush = Brushes.LightGreen;
            stopBtn.BorderBrush = Brushes.Transparent;
            pauseBtn.BorderBrush = Brushes.Transparent;
            startBtn.BorderBrush = Brushes.Transparent;
            stepForwardBtn.BorderBrush = Brushes.Transparent;

            Mwvm.stepBack();

        }
        public void Pause_Click(object sender, RoutedEventArgs e)
        {
            var pause = (Button)sender;
            pause.BorderBrush = Brushes.LightGreen;
            stopBtn.BorderBrush = Brushes.Transparent;
            startBtn.BorderBrush = Brushes.Transparent;
            stepForwardBtn.BorderBrush = Brushes.Transparent;
            stepBackBtn.BorderBrush = Brushes.Transparent;

            stepForwardBtn.IsEnabled = true;
            stepBackBtn.IsEnabled = true;

            Mwvm.pauseSimulation();
        }
        public void Stop_Click(object? sender, RoutedEventArgs? e)
        {
            normalSpeedBtn.BorderBrush = Brushes.Yellow;
            slowSpeedBtn.BorderBrush = Brushes.Black;
            fastSpeedBtn.BorderBrush = Brushes.Black;

            if (sender != null)
            {
                var stop = (Button)sender;
                stop.BorderBrush = Brushes.LightGreen;
            }
            pauseBtn.BorderBrush = Brushes.Transparent;
            startBtn.BorderBrush = Brushes.Transparent;
            stepForwardBtn.BorderBrush = Brushes.Transparent;
            stepBackBtn.BorderBrush = Brushes.Transparent;
            

            startBtn.IsEnabled = false;
            stopBtn.IsEnabled = false;
            pauseBtn.IsEnabled = false;
            stepBackBtn.IsEnabled = false;
            stepForwardBtn.IsEnabled = false;

            sidebarPanel.IsEnabled = true;

            inputWordLoaded = false;
            InputWordTextBoxChanged(null, null);

            tableView.IsEnabled = true;
            tableView.loadRows = false;
            tableView.InitializeTable(null, null);

            diagrammToolboxView.IsEnabled = true;

            menu.IsEnabled = true;
            inputPanel.IsEnabled = true;

            Dispatcher.UIThread.InvokeAsync(() => {
                acceptedTB.Text = "";
            });

            Mwvm.stopSimulation();

            Mwvm.SimulationContext = new SimulationContext(Mwvm.TuringMachine);
            initTapes(Mwvm.TuringMachine.TapeCount);
        }
        public void SlowSpeed_Click(object sender, RoutedEventArgs e)
        {
            var slow = (Button)sender;
            slow.BorderBrush = Brushes.Yellow;
            normalSpeedBtn.BorderBrush = Brushes.Black;
            fastSpeedBtn.BorderBrush = Brushes.Black;

            Mwvm.setAnimationSpeed(2.0);

        }
        public void NormalSpeed_Click(object sender, RoutedEventArgs e)
        {
            var normal = (Button)sender;
            normal.BorderBrush = Brushes.Yellow;
            slowSpeedBtn.BorderBrush = Brushes.Black;
            fastSpeedBtn.BorderBrush = Brushes.Black;

            Mwvm.setAnimationSpeed(1.0);

        }
        public void FastSpeed_Click(object sender, RoutedEventArgs e)
        {
            var fast = (Button)sender;
            fast.BorderBrush = Brushes.Yellow;
            slowSpeedBtn.BorderBrush = Brushes.Black;
            normalSpeedBtn.BorderBrush = Brushes.Black;

            Mwvm.setAnimationSpeed(0.5);
        }
        public void InputWordTextBoxChanged(object? sender, KeyEventArgs? e)
        {
            var inputWordTextBox = sender as TextBox;
            if (inputWordTextBox != null) inputWord = inputWordTextBox.Text;

            if (loadInputWordBtn != null && !inputWordLoaded) loadInputWordBtn.IsEnabled = true;
            if (inputWord != null)
            {
                for (int i = 0; i < inputWord.Length; i++)
                {
                    if (!Mwvm.TuringMachine.TapeAlphabet.InputAlphabet.Contains(inputWord[i]))
                    {
                        if (loadInputWordBtn != null) loadInputWordBtn.IsEnabled = false;
                    }
                }
            }
        }
        public void LoadInputWordButton_Click(object sender, RoutedEventArgs e)
        {
            startBtn.IsEnabled = true;
            stopBtn.IsEnabled = true;
            slowSpeedBtn.IsEnabled = true;
            normalSpeedBtn.IsEnabled = true;
            fastSpeedBtn.IsEnabled = true;

            inputWordLoaded = true;
            loadInputWordBtn.IsEnabled = false;

            tableView.IsEnabled = false;
            diagrammToolboxView.IsEnabled = false;

            menu.IsEnabled = false;
            inputPanel.IsEnabled = false;

            Mwvm.SimulationContext.writeInputWordToTape(inputWord);
        }
        public void IA_KeyUp(object sender, KeyEventArgs e)
        {
            if (loadInputWordBtn != null && !inputWordLoaded) loadInputWordBtn.IsEnabled = true;
            if (inputWord != null)
            {
                for (int i = 0; i < inputWord.Length; i++)
                {
                    if (!Mwvm.TuringMachine.TapeAlphabet.InputAlphabet.Contains(inputWord[i]))
                    {
                        if (loadInputWordBtn != null) loadInputWordBtn.IsEnabled = false;
                    }
                }
            }

            Stop_Click(null, null);
        }
        
        private void tapeCount_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "TapeCount") 
            {
                initTapes(Mwvm.TapeCount);
            }
        }
       

        private void Mwvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SimulationContext" && Mwvm != null)
            {
                Mwvm.SimulationContext.CurrentSimulationStateChanged += new SimulationContext.CurrentSimulationStateChangedHandler(HandleSimulationStateChanged);
            }
        }

        public void HandleSimulationStateChanged(ISimulationState? ss)
        {
            if (Mwvm != null && Mwvm.SimulationContext.CurrentState != null)
            {
                if (Mwvm.SimulationContext.isWordAccepted() == true)
                {
                    Dispatcher.UIThread.InvokeAsync(() => {
                        acceptedTB.Text = "Eingabewort wurde akzeptiert. Aktueller Zustand: " + Mwvm.SimulationContext.CurrentState;
                        acceptedTB.Foreground = Brushes.Green;
                        startBtn.IsEnabled = false;
                    });
                }
                else if (Mwvm.SimulationContext.isWordAccepted() == false)
                {
                    Dispatcher.UIThread.InvokeAsync(() => {
                        acceptedTB.Text = "Eingabewort wurde nicht akzeptiert. Aktueller Zustand: " + Mwvm.SimulationContext.CurrentState;
                        acceptedTB.Foreground = Brushes.Red;
                        startBtn.IsEnabled = false;
                    });
                }
                else if (Mwvm.SimulationContext.isSimulationActive())
                {
                    Dispatcher.UIThread.InvokeAsync(() => {
                        acceptedTB.Text = "";
                        startBtn.IsEnabled = true;
                    });
                }
                else
                {
                    Dispatcher.UIThread.InvokeAsync(() => {
                        acceptedTB.Text = "";
                    });
                }
            }
        }
        
        
        public void addDiagramm(MainWindowViewModel Mwvm)
        {
            diagrammToolboxView = new ToolboxDiagram(Mwvm);
            var diagram = this.Find<Panel>("ToolboxDiagram");
            diagram.Children.Add(diagrammToolboxView);
        }
    }
}
