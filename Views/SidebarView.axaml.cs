using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using turma.Models.TuringMachine;
using turma.ViewModels;

namespace turma.Views
{
    public partial class SidebarView : UserControl
    {
        private MainWindowViewModel Mwvm { get; set; }
        private int index { get; set; }
        private Tuple<State,State>? selectedStates { get; set; }
        public SidebarView()
        {
            InitializeComponent();
        }

        public SidebarView(MainWindowViewModel MwvmFromwindow)
        {
            Mwvm = MwvmFromwindow;
            Mwvm.TuringMachineDefinitionChanged += new MainWindowViewModel.TuringMachineDefinitionChangedEventHandler(handleTuringMachineDefinitionChanged);
            Mwvm.TransitionClicked += new MainWindowViewModel.TransitionClickedEventHandler(handleTransitionClicked);
            DataContext = Mwvm;
            Mwvm.PropertyChanged += new PropertyChangedEventHandler(handlePropertyChanged);
            InitializeComponent();
            ClosePanel();
        }

        public void InitializeComponent()
        {
            InitializeSidepanel(null, null);
            if (Mwvm != null && Mwvm.OpenSidebar)
            {
                OpenPanel();
            }
        }

        //Initializing the Sidebar Elements
        public void InitializeSidepanel(object? sender, RoutedEventArgs? e)
        {
            if (sender != null)
            {
                Button b = (Button)sender;
                if (b.Name != null)
                    index = Int32.Parse(b.Name);
            }

            ITuringMachine TuringMachine = Mwvm.TuringMachine;
            AvaloniaXamlLoader.Load(this);


            //Create Tabs for the different Tapes
            var tabsWrapPanel = this.Find<WrapPanel>("TabsWrapPanel");
            for (int i = 0; i < TuringMachine.TapeCount; i++)
            {
                var button = new Button();
                button.Content = "Band " + (i + 1);
                button.Click += InitializeSidepanel;
                button.Click += OnOpenPanel;
                button.Name = i.ToString();
                button.Margin = new Avalonia.Thickness(0, 5, 0, 0);
                button.CornerRadius = new Avalonia.CornerRadius(4, 0);
                tabsWrapPanel.Children.Insert(i, button);

                if (index == i)
                {
                    button.Background = Brush.Parse("#c9c9c9");
                }
            }

            List<string> StringListOfAllStates = new List<string>();
            foreach (State item in TuringMachine.StateSet.AllStates)
            {
                StringListOfAllStates.Add(item.Name);
            }

            //Find UI Elements already created in XAML
            var tsp = this.FindControl<Panel>("TransitionSidePanel");
            var kb = tsp.FindControl<TextBox>("Kommentar");
            var sv = tsp.FindControl<ScrollViewer>("sv");
            var listbox = sv.FindControl<ListBox>("TransitionView");
            var noTransitionMessage = listbox.FindControl<TextBlock>("noTransition");
            if (selectedStates == null) return;
            //listbox.Items = TuringMachine.Transitions;

            
            //Sidebar Header
            WrapPanel Header = new WrapPanel();
            Header.Margin = new Thickness(10, 0, 0, 10);
            var StartOfSentence = new TextBlock();
            StartOfSentence.Text = "Übergänge von ";
            StartOfSentence.FontSize = 14;
            var SelectedState1Bold = new TextBlock();
            SelectedState1Bold.Text = selectedStates.Item1.ToString();
            SelectedState1Bold.FontWeight = FontWeight.Bold;
            SelectedState1Bold.FontSize = 15;
            var EndOfSentence = new TextBlock();
            EndOfSentence.Text = " zu ";
            EndOfSentence.FontSize = 14;
            var SelectedState2Bold = new TextBlock();
            SelectedState2Bold.Text = selectedStates.Item2.ToString();
            SelectedState2Bold.FontWeight = FontWeight.Bold;
            SelectedState2Bold.FontSize = 15;
            
            Header.Children.Add(StartOfSentence);
            Header.Children.Add(SelectedState1Bold);
            Header.Children.Add(EndOfSentence);
            Header.Children.Add(SelectedState2Bold);
            tsp.Children.Add(Header);

            var selectedTransitions = new List<Transition>();

            //Filter Transitions according to selected State (WIP)
            for (int x = 0; x < TuringMachine.Transitions.Count; x++)
            {
                if (TuringMachine.Transitions[x].ConditionState.ToString() == selectedStates.Item1.ToString() && TuringMachine.Transitions[x].InstructionState.ToString() == selectedStates.Item2.ToString())
                {
                    selectedTransitions.Add(TuringMachine.Transitions[x]);
                }
            }

            //listbox.Items = selectedTransitions;
            //Console.Write(selectedTransitions);

            if (selectedStates.Item1.ToString() == "" && selectedStates.Item2.ToString() == "")
            {
                noTransitionMessage.IsVisible = true;
                sv.IsVisible = true; 
                listbox.IsVisible = true;
                Header.IsVisible = false;
            }
            else
            {
                noTransitionMessage.IsVisible = false;
                Header.IsVisible = true;
                //listbox.Items = TuringMachine.Transitions;
                listbox.Items = selectedTransitions;
                Console.Write(selectedTransitions);
            }

            //Create Sidebar Transition UI Elements
            listbox.ItemTemplate = new FuncDataTemplate<Transition>((itemModel, namescope) =>
            {
                var transitionBorder = new Border();
                transitionBorder.BorderBrush = Brushes.Black;
                transitionBorder.BorderThickness = new Thickness(2);
                transitionBorder.Padding = new Thickness(5);

                var transitionStackpanel = new StackPanel();
                var tapePanel = new StackPanel();
                var tapeGrid = new Grid();

                //Creating the Grid
                ColumnDefinition column2 = new ColumnDefinition();
                ColumnDefinition column3 = new ColumnDefinition();
                ColumnDefinition column4 = new ColumnDefinition();

                column2.Width = new GridLength(81);
                column3.Width = new GridLength(81);
                column4.Width = new GridLength(81);

                tapeGrid.ColumnDefinitions.Add(column2);
                tapeGrid.ColumnDefinitions.Add(column3);
                tapeGrid.ColumnDefinitions.Add(column4);

                RowDefinition row1 = new RowDefinition();
                RowDefinition row2 = new RowDefinition();
                RowDefinition row3 = new RowDefinition();

                row1.Height = new GridLength(15);
                row2.Height = new GridLength(35);
                row3.Height = new GridLength(55);

                tapeGrid.RowDefinitions.Add(row1);
                tapeGrid.RowDefinitions.Add(row2);
                tapeGrid.RowDefinitions.Add(row3);

                tapePanel.Children.Add(tapeGrid);

                //Create Combobox Descriptions
                var textBlockLesen = new TextBlock();
                textBlockLesen.Text = "Lesen";
                textBlockLesen.FontSize = 10;
                textBlockLesen.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

                var textBlockBewegung = new TextBlock();
                textBlockBewegung.Text = "Bewegung";
                textBlockBewegung.FontSize = 10;
                textBlockBewegung.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

                var textBlockSchreiben = new TextBlock();
                textBlockSchreiben.Text = "Schreiben";
                textBlockSchreiben.FontSize = 10;
                textBlockSchreiben.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

                //Condition Tape Symbol Combobox
                var comboBoxCTS = new ComboBox();
                comboBoxCTS.Items = TuringMachine.TapeAlphabet.FullTapeAlphabet;
                comboBoxCTS.Name = "combo";
                comboBoxCTS.FontSize = 15;
                comboBoxCTS.Width = 80;
                comboBoxCTS.Height = 35;
                var selectedItemCTS = comboBoxCTS.GetObservable(ComboBox.SelectedItemProperty);
                selectedItemCTS.Subscribe(value => { if (value != null) { itemModel.ConditionTapeSymbol[index] = (char)value; } });
                if (itemModel != null) comboBoxCTS.SelectedItem = itemModel.ConditionTapeSymbol[index];

                //Direction Combobox
                var comboBoxD = new Avalonia.Controls.ComboBox();
                comboBoxD.Items = Enum.GetValues(typeof(Transition.Direction)).Cast<Transition.Direction>();
                comboBoxD.FontSize = 15;
                comboBoxD.Width = 80;
                comboBoxD.Height = 35;
                var selectedItemD = comboBoxD.GetObservable(ComboBox.SelectedItemProperty);
                selectedItemD.Subscribe(value => { if (value != null) { itemModel.Directions[index] = (Transition.Direction)value; } });
                if (itemModel != null) comboBoxD.SelectedItem = itemModel.Directions[index];

                //Instruction Tape Symbol Combobox
                var comboBoxITS = new Avalonia.Controls.ComboBox();
                comboBoxITS.Items = TuringMachine.TapeAlphabet.FullTapeAlphabet;
                comboBoxITS.FontSize = 15;
                comboBoxITS.Width = 80;
                comboBoxITS.Height = 35;
                var selectedItemITS = comboBoxITS.GetObservable(ComboBox.SelectedItemProperty);
                selectedItemITS.Subscribe(value => { if (value != null) { itemModel.InstructionTapeSymbol[index] = (char)value; } });
                if (itemModel != null) comboBoxITS.SelectedItem = itemModel.InstructionTapeSymbol[index];

                //Commentbox (WIP)
                var commentBox = new TextBox();
                commentBox.Watermark = "Kommentar";
                commentBox.TextWrapping = TextWrapping.Wrap;
                commentBox.Margin = new Thickness(0, 5, 0, 0);
                var writtenText = commentBox.GetObservable(TextBox.TextProperty);
                writtenText.Subscribe(value => { if (value != null) { itemModel.Description = value; } });
                if (itemModel != null) commentBox.Text = itemModel.Description;

                //Add Row to Grid
                tapeGrid.RowDefinitions.Add(new RowDefinition());

                //Add Combobox to Grid
                tapeGrid.Children.Add(comboBoxCTS);
                Grid.SetColumn(comboBoxCTS, 0);
                Grid.SetRow(comboBoxCTS, 1);
                tapeGrid.Children.Add(comboBoxD);
                Grid.SetColumn(comboBoxD, 1);
                Grid.SetRow(comboBoxD, 1);
                tapeGrid.Children.Add(comboBoxITS);
                Grid.SetColumn(comboBoxITS, 2);
                Grid.SetRow(comboBoxITS, 1);

                //Add Commentbox to Grid
                tapeGrid.Children.Add(commentBox);
                Grid.SetColumn(commentBox, 0);
                Grid.SetColumnSpan(commentBox, 3);
                Grid.SetRow(commentBox, 2);

                //Add Combobox Description Textblocks to Grid
                tapeGrid.Children.Add(textBlockLesen);
                Grid.SetColumn(textBlockLesen, 0);
                Grid.SetRow(textBlockLesen, 0);
                tapeGrid.Children.Add(textBlockBewegung);
                Grid.SetColumn(textBlockBewegung, 1);
                Grid.SetRow(textBlockBewegung, 0);
                tapeGrid.Children.Add(textBlockSchreiben);
                Grid.SetColumn(textBlockSchreiben, 2);
                Grid.SetRow(textBlockSchreiben, 0);

                
                //transitionStackpanel.Children.Add(textBlockTransitionNumber);
                transitionStackpanel.Children.Add(tapePanel);
                transitionBorder.Child = transitionStackpanel;

                return transitionBorder;
            });

        }

        //Add a Transition on Button Click
        public void AddTransition(object sender, RoutedEventArgs e)
        {
            ITuringMachine TuringMachine = Mwvm.TuringMachine;

            if (selectedStates == null) return;
            TuringMachine.Transitions.Add(
            new Transition(selectedStates.Item1, selectedStates.Item2, new char[TuringMachine.TapeCount], new char[TuringMachine.TapeCount], new Transition.Direction[TuringMachine.TapeCount], ""));

            InitializeComponent();

        }

        //Remove the selected Transition on Button Click
        public async void RemoveTransition(object sender, RoutedEventArgs e)
        {
            //Find UI Elements
            var tsp = this.FindControl<Panel>("TransitionSidePanel");
            var sv = tsp.FindControl<ScrollViewer>("sv");
            var listbox = sv.FindControl<ListBox>("TransitionView");

            //Remove Transition and Listbox UI Element on the selected Index

            if (listbox.SelectedItem != null)
            {
                Task<MessageBox.MessageBoxResult> m;
                await(m = MessageBox.Show(App.MainWindow, "Möchten Sie den ausgewählten Übergang löschen?", "Löschen bestätigen", MessageBox.MessageBoxButtons.OkCancel));
                if (m.Result == MessageBox.MessageBoxResult.Ok)
                {
                    Mwvm.TuringMachine.Transitions.Remove((Transition)(listbox.SelectedItem));
                }
            }

            InitializeComponent();
        }

        //Close the Sidepanel on Button Click
        public void OnClosePanel(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        //Open the Sidepanel on Button Click
        public void OnOpenPanel(object? sender, RoutedEventArgs e)
        {
            OpenPanel();
        }

        //Close the Panel
        public void ClosePanel()
        {
            Mwvm.OpenSidebar = false;
            var tsp = this.FindControl<Panel>("TransitionSidePanel");
            tsp.IsVisible = false;
            var opb = this.FindControl<Button>("OpenPanelButton");
            opb.IsVisible = true;
        }

        //Open the Panel
        public void OpenPanel()
        {
            Mwvm.OpenSidebar = true;
            var tsp = this.FindControl<Panel>("TransitionSidePanel");
            tsp.IsVisible = true;
            var opb = this.FindControl<Button>("OpenPanelButton");
            opb.IsVisible = false;
        }

        public void handleTuringMachineDefinitionChanged(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        public void handleTransitionClicked(Tuple<State, State> tuple)
        {
            selectedStates = tuple;
            InitializeComponent();
            OpenPanel();
        }

        public void handlePropertyChanged(object sender, PropertyChangedEventArgs e) { 
            if (e.PropertyName == "ShowTable")
            {
                ClosePanel();
            }
        } 

    }
}
