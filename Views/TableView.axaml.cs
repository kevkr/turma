using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using turma.Models.Simulation;
using turma.ViewModels;
using turma.Models.TuringMachine;
using Avalonia.Interactivity;
using Avalonia.Controls.Templates;
using System.Collections.Generic;
using Avalonia.Media;
using System;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using Avalonia.Threading;
using MsgBox;
using System.Threading.Tasks;

namespace turma.Views
{
    public partial class TableView : UserControl
    {
        private int index { get; set; }
        private MainWindowViewModel? Mwvm { get; set; }
        private Transition? currentTransition { get; set; }
        public bool loadRows { get; set; }

        public DataGrid? table;

        public TableView()
        {
            InitializeComponent();
        }

        public TableView(MainWindowViewModel MwvmFromwindow)
        {
            Mwvm = MwvmFromwindow;
            Mwvm.TuringMachineDefinitionChanged += new MainWindowViewModel.TuringMachineDefinitionChangedEventHandler(handleFileOpened);
            Mwvm.SimulationContext.CurrentTransitionChanged += new SimulationContext.CurrentTransitionChangedHandler(HandleCurrentTransitionChanged);
            Mwvm.PropertyChanged += Mwvm_PropertyChanged;
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            InitializeTable(null, null);
        }

        //InitializeTable creates the Table and the Buttons for the Tabs
        public void InitializeTable(object? sender, RoutedEventArgs? e)
        {
            if (Mwvm == null) return;
            //Initialize Table also gets called by the Buttons for the Tabs; If that happens it sets the index respectively
            if (sender != null)
            {
                Button b = (Button) sender;
                if (b.Name != null)
                index = Int32.Parse(b.Name);
            }

            //Gets the reference to the TM
            ITuringMachine TuringMachine = Mwvm.TuringMachine;

            AvaloniaXamlLoader.Load(this);

            //Gets the WrapPanel from the Xaml and places Buttons in it, which represent the Tabs
            var tabsWrapPanel = this.Find<WrapPanel>("TabsWrapPanel");
            for (int i = 0; i < TuringMachine.TapeCount; i++)
            {
                var button = new Button();
                button.Content = "Band " + (i + 1);
                button.Click += InitializeTable;
                button.Name = i.ToString();
                button.Margin = new Avalonia.Thickness(0,5,0,0);
                button.CornerRadius = new Avalonia.CornerRadius(4, 0);
                tabsWrapPanel.Children.Insert(i, button);

                if (index == i)
                {
                    button.Background = Brush.Parse("#c9c9c9");
                }
            }

            //Gets the DataGrid from the Xaml and defines the Columns

            table = this.FindControl<DataGrid>("Table");
            loadRows = false;

            table.Items = TuringMachine.Transitions;

            var col1 = new DataGridTemplateColumn();
            var col2 = new DataGridTemplateColumn();
            var col3 = new DataGridTemplateColumn();
            var col4 = new DataGridTemplateColumn();
            var col5 = new DataGridTemplateColumn();
            var col6 = new DataGridTemplateColumn();
            col1.Header = "Zustand";
            col2.Header = "Lesen";
            col3.Header = "Schreiben";
            col4.Header = "Bewegung";
            col5.Header = "Folgezustand";
            col6.Header = "Kommentar";
            var WidthOfColumn = new DataGridLength(100);
            col1.Width = WidthOfColumn;
            col2.Width = WidthOfColumn;
            col3.Width = WidthOfColumn;
            col4.Width = WidthOfColumn;
            col5.Width = WidthOfColumn;
            col6.Width = new DataGridLength(380);
            table.Columns.Add(col1);
            table.Columns.Add(col2);
            table.Columns.Add(col3);
            table.Columns.Add(col4);
            table.Columns.Add(col5);
            table.Columns.Add(col6);

            //A list of strings of the List of all states is needed to give the ComboBox the selectable Items
            List<string> StringListOfAllStates = new List<string>();
            foreach (State item in TuringMachine.StateSet.AllStates)
            {
                StringListOfAllStates.Add(item.Name);
            }

            //Defines the bindings of the columns respectively
            //ConditionState:
            col1.CellTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var comboBox = new ComboBox();
                comboBox.Items = StringListOfAllStates;
                var selectedItem = comboBox.GetObservable(ComboBox.SelectedItemProperty);
                selectedItem.Subscribe(value => { if (value != null) { itemModel.ConditionState = new State((string)value); } });
                if (itemModel != null && itemModel.ConditionState != null)
                {
                    comboBox.SelectedItem = itemModel.ConditionState.Name;
                }

                return comboBox;
            });
            //ConditionTapeSymbol:
            col2.CellTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var comboBox = new ComboBox();
                comboBox.Items = TuringMachine.TapeAlphabet.FullTapeAlphabet;
                var selectedItem = comboBox.GetObservable(ComboBox.SelectedItemProperty);
                selectedItem.Subscribe(value => { if (value != null) { itemModel.ConditionTapeSymbol[index] = (char)value; } });
                if (itemModel != null && index < itemModel.ConditionTapeSymbol.Length) comboBox.SelectedItem = itemModel.ConditionTapeSymbol[index];

                return comboBox;
            });
            //InstructionTapeSymbol:
            col3.CellTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var comboBox = new ComboBox();
                comboBox.Items = TuringMachine.TapeAlphabet.FullTapeAlphabet;
                var selectedItem = comboBox.GetObservable(ComboBox.SelectedItemProperty);
                selectedItem.Subscribe(value => { if (value != null) { itemModel.InstructionTapeSymbol[index] = (char)value; } });
                if (itemModel != null && index < itemModel.InstructionTapeSymbol.Length) comboBox.SelectedItem = itemModel.InstructionTapeSymbol[index];

                return comboBox;
            });
            //Direction:
            col4.CellTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var comboBox = new ComboBox();
                comboBox.Items = Enum.GetValues(typeof(Transition.Direction)).Cast<Transition.Direction>();
                var selectedItem = comboBox.GetObservable(ComboBox.SelectedItemProperty);
                selectedItem.Subscribe(value => { if (value != null) { itemModel.Directions[index] = (Transition.Direction)value; } });
                if (itemModel != null && index < itemModel.Directions.Length) comboBox.SelectedItem = itemModel.Directions[index];

                return comboBox;
            });
            //InstructionState:
            col5.CellTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var comboBox = new ComboBox();
                comboBox.Items = StringListOfAllStates;
                var selectedItem = comboBox.GetObservable(ComboBox.SelectedItemProperty);
                selectedItem.Subscribe(value => { if (value != null) { itemModel.InstructionState = new State((string)value); } });
                if (itemModel != null && itemModel.InstructionState != null)
                {
                    comboBox.SelectedItem = itemModel.InstructionState.Name;
                }

                return comboBox;
            });
            //Description:
            col6.CellTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var textBox = new TextBox();
                var writtenText = textBox.GetObservable(TextBox.TextProperty);
                writtenText.Subscribe(value => { if (value != null) { itemModel.Description = value; } });
                if (itemModel != null) textBox.Text = itemModel.Description;

                return textBox;
            });
        }

        //New Transition is added by a Button click
        public void AddTransition(object sender, RoutedEventArgs e)
        {
            if (Mwvm == null) return;
            ITuringMachine TuringMachine = Mwvm.TuringMachine;
            TuringMachine.Transitions.Add(
                    new Transition(new State(""), new State(""), new char[TuringMachine.TapeCount], new char[TuringMachine.TapeCount], new Transition.Direction[TuringMachine.TapeCount], ""));

            InitializeComponent();
        }

        //Selected Transition is removed by a Button click
        public async void RemoveTransition(object sender, RoutedEventArgs e)
        {
            if (Mwvm == null || table == null) return;
            ITuringMachine TuringMachine = Mwvm.TuringMachine;

            if (TuringMachine.Transitions.Count > 0 && table.SelectedIndex >= 0)
            {
                Task<MessageBox.MessageBoxResult> m;
                await (m = MessageBox.Show(App.MainWindow, "Möchten Sie den ausgewählten Übergang löschen?", "Löschen bestätigen", MessageBox.MessageBoxButtons.OkCancel));
                if (m.Result == MessageBox.MessageBoxResult.Ok)
                {
                    TuringMachine.Transitions.RemoveAt(table.SelectedIndex);
                }
            }

            InitializeComponent();
        }

        //Enables or disables Button to remove transitions if an item is selected or not
        public void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            var tabledatagrid = sender as DataGrid;

            var removeTransitionButton = this.FindControl<Button>("RemoveTransition");
            if (tabledatagrid != null)
            {
                if (tabledatagrid.SelectedIndex != -1)
                {
                    removeTransitionButton.IsEnabled = true;
                }
                else
                {
                    removeTransitionButton.IsEnabled = false;
                }
            }
        }

        //When a file gets opened the table gets reloaded
        public void handleFileOpened(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        //When the CurrentTransition changes, the table gets reloaded and the "Loading_Rows" event gets enabled
        public void HandleCurrentTransitionChanged(Transition? transition)
        {
            if (Mwvm == null) return;

            if (transition != null) currentTransition = transition;
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                InitializeTable(null, null);
                loadRows = true;
            });
        }

        //When Mwvm changes, eventhandlers have to be added again
        private void Mwvm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SimulationContext" && Mwvm != null)
            {
                Mwvm.SimulationContext.CurrentTransitionChanged += new SimulationContext.CurrentTransitionChangedHandler(HandleCurrentTransitionChanged);
            }
        }

        //Colors the current transition and scrolls to it
        public void dataGrid_LoadingRows(object sender, DataGridRowEventArgs e)
        {
            if (loadRows && Mwvm != null && table != null)
            {
                var dataObject = e.Row.DataContext as Transition;
                if (Mwvm.SimulationContext.TransitionHistory.transitionStack.Any())
                {
                    if (dataObject != null && dataObject == Mwvm.SimulationContext.TransitionHistory.transitionStack.First())
                    {
                        e.Row.Background = Brushes.CornflowerBlue;
                    }
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        table.ScrollIntoView(Mwvm.SimulationContext.TransitionHistory.transitionStack.First(), null);
                    });
                }
            }
        }

    }
}
