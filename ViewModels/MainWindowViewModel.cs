using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive;
using System.Text;
using turma.Models.TuringMachine;
using turma.Models.Simulation;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using MsgBox;
using turma.Views;
using Avalonia;

namespace turma.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public TuringMachine TuringMachine;
        
        public Serializer Serializer;
        public Validator Validator;
        public string CurrentFilePath { get; set; }

        public delegate void TuringMachineDefinitionChangedEventHandler(object sender, EventArgs e);

        public event TuringMachineDefinitionChangedEventHandler? TuringMachineDefinitionChanged;

        public delegate void TransitionClickedEventHandler(Tuple<State,State> tuple);

        public event TransitionClickedEventHandler? TransitionClicked;

        public int SelectedTableIndex { get; set; }

        private string allStates;
        private string acceptingStates;
        private string tapeAlphabet;
        private string inputAlphabet;
        private string? startingState;
        private int tapeCount;
        private bool showTable = true;
        private bool openSidebar = false;
        private bool isTransformable = true;
        private SimulationContext _simulationContext;

        public SimulationContext SimulationContext
        {
            get { return _simulationContext; }
            set { this.RaiseAndSetIfChanged(ref _simulationContext, value); }
        }

        public string AllStates
        {
            get { return allStates; }
            set
            {
                this.RaiseAndSetIfChanged(ref allStates, value);
                TuringMachine.StateSet.AllStates = convertStringToStates(value);
                raiseDefChanged();
            }
        }
        public string AcceptingStates
        {
            get => acceptingStates;
            set
            {
                this.RaiseAndSetIfChanged(ref acceptingStates, value);
                TuringMachine.StateSet.AcceptingStates = convertStringToStates(value);
                raiseDefChanged();
            }
        }
        public string TapeAlphabet
        {
            get => tapeAlphabet;
            set
            {
                this.RaiseAndSetIfChanged(ref tapeAlphabet, value);
                TuringMachine.TapeAlphabet.TapeSymbols = convertStringToTapeSymbols(value);
                raiseDefChanged();
            }
        }
        public string InputAlphabet
        {
            get => inputAlphabet;
            set
            {
                this.RaiseAndSetIfChanged(ref inputAlphabet, value);
                TuringMachine.TapeAlphabet.InputAlphabet = convertStringToTapeSymbols(value);
                raiseDefChanged();
            }
        }
        public string? StartingState
        {
            get => startingState;
            set
            {
                this.RaiseAndSetIfChanged(ref startingState, value);
                TuringMachine.StateSet.StartingState = null;
                if (value != null && value.Length > 0) TuringMachine.StateSet.StartingState = new State(value);
                raiseDefChanged();
            }
        }

        public int TapeCount
        {
            get => tapeCount;
            set
            {
                if (value < TapeCount)
                {
                    LowerTapeCount(value, TuringMachine.TapeCount);
                }
                else if (value > TapeCount)
                {
                    setTapeCount(value);
                    TuringMachine.TapeCount = value;
                    this.RaiseAndSetIfChanged(ref tapeCount, value);
                    raiseDefChanged();
                } else if(value == TapeCount)
                {
                    this.RaiseAndSetIfChanged(ref tapeCount, 0);
                    this.RaiseAndSetIfChanged(ref tapeCount, value);
                    raiseDefChanged();
                }
            }
        }

        public async Task LowerTapeCount(int newValue, int oldValue)
        {
            int diff = (oldValue - newValue);

            string message = "";
            if (diff > 1) message += "Die letzten " + diff + " Bänder löschen?";
            else message += "Das letzte Band löschen?";
            message += "\nDabei werden alle Übergänge für ";
            if (diff > 1) message += "diese Bänder gelöscht.";
            else message += "das Band gelöscht.";

            Task<MessageBox.MessageBoxResult> m;
            await (m = MessageBox.Show(App.MainWindow, message, "Löschen bestätigen", MessageBox.MessageBoxButtons.OkCancel));
            if (m.Result == MessageBox.MessageBoxResult.Ok)
            {
                setTapeCount(newValue);
                tapeCount = newValue;
                TapeCount = newValue;

            } 
            else 
            {
                tapeCount = oldValue;
                TapeCount = oldValue;
            }
        }
        public bool IsTransformable
        {
            get { return isTransformable; }
            set
            {
                this.RaiseAndSetIfChanged(ref isTransformable, value);
            }
        }

        // True: Show Table, False: Show Diagram
        public bool ShowTable
        {
            get => showTable; set { this.RaiseAndSetIfChanged(ref showTable, value); }
        }

        public bool OpenSidebar {
            get => openSidebar; set { this.RaiseAndSetIfChanged(ref openSidebar, value);}
        }

        public MainWindowViewModel()
        {
            TuringMachine = new TuringMachine();
            SimulationContext = new SimulationContext(TuringMachine);
            Serializer = new Serializer();
            Validator = new Validator();
            CurrentFilePath = "";

            allStates = "";
            acceptingStates = "";
            tapeAlphabet = "";
            inputAlphabet = "";
            tapeCount = 1;
        }


        public bool isTuringMachineDefinitionValid()
        {
            try 
            {
                Validator.validate(TuringMachine);
            }
            catch (Exception e)
            { 
                return false;
            }
            return true;
        }

        public void Save()
        {
            try
            {
                syncToModel();
                string yml = Serializer.serialize((TuringMachine)TuringMachine);
                File.WriteAllText(CurrentFilePath, yml);
            } catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show(App.MainWindow, "Die Turing Maschine konnte nicht gespeichert werden. \nException: " + e.Message, "Error", MessageBox.MessageBoxButtons.Ok);
            }
        }
        public void Save(string path)
        {
            try
            {
                syncToModel();
                CurrentFilePath = path;
                string yml = Serializer.serialize((TuringMachine)TuringMachine);
                File.WriteAllText(path, yml);
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show(App.MainWindow, "Die Turing Maschine konnte nicht gespeichert werden. \nException: "+e.Message, "Error", MessageBox.MessageBoxButtons.Ok);
            }
        }

        public void LoadFile(string path)
        {
            CurrentFilePath = path;
            string yml = File.ReadAllText(path);
            TuringMachine? result = Serializer.deserialize(yml);
            if (result != null)
            {
                TuringMachine = result;
                SimulationContext.TuringMachine = result;
                syncFromModel();
                raiseDefChanged();
            }
            else
            {
                MessageBox.Show(App.MainWindow, "Die Datei konnte nicht eingelesen werden.", "Error", MessageBox.MessageBoxButtons.Ok);
            }
            
        }
        public void setTapeCount(int count)
        {
            if (count < 1) return;

            Tape[] newTapes = new Tape[count];
            ReadWriteHead[] newReadWriteHeads = new ReadWriteHead[count];
            newTapes[0] = SimulationContext.Tapes[0];
            newReadWriteHeads[0] = new ReadWriteHead(newTapes[0]);
            for (int i = 1; i < count; i++)
            {
                newTapes[i] = new Tape();
                newReadWriteHeads[i] = new ReadWriteHead(newTapes[i]);
            }
            SimulationContext.Tapes = newTapes;
            SimulationContext.ReadWriteHeads = newReadWriteHeads;

            if (TuringMachine.TapeCount > count)
            {
                List<Transition> newTransitions = new List<Transition>();
                for (int i = 0; i < TuringMachine.Transitions.Count; i++)
                {
                    char[] newConditionTapeSymbol = new char[count];
                    char[] newInstructionTapeSymbol = new char[count];
                    Transition.Direction[] newDirections = new Transition.Direction[count];
                    for (int t = 0; t < count; t++)
                    {
                        newConditionTapeSymbol[t] = TuringMachine.Transitions[i].ConditionTapeSymbol[t];
                        newInstructionTapeSymbol[t] = TuringMachine.Transitions[i].InstructionTapeSymbol[t];
                        newDirections[t] = TuringMachine.Transitions[i].Directions[t];
                    }
                    newTransitions.Add(new Transition(TuringMachine.Transitions[i].ConditionState, TuringMachine.Transitions[i].InstructionState,
                        newConditionTapeSymbol, newInstructionTapeSymbol, newDirections, TuringMachine.Transitions[i].Description)); ;
                }
                TuringMachine.Transitions = newTransitions;
                TuringMachine.TapeCount = count;
                Debug.WriteLine("New Tapecount = "+TuringMachine.TapeCount);
            }
            else if (TuringMachine.TapeCount < count)
            {
                List<Transition> newTransitions = new List<Transition>();
                for (int i = 0; i < TuringMachine.Transitions.Count; i++)
                {
                    char[] newConditionTapeSymbol = new char[count];
                    char[] newInstructionTapeSymbol = new char[count];
                    Transition.Direction[] newDirections = new Transition.Direction[count];
                    for (int t = 0; t < TuringMachine.Transitions[i].ConditionTapeSymbol.Length; t++)
                    {
                        newConditionTapeSymbol[t] = TuringMachine.Transitions[i].ConditionTapeSymbol[t];
                        newInstructionTapeSymbol[t] = TuringMachine.Transitions[i].InstructionTapeSymbol[t];
                        newDirections[t] = TuringMachine.Transitions[i].Directions[t];
                    }
                    newTransitions.Add(new Transition(TuringMachine.Transitions[i].ConditionState, TuringMachine.Transitions[i].InstructionState,
                        newConditionTapeSymbol, newInstructionTapeSymbol, newDirections, TuringMachine.Transitions[i].Description)); ;
                }
                TuringMachine.Transitions = newTransitions;
                TuringMachine.TapeCount = count;
                Debug.WriteLine("New Tapecount = "+TuringMachine.TapeCount);
            }
            raiseDefChanged();
        }

        public bool startSimulation()
        {
            syncToModel();
            if (isTuringMachineDefinitionValid())
            {
                SimulationContext.startSimulation();
                return true;
            }
            else
            {
                try
                {
                    Validator.validate(TuringMachine);
                }
                catch (Exception e)
                {
                    MessageBox.Show(App.MainWindow, "Ungültige Turing Machine:\n" + e.Message, "Error", MessageBox.MessageBoxButtons.Ok);
                }
                return false;
            }
        }
        public void stopSimulation()
        {
            SimulationContext.stopSimulation();
        }
        public void pauseSimulation()
        {
            SimulationContext.pauseSimulation();
        }
        public void stepForward()
        {
            SimulationContext.stepForward();
        }
        public void stepBack()
        {
            SimulationContext.stepBack();
        }
        public void updateTransitionView()
        {

        }
        public void setAnimationSpeed(double speed)
        {
            SimulationContext.AnimationSpeed = speed;
        }

        public void transformToEquivalentMachine(Task<string> dialogRes)
        {
            if (isTransformable)
            {
                TuringMachine backup = TuringMachine.Clone();
                try
                {
                    TuringMachineTransformer tmTransformer = new TuringMachineTransformer();
                    if (dialogRes.Result[0] == '1')
                    {
                        tmTransformer.NoTransitionsIntoStartingState(TuringMachine);
                    }
                    if (dialogRes.Result[1] == '1')
                    {
                        tmTransformer.NeverStay(TuringMachine);
                    }
                    if (dialogRes.Result[3] == '1')
                    {
                        TuringMachine = tmTransformer.LeftAndRightStates(TuringMachine);
                    }
                    if (dialogRes.Result[2] == '1')
                    {
                        tmTransformer.NeverWriteEpsilon(TuringMachine);
                    }
                    if (dialogRes.Result[4] == '1')
                    {
                        tmTransformer.MergeAcceptingStates(TuringMachine);
                    }
                    SimulationContext.TuringMachine = TuringMachine;
                    raiseDefChanged();
                    syncFromModel();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    TuringMachine = backup;
                    MessageBox.Show(App.MainWindow, "Die Turing Maschine konnte nicht transformiert werden. "+e.Message, "Error", MessageBox.MessageBoxButtons.Ok);
                }
            }
        }

        public void isWordAccepted()
        {

        }

        public void checkTransformability()
        {
            try
            {
                if (TapeCount == 1 && isTuringMachineDefinitionValid())
                {
                    IsTransformable = true;
                }
                else
                {
                    IsTransformable = false;
                }
            } catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                IsTransformable=false;
            }
        }

        public void raiseDefChanged()
        {
            checkTransformability();
            if (TuringMachineDefinitionChanged != null)
            {
                TuringMachineDefinitionChanged(this, new EventArgs());
            }
        }

        public void syncToModel()
        {
            TuringMachine.StateSet.AllStates.Clear();
            TuringMachine.StateSet.AllStates.AddRange(convertStringToStates(AllStates));

            TuringMachine.StateSet.AcceptingStates.Clear();
            TuringMachine.StateSet.AcceptingStates.AddRange(convertStringToStates(AcceptingStates));

            if (startingState != null)
                TuringMachine.StateSet.StartingState = new State(startingState);

            TuringMachine.TapeAlphabet.TapeSymbols.Clear();
            TuringMachine.TapeAlphabet.TapeSymbols.AddRange(convertStringToTapeSymbols(TapeAlphabet));

            TuringMachine.TapeAlphabet.InputAlphabet.Clear();
            TuringMachine.TapeAlphabet.InputAlphabet.AddRange(convertStringToTapeSymbols(InputAlphabet));

            TuringMachine.TapeCount = TapeCount;
        }

        public void syncFromModel()
        {
            AllStates = convertStatesToString(TuringMachine.StateSet.AllStates);

            AcceptingStates = convertStatesToString(TuringMachine.StateSet.AcceptingStates);


            if (TuringMachine.StateSet.StartingState != null)
            {
                StartingState = TuringMachine.StateSet.StartingState.Name;
            }
            else
            {
                StartingState = null;
            }

            TapeAlphabet = convertTapeSymbolsToString(TuringMachine.TapeAlphabet.TapeSymbols);
            InputAlphabet = convertTapeSymbolsToString(TuringMachine.TapeAlphabet.InputAlphabet);

            //When syncing from the model (e.g. because a file was opened) we want to set the tapeCount
            //without opening the popup in LowerTapeCount()
            tapeCount = TuringMachine.TapeCount;
            setTapeCount(tapeCount);
            this.RaisePropertyChanged(nameof(TapeCount));

            checkTransformability();
        }

        public string convertStatesToString(List<State> states)
        {
            StringBuilder sb = new StringBuilder();

            foreach (State state in states)
            {
                sb.Append(state.Name);
                sb.Append(", ");
            }
            if (states.Count > 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            return sb.ToString();
        }

        public List<State> convertStringToStates(string inputStates)
        {
            List<State> resultStates = new List<State>();
            string[] stateStrings = inputStates.Split(',');
            foreach (string state in stateStrings)
            {
                if (!string.IsNullOrEmpty(state.Trim()))
                {
                    resultStates.Add(new State(state.Trim()));
                }
            }
            return resultStates;
        }

        public string convertTapeSymbolsToString(List<char> symbols)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char symbol in symbols)
            {
                sb.Append(symbol);
                sb.Append(", ");
            }
            if (symbols.Count > 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }
            return sb.ToString();
        }

        public List<char> convertStringToTapeSymbols(string inputSymbols)
        {
            List<char> resultSymbols = new List<char>();
            inputSymbols = inputSymbols.Trim();
            string[] symbolStrings = inputSymbols.Split(',');
            foreach (string symbol in symbolStrings)
            {
                if (symbol.Length > 0 && symbol.Trim().Length > 0)
                    resultSymbols.Add(symbol.Trim()[0]);
            }
            return resultSymbols;
        }

        public void raiseTransitionClicked(Tuple<State,State> tuple)
        {
            TransitionClicked(tuple);
        }

        public async Task transfromTMDialog()
        {
            string message = "Welche Eigenschaften sollte die transformierte Turingmaschine haben?\n";
            Task<string> m;
            await (m = DialogBox.Show(App.MainWindow, message, "Wählen Sie TM-Eigenschaften"));
            transformToEquivalentMachine(m);
            return;
        }
    }
}