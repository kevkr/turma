using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace turma.Models.TuringMachine
{
    public class StateSet
    {
        public StateSet()
        {
            AcceptingStates = new List<State>();
            AllStates = new List<State>();
            StartingState = null;
            LeftStates = new List<State>();
            RightStates = new List<State>();
        }

        /*Konstruktor*/
        public StateSet(List<State> states, State startingState, List<State> acceptingStates)
        {
            AllStates = states;
            StartingState = startingState;
            AcceptingStates = acceptingStates;
            LeftStates = new List<State>();
            RightStates = new List<State>();
        }
        public State? StartingState
        {
            get; set;
        }

        public List<State> AcceptingStates
        {
            get; set;
        }

        public List<State> AllStates
        {
            get; set;
        }

        public List<State>? LeftStates
        {
            get; set;
        }

        public List<State>? RightStates
        {
            get; set;
        }

        public override bool Equals(object? obj)
        {
            StateSet? other = obj as StateSet;
            if (other == null) return false;
            return Enumerable.SequenceEqual(AllStates, other.AllStates) && 
                   Enumerable.SequenceEqual(AcceptingStates, other.AcceptingStates) && 
                   object.Equals(StartingState, other.StartingState) && 
                   Enumerable.SequenceEqual(LeftStates, other.LeftStates) && 
                   Enumerable.SequenceEqual(RightStates, other.RightStates);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartingState, AcceptingStates, AllStates, LeftStates, RightStates);
        }
    }
}

