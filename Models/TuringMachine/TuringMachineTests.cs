using Xunit;
using turma.Models.TuringMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.TuringMachine.Tests
{
    public class TuringMachineTests
    {

        [Fact()]
        public void TestIfEqualsReturnsFalseWhenParameterIsNull()
        {
            TuringMachine turingMachine = new TuringMachine();
            bool test;

            test = turingMachine.Equals(null);

            Assert.False(test);
        }

        [Fact()]
        public void TestIfEqualsReturnsFalseWhenParameterIsAnotherTuringMachine()
        {
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            State q4 = new State("q4");
            State q5 = new State("q5");
            List<State> states1 = new List<State>();
            states1.Add(q1);
            states1.Add(q2);
            states1.Add(q3);
            states1.Add(q4);
            states1.Add(q5);

            List<State> states2 = new List<State>();
            states2.Add(q1);
            states2.Add(q4);
            states2.Add(q5);

            char[] testConditionTapeSymbols = { 'a', 'B', 'u' };
            char[] testInstructionTapeSymbols = { 'X', 'Y', 'Z' };
            List<char> testTapeAlphabet = new List<char> { 'a', 'B', 'u', 'X', 'Y', 'Z' };
            List<char> testInputTapeAlphabet = new List<char> { 'a', 'u', };

            Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };
            List<Transition> transitions = new List<Transition>();
            Transition t1 = new Transition(q1, q2, testConditionTapeSymbols, testInstructionTapeSymbols, testDirections, "Test Transition");

            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(q4);
            acceptingStates.Add(q5);

            StateSet stateSet1 = new StateSet(states1, q1, acceptingStates);
            StateSet stateSet2 = new StateSet(states2, q1, acceptingStates);

            TapeAlphabet tapeAlphabet = new TapeAlphabet();

            TuringMachine turingMachine1 = new TuringMachine(1, transitions, stateSet1, tapeAlphabet);
            TuringMachine turingMachine2 = new TuringMachine(1, transitions, stateSet2, tapeAlphabet);
            bool test;

            test = turingMachine1.Equals(turingMachine2);

            Assert.False(test);
        }

        [Fact()]
        public void TestIfEqualsReturnsTrueWhenParameterIsTheSameTuringMachine()
        {   
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            State q4 = new State("q4");
            State q5 = new State("q5");
            List<State> states = new List<State>();
            states.Add(q1);
            states.Add(q2);
            states.Add(q3);
            states.Add(q4);
            states.Add(q5);

            char[] testConditionTapeSymbols = { 'a', 'B', 'u' };
            char[] testInstructionTapeSymbols = { 'X', 'Y', 'Z' };
            List<char> testTapeAlphabet = new List<char> { 'a', 'B', 'u', 'X', 'Y', 'Z' };
            List<char> testInputTapeAlphabet = new List<char> { 'a', 'u', };

            Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };
            List<Transition> transitions = new List<Transition>();
            Transition t1 = new Transition(q1, q2, testConditionTapeSymbols, testInstructionTapeSymbols, testDirections, "Test Transition");

            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(q4);
            acceptingStates.Add(q5);

            StateSet stateSet = new StateSet(states, q1, acceptingStates);

            TapeAlphabet tapeAlphabet = new TapeAlphabet();
            
            TuringMachine turingMachine1 = new TuringMachine(1, transitions, stateSet, tapeAlphabet);
            TuringMachine turingMachine2 = new TuringMachine(1, transitions, stateSet, tapeAlphabet);
            bool test;

            test = turingMachine1.Equals(turingMachine2);

            Assert.True(test);
        }


        [Fact()]
        public void TestIfHashCodeReturnsInt()
        {
            TuringMachine turingMachine = new TuringMachine();

            Assert.IsType<int>(turingMachine.GetHashCode());
        }


        [Fact()]
        public void TestIfClonedTuringMachineHasTheSameValuesAsTheOriginalOne()
        {
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            State q4 = new State("q4");
            State q5 = new State("q5");
            List<State> states = new List<State>();
            states.Add(q1);
            states.Add(q2);
            states.Add(q3);
            states.Add(q4);
            states.Add(q5);

            char[] testConditionTapeSymbols = { 'a', 'B', 'u' };
            char[] testInstructionTapeSymbols = { 'X', 'Y', 'Z' };
            List<char> testTapeAlphabet = new List<char> { 'a', 'B', 'u', 'X', 'Y', 'Z' };
            List<char> testInputTapeAlphabet = new List<char> { 'a', 'u', };

            Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };
            List<Transition> transitions = new List<Transition>();
            Transition t1 = new Transition(q1, q2, testConditionTapeSymbols, testInstructionTapeSymbols, testDirections, "Test Transition");

            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(q4);
            acceptingStates.Add(q5);

            StateSet stateSet = new StateSet(states, q1, acceptingStates);

            TapeAlphabet tapeAlphabet = new TapeAlphabet();

            TuringMachine turingMachine1 = new TuringMachine(1, transitions, stateSet, tapeAlphabet);
            TuringMachine turingMachine2 = new TuringMachine();

            turingMachine2 = turingMachine1.Clone();

            Assert.Equal(turingMachine1, turingMachine2);
        }
    }
}