using Xunit;
using turma.Models.TuringMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.TuringMachine.Tests
{
    public class TapeAlphabetTests
    {
        //Constructor with parameters
        [Fact()]
        public void TestTapeAlphabetWithParametersIfTapeAlphabetsTapeSymbolsEqualsTapeSymbolsFromParameters()
        {
            List<char> tapeSymbols = new List<char>();
            tapeSymbols.Add('A');
            tapeSymbols.Add('B');
            tapeSymbols.Add('C');

            List<char> inputAlphabet = new List<char>();
            inputAlphabet.Add('X');
            inputAlphabet.Add('Y');
            inputAlphabet.Add('Z');

            TapeAlphabet tapeAlphabet = new TapeAlphabet(tapeSymbols, inputAlphabet);

            Assert.Equal(tapeSymbols, tapeAlphabet.TapeSymbols);
        }

        [Fact()]
        public void TestTapeAlphabetWithParametersIfTapeAlphabetsInputAlphabetEqualsInputAlphabetFromParameters()
        {
            List<char> tapeSymbols = new List<char>();
            tapeSymbols.Add('A');
            tapeSymbols.Add('B');
            tapeSymbols.Add('C');

            List<char> inputAlphabet = new List<char>();
            inputAlphabet.Add('X');
            inputAlphabet.Add('Y');
            inputAlphabet.Add('Z');

            TapeAlphabet tapeAlphabet = new TapeAlphabet(tapeSymbols, inputAlphabet);

            Assert.Equal(inputAlphabet, tapeAlphabet.InputAlphabet);
        }


        //Constructor without parameter
        [Fact()]
        public void TestTapeAlphabetConstructorIfInputAlphabetTypeIsListOfChar()
        {
            TapeAlphabet tapeAlphabet = new TapeAlphabet();

            Assert.IsType<List<char>>(tapeAlphabet.InputAlphabet);
        }

        [Fact()]
        public void TestTapeAlphabetConstructorIfTapeSymbolsTypeIsListOfChar()
        {
            TapeAlphabet tapeAlphabet = new TapeAlphabet();

            Assert.IsType<List<char>>(tapeAlphabet.TapeSymbols);
        }

        //Equals()
        [Fact()]
        public void TestEqualsIfNullAsParameterReturnsFalse()
        {
            TapeAlphabet tapeAlphabet = new TapeAlphabet();
            bool test;

            test = tapeAlphabet.Equals(null);

            Assert.False(test);
        }

        [Fact()]
        public void TestEqualsIfFalseGetsReturnedWhenParameterIsAnotherTapeAlphabet()
        {
            List<char> tapeSymbols = new List<char>();
            tapeSymbols.Add('A');
            tapeSymbols.Add('B');
            tapeSymbols.Add('C');

            List<char> inputAlphabet1 = new List<char>();
            inputAlphabet1.Add('X');
            inputAlphabet1.Add('Y');
            inputAlphabet1.Add('Z');

            List<char> inputAlphabet2 = new List<char>();
            inputAlphabet2.Add('X');
            inputAlphabet2.Add('Y');


            TapeAlphabet tapeAlphabet1 = new TapeAlphabet(tapeSymbols, inputAlphabet1);
            TapeAlphabet tapeAlphabet2 = new TapeAlphabet(tapeSymbols, inputAlphabet2);
            bool test;

            test = tapeAlphabet1.Equals(tapeAlphabet2);

            Assert.False(test);
        }

        [Fact()]
        public void TestEqualsIfTrueGetsReturnedWhenParameterIsTheSameTapeAlphabet()
        {
            List<char> tapeSymbols = new List<char>();
            tapeSymbols.Add('A');
            tapeSymbols.Add('B');
            tapeSymbols.Add('C');

            List<char> inputAlphabet = new List<char>();
            inputAlphabet.Add('X');
            inputAlphabet.Add('Y');
            inputAlphabet.Add('Z');

            TapeAlphabet tapeAlphabet1 = new TapeAlphabet(tapeSymbols, inputAlphabet);
            TapeAlphabet tapeAlphabet2 = new TapeAlphabet(tapeSymbols, inputAlphabet);
            bool test;

            test = tapeAlphabet1.Equals(tapeAlphabet2);

            Assert.True(test);
        }


        //GetHashCode()
        [Fact()]
        public void TestIfGetHashCodeReturnsInt()
        {
            TapeAlphabet tapeAlphabet = new TapeAlphabet();
            int hash;

            hash = tapeAlphabet.GetHashCode();

            Assert.IsType<int>(hash);
        }

    }
}