using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.ViewModels
{
    public class TuringMachineTransformer
    {
        public void NoTransitionsIntoStartingState(TuringMachine turingMachine)
        {
            if (turingMachine.StateSet.StartingState == null) return;
            State proxyState = new State(turingMachine.StateSet.StartingState.Name + "Proxy");
            if (turingMachine.StateSet.AllStates.Contains(proxyState))
            {
                Random rand = new Random();
                proxyState.Name += rand.Next(1000);
            }
            List<Transition> insertionList = new List<Transition>();
            turingMachine.StateSet.AllStates.Add(proxyState);
            foreach (Transition t in turingMachine.Transitions)
            {
                if (t.InstructionState != null && t.ConditionState != null)
                {
                    if (t.InstructionState.Equals(turingMachine.StateSet.StartingState))
                    {
                        t.InstructionState = proxyState;
                    }
                    if (t.ConditionState.Equals(turingMachine.StateSet.StartingState))
                    {
                        insertionList.Add(new Transition(proxyState, t.InstructionState, t.ConditionTapeSymbol, t.InstructionTapeSymbol, t.Directions, ""));
                    }
                }
            }
            turingMachine.Transitions.AddRange(insertionList);
        }

        public void NeverWriteEpsilon(TuringMachine turingMachine)
        {
            if (turingMachine.TapeAlphabet.FullTapeAlphabet.Contains('^'))
            {
                throw new Exception("Das Zirkumflex Zeichen ist im Bandalphabet oder Eingabealphabet nicht zugelassen");
            }
            else
            {
                turingMachine.TapeAlphabet.TapeSymbols.Add('^');
            }

            for (int i = 0; i < turingMachine.Transitions.Count; i++)
            {
                if (turingMachine.Transitions[i].InstructionTapeSymbol[0] == TapeAlphabet.Epsilon)
                {
                    turingMachine.Transitions[i].InstructionTapeSymbol = "^".ToCharArray();
                }
                if (turingMachine.Transitions[i].ConditionTapeSymbol[0] == TapeAlphabet.Epsilon)
                {
                    turingMachine.Transitions[i].ConditionTapeSymbol = "^".ToCharArray();
                }
            }
            foreach (State s in turingMachine.StateSet.AllStates)
            {
                turingMachine.Transitions.Add(new Transition(s, s, new char[] { TapeAlphabet.Epsilon}, "^".ToCharArray(), new Transition.Direction[] { Transition.Direction.stay }, ""));
            }
        }

        public void NeverStay(TuringMachine turingMachine)
        {
            //RWH always moves
            List<Transition> markedForRemoval = new List<Transition>();
            List<Transition> insertionList = new List<Transition>();
            foreach (Transition t in turingMachine.Transitions)
            {
                if (t.Directions[0] == Transition.Direction.stay && t.ConditionState != null && t.InstructionState != null)
                {
                    State proxyState = new State(t.ConditionState.Name + "-" + t.InstructionState.Name + "ProxyState");
                    if (turingMachine.StateSet.AllStates.Contains(proxyState))
                    {
                        proxyState.Name = proxyState.Name + proxyState.GetHashCode();
                    }
                    turingMachine.StateSet.AllStates.Add(proxyState);
                    t.Directions[0] = Transition.Direction.right;
                    Transition transitionToProxy = new Transition(t.ConditionState, proxyState, 
                        t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.right }, "");
                    foreach (char s in turingMachine.TapeAlphabet.FullTapeAlphabet)
                    {
                        Transition proxyTransition = new Transition(proxyState, t.InstructionState,
                        new char[] { s }, new char[] { s }, new Transition.Direction[] { Transition.Direction.left }, "");
                        insertionList.Add(proxyTransition);
                    }
                    insertionList.Add(transitionToProxy);
                    
                    markedForRemoval.Add(t);
                }
            }
            turingMachine.Transitions.RemoveAll(x => markedForRemoval.Contains(x));
            turingMachine.Transitions.AddRange(insertionList);
        }

        public TuringMachine LeftAndRightStates(TuringMachine turingMachine)
        {
            if (turingMachine.StateSet.StartingState == null) throw new Exception("Startzustand nicht definiert");
            if (turingMachine.StateSet.AcceptingStates.Contains(turingMachine.StateSet.StartingState)) throw new Exception("Startzustand darf kein akzeptierender Zustand sein");
            TuringMachine result = new TuringMachine();
            result.StateSet.StartingState = new State(turingMachine.StateSet.StartingState.Name);
            result.StateSet.RightStates = new List<State>();
            result.StateSet.LeftStates = new List<State>();
            result.StateSet.AllStates = new List<State>();
            result.StateSet.AllStates.Add(result.StateSet.StartingState);
            result.Transitions = new List<Transition>();
            result.TapeAlphabet = turingMachine.TapeAlphabet;

            foreach (State s in turingMachine.StateSet.AllStates)
            {
                State sLeft = new State(s.Name + "L");
                State sRight = new State(s.Name + "R");

                if (!turingMachine.StateSet.StartingState.Equals(s))
                {
                    result.StateSet.LeftStates.Add(sLeft);
                    result.StateSet.RightStates.Add(sRight);
                }

                if (turingMachine.StateSet.AcceptingStates.Contains(s))
                {
                    result.StateSet.AcceptingStates.Add(sLeft);
                    result.StateSet.AcceptingStates.Add(sRight);
                }
            }

            foreach (Transition t in turingMachine.Transitions)
            {
                if (t.ConditionState != null && t.InstructionState != null)
                {
                    if (t.Directions[0] == Transition.Direction.right && !t.ConditionState.Equals(result.StateSet.StartingState))
                    {
                        State? instructionStateRight = result.StateSet.RightStates.Find(x => x.Name == t.InstructionState + "R");
                        State? conditionStateLeft = result.StateSet.LeftStates.Find(x => x.Name == t.ConditionState + "L");
                        State? conditionStateRight = result.StateSet.RightStates.Find(x => x.Name == t.ConditionState + "R");

                        result.Transitions.Add(new Transition(conditionStateLeft, instructionStateRight, t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.right }, ""));
                        result.Transitions.Add(new Transition(conditionStateRight, instructionStateRight, t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.right }, ""));
                    }
                    if (t.Directions[0] == Transition.Direction.left && !t.ConditionState.Equals(result.StateSet.StartingState))
                    {
                        State? instructionStateLeft = result.StateSet.LeftStates.Find(x => x.Name == t.InstructionState + "L");
                        State? conditionStateLeft = result.StateSet.LeftStates.Find(x => x.Name == t.ConditionState + "L");
                        State? conditionStateRight = result.StateSet.RightStates.Find(x => x.Name == t.ConditionState + "R");

                        result.Transitions.Add(new Transition(conditionStateLeft, instructionStateLeft, t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.left }, ""));
                        result.Transitions.Add(new Transition(conditionStateRight, instructionStateLeft, t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.left }, ""));
                    }
                    if (t.ConditionState != null && t.ConditionState.Equals(result.StateSet.StartingState))
                    {
                        if (t.Directions[0] == Transition.Direction.left)
                        {
                            State? instructionStateLeft = result.StateSet.LeftStates.Find(x => x.Name == t.InstructionState + "L");
                            result.Transitions.Add(new Transition(t.ConditionState, instructionStateLeft, t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.left }, ""));
                        }
                        if (t.Directions[0] == Transition.Direction.right)
                        {
                            State? instructionStateRight = result.StateSet.RightStates.Find(x => x.Name == t.InstructionState + "R");
                            result.Transitions.Add(new Transition(t.ConditionState, instructionStateRight, t.ConditionTapeSymbol, t.InstructionTapeSymbol, new Transition.Direction[] { Transition.Direction.right }, ""));
                        }
                    }
                }
            }
            result.StateSet.AllStates.AddRange(result.StateSet.LeftStates);
            result.StateSet.AllStates.AddRange(result.StateSet.RightStates);
            return result;
        }

        public void MergeAcceptingStates(TuringMachine turingMachine)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendJoin('_', turingMachine.StateSet.AcceptingStates);
            if (turingMachine.StateSet.AllStates.Find(x => x.Name == sb.ToString()) != null)
            {
                Random rand = new Random();
                sb.Append(rand.Next(1000));
            }
   
            State mergedAcceptingState = new State(sb.ToString());
            foreach (Transition t in turingMachine.Transitions)
            {
                if (t.InstructionState != null && turingMachine.StateSet.AcceptingStates.Contains(t.InstructionState))
                {
                    t.InstructionState = mergedAcceptingState;
                }
            }
            turingMachine.StateSet.AcceptingStates.Clear();
            turingMachine.StateSet.AllStates.Add(mergedAcceptingState);
            turingMachine.StateSet.AcceptingStates.Add(mergedAcceptingState);
        }
    }
}
