using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using turma.Models.TuringMachine;
using System.Diagnostics;

namespace turma.ViewModels
{
    public class Validator
    {
        public bool validate(TuringMachine turingMachine)
        {
            if (turingMachine == null)
            {
                throw new Exception("Die Turing Machine existiert nicht.");
            }
            // Check if Starting State is set
            else if (turingMachine.StateSet.StartingState == null)
            {
                throw new Exception("Der Startzustand ist nicht definiert.");
            }
            // Check if all Accepting States are part of the Set of all States.
            else if (!turingMachine.StateSet.AcceptingStates.TrueForAll(state => turingMachine.StateSet.AllStates.Contains(state)))
            {
                throw new Exception("Nicht alle Akzept. Zustände sind Teil der Zustandsmenge.");
            }
            // Check if there are duplicate States in the Set of all States.
            else if (turingMachine.StateSet.AllStates.Distinct().Count() != turingMachine.StateSet.AllStates.Count())
            {
                throw new Exception("Es gibt Duplikate in der Zustandsmenge.");
            }

            // Check if for all transitions the instruction and condition state exist and are contained in the set of all states. 
            else if (!turingMachine.Transitions.TrueForAll(transition => transition.InstructionState != null && turingMachine.StateSet.AllStates.Contains(transition.InstructionState) && transition.ConditionState != null && turingMachine.StateSet.AllStates.Contains(transition.ConditionState)))
            {
                throw new Exception("In einem Übergang werden Zustände verwendet, die nicht in der Zustandsmenge enthalten sind.");
            }
            // Check if for all transitions the instruction tape symbols are part of the TapeAlphabet
            else if (!turingMachine.Transitions.TrueForAll(transition => Array.TrueForAll(transition.InstructionTapeSymbol, tapesymbol => turingMachine.TapeAlphabet.FullTapeAlphabet.Contains(tapesymbol))))
            {
                throw new Exception("In einen Übergang werden Zeichen geschrieben, die nicht im Bandalphabet enthalten sind.");
            }
            // Check if for all transitions the condition tape symbols are part of the TapeAlphabet
            else if (!turingMachine.Transitions.TrueForAll(transition => Array.TrueForAll(transition.ConditionTapeSymbol, tapesymbol => turingMachine.TapeAlphabet.FullTapeAlphabet.Contains(tapesymbol))))
            {
                throw new Exception("In einen Übergang werden Zeichen gelesen, die nicht im Bandalphabet enthalten sind.");
            }
            // Check if for every transition the combination of condition state and condition tape symbol is unique
            else if (!turingMachine.Transitions.TrueForAll(transition => turingMachine.Transitions.Where(t => t.ConditionState.Equals(transition.ConditionState) && t.ConditionTapeSymbol.SequenceEqual(transition.ConditionTapeSymbol)).Count() == 1)) {
                throw new Exception("Es gibt mehrere Übergänge mit gleichem Zustand und zu lesenden Zeichen.");
            }

            // Check if there are duplicate TapeSymbols in the Set of all TapeSymbols.
            else if (turingMachine.TapeAlphabet.TapeSymbols.Distinct().Count() != turingMachine.TapeAlphabet.TapeSymbols.Count())
            {
                throw new Exception("Es gibt Duplikate in der Menge aller Bandsymbole.");
            }
            // Check if there are duplicate InputTapeSymbols in the Set of all InputTapeSymbols.
            else if (turingMachine.TapeAlphabet.InputAlphabet.Distinct().Count() != turingMachine.TapeAlphabet.InputAlphabet.Count())
            {
                throw new Exception("Es gibt Duplikate im Eingabealphabet.");
            }

            // Check if no InputTapeSymbols are in the Set of TapeSymbols.
            else if (!turingMachine.TapeAlphabet.InputAlphabet.TrueForAll(inputTapeSymbol => !turingMachine.TapeAlphabet.TapeSymbols.Contains(inputTapeSymbol)))
            {
                throw new Exception("Zeichen des Eingabealphabets sind auch im Bandalphabet");
            }

            return true;
        }
    }
}
