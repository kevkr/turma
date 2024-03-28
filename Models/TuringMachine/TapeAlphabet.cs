using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using turma.Models.TuringMachine;

namespace turma.Models.TuringMachine
{
    public class TapeAlphabet
    {
        /*
         TapeSymbols -> TapeAlphabet without Inputalphabet and Epsilon
         Epsilon -> Epsilon (space symbol)
         InputAlphabet -> InputAlphabet
         FullTapeAlphabet -> TapeSymbols + Epsilon + InputAlphabet
         */
        public List<char> TapeSymbols { get; set; }
        private List<char> fullTapeAlphabet;
        public readonly static char Epsilon = 'Ɛ';
        public List<char> InputAlphabet { get; set; }

        /*Konstruktor*/
        public TapeAlphabet(List<char> tapeSymbols, List<char> inputAlphabet)
        {
            InputAlphabet = inputAlphabet;
            TapeSymbols = tapeSymbols;
            fullTapeAlphabet = new List<char>();

        }

        public TapeAlphabet()
        {
            InputAlphabet = new List<char>();
            TapeSymbols = new List<char>();
            fullTapeAlphabet = new List<char>();
        }

        public List<char> FullTapeAlphabet
        {
            get
            {
                fullTapeAlphabet = new List<char>();
                fullTapeAlphabet.AddRange(InputAlphabet);
                fullTapeAlphabet.AddRange(TapeSymbols);
                fullTapeAlphabet.Add(Epsilon);
                return fullTapeAlphabet;
            }
        }

        public override bool Equals(object? obj)
        {
            TapeAlphabet? other = obj as TapeAlphabet;
            if (other == null) return false;

            return Enumerable.SequenceEqual(FullTapeAlphabet.OrderBy(c => c), other.FullTapeAlphabet.OrderBy(c => c));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FullTapeAlphabet);
        }
    }
}