using Xunit;
using turma.Models.TuringMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.TuringMachine.Tests
{
    public class TransitionTests
    {
        [Fact()]
        public void ToStringTest()
        {
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            char[] conditionTapeSymbols = { 'a', 'b', 'c' };
            char[] instructionTapeSymbols = { 'X', 'Y', 'Z' };
            Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };
            string description = "Test description";

            Transition transition = new Transition(q1, q2, conditionTapeSymbols, instructionTapeSymbols, testDirections, description);
            string test;

            test = transition.ToString();

            Assert.Equal("{\n conditionState -> q1\n instructionState -> q2\n conditionTapeSymbols -> {a,b,c}\n instructionTapeSymbols -> {X,Y,Z}\n directions -> {left,right,stay}\n description -> Test description\n}", test);
        }



        [Fact()]
        public void TestIfEqualsReturnsFalseWhenParameterIsNull()
        {
            Transition transition = new Transition();
            bool test;

            test = transition.Equals(null);

            Assert.False(test);
        }

        [Fact()]
        public void TestIfEqualsReturnsFalseWhenParameterIsDifferentTransition()
        {
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            char[] conditionTapeSymbols = { 'a', 'b', 'c' };
            char[] instructionTapeSymbols = { 'X', 'Y', 'Z' };
            Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };
            string description = "Test description";

            Transition transition1 = new Transition(q1, q2, conditionTapeSymbols, instructionTapeSymbols, testDirections, description);
            Transition transition2 = new Transition(q1, q3, conditionTapeSymbols, instructionTapeSymbols, testDirections, description);

            bool test;

            test = transition1.Equals(transition2);

            Assert.False(test);
        }


        [Fact()]
        public void TestIfEqualsReturnsTrueWhenParameterIsTheSameTransition()
        {
            State q1 = new State("q1");
            State q2 = new State("q2");
            char[] conditionTapeSymbols = { 'a', 'b', 'c' };
            char[] instructionTapeSymbols = { 'X', 'Y', 'Z' };
            Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };
            string description = "Test description";

            Transition transition1 = new Transition(q1, q2, conditionTapeSymbols, instructionTapeSymbols, testDirections, description);
            Transition transition2 = new Transition(q1, q2, conditionTapeSymbols, instructionTapeSymbols, testDirections, description);

            bool test;

            test = transition1.Equals(transition2);

            Assert.True(test);
        }


        [Fact()]
        public void TestIfHashCodeReturnsInt()
        {
            Transition transition = new Transition();

            Assert.IsType<int>(transition.GetHashCode());
        }
    }
}