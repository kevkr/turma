using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace turma.Models.TuringMachine
{
    public class Transition
    {

        public Transition() {
            ConditionTapeSymbol = new char[1];
            InstructionTapeSymbol = new char[1];
            Directions = new Direction[1];
            Description = "";
        }
        public Transition(State? conditionState, State? instructionState,
                          char[] conditionTapeSymbols, char[] instructionTapeSymbols,
                          Direction[] direction, String description)
        {
            ConditionState = conditionState;
            InstructionState = instructionState;
            ConditionTapeSymbol = conditionTapeSymbols;
            InstructionTapeSymbol = instructionTapeSymbols;
            Directions = direction;
            Description = description;
        }

        public enum Direction { left=-1, stay = 0, right=1 };

        public string Description
        {
            get; set;
        }

        public State? ConditionState
        {
            get; set;
        }

        public State? InstructionState
        {
            get; set;
        }

        public char[] ConditionTapeSymbol
        {
            get; set;
        }

        public char[] InstructionTapeSymbol
        {
            get; set;
        }

        public Direction[] Directions
        {
            get; set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            if (ConditionState != null)
            sb.AppendFormat("\n {0}", "conditionState -> " + ConditionState.Name);
            if(InstructionState != null)
            sb.AppendFormat("\n {0}", "instructionState -> " + InstructionState.Name);
            sb.AppendFormat("\n {0}", "conditionTapeSymbols -> {" + String.Join(",", ConditionTapeSymbol) + "}");
            sb.AppendFormat("\n {0}", "instructionTapeSymbols -> {" + String.Join(",", InstructionTapeSymbol) + "}");
            sb.AppendFormat("\n {0}", "directions -> {" + String.Join(",", Directions) + "}");
            sb.AppendFormat("\n {0}", "description -> " + Description);
            sb.Append("\n}");

            return sb.ToString();
        }

        public override bool Equals(object? obj)
        {
            Transition? other = obj as Transition;
            if (other == null) return false;
            return Description.Equals(other.Description) &&
                object.Equals(ConditionState, other.ConditionState) &&
                object.Equals(InstructionState, other.InstructionState) &&
                Enumerable.SequenceEqual(ConditionTapeSymbol, other.ConditionTapeSymbol) &&
                Enumerable.SequenceEqual(InstructionTapeSymbol, other.InstructionTapeSymbol) &&
                Enumerable.SequenceEqual(Directions, other.Directions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Description, ConditionState, InstructionState, ConditionTapeSymbol, InstructionTapeSymbol, Directions);
        }
    }
}
