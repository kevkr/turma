using Avalonia.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;

namespace turma.Models.TuringMachine
{
    public class TuringMachine : ITuringMachine
    {

        /*Konstruktor ohne Parameter*/
        public TuringMachine()
        {
            TapeAlphabet = new TapeAlphabet();
            Transitions = new List<Transition>();
            StateSet = new StateSet();
            TapeAlphabet = new TapeAlphabet();
            TapeCount = 1; //Default Tape Count soll 1 sein
        }

        /*Konstruktor mit Parametern*/
        public TuringMachine(int tapeCount, List<Transition> transitions, StateSet stateSet ,TapeAlphabet tapeAlphabet)
        {
            TapeCount = tapeCount;
            Transitions = transitions;
            StateSet = stateSet;
            TapeAlphabet = tapeAlphabet;
        }
        public int TapeCount
        {
            get; set;
        }

        public List<Transition> Transitions
        {
            get; set;
        }

        public TapeAlphabet TapeAlphabet
        {
            get; set;
        }

        public StateSet StateSet
        {
            get; set;
        }
        public void TransformToEquivalentMachine()
        {

        }

        public override bool Equals(object? obj)
        {
            TuringMachine? other = obj as TuringMachine;
            if (other == null) return false; 

            return TapeCount.Equals(other.TapeCount) && TapeAlphabet.Equals(other.TapeAlphabet) && StateSet.Equals(other.StateSet) && Enumerable.SequenceEqual(Transitions, other.Transitions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TapeCount, Transitions, TapeAlphabet, StateSet);
        }

        public TuringMachine Clone()
        {
            return (TuringMachine) this.MemberwiseClone();
        }
    }
}
