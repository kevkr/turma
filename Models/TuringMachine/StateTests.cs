using Xunit;
using turma.Models.TuringMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.TuringMachine.Tests
{
    public class StateTests
    {
        [Fact()]
        public void TestIfStateConstructerSetsParameterStringAsNameOfState()
        {
            State state = new State("TestState");

            Assert.Equal("TestState", state.Name);
        }

        [Fact()]
        public void TestIfToStringMethodReturnsNameOfTheState()
        {
            State state = new State("TestState");
            string name;

            name = state.ToString();

            Assert.Equal("TestState", name);
        }

        [Fact()]
        public void TestIfEqualsReturnsFalseWhenParameterIsNull()
        {
            State state = new State("TestState");
            bool test;

            test = state.Equals(null);

            Assert.False(test);
        }

        [Fact()]
        public void TestIfEqualsReturnsFalseWhenParameterNameIsNotNameOfState()
        {
            State state1 = new State("TestState1");
            State state2 = new State("TestState2");
            bool test;

            test = state1.Equals(state2);

            Assert.False(test);
        }

        [Fact()]
        public void TestIfEqualsReturnsTrueWhenParameterNameIsNameOfState()
        {
            State state1 = new State("TestState1");
            State state2 = new State("TestState1");
            bool test;

            test = state1.Equals(state2);

            Assert.True(test);
        }

        [Fact()]
        public void TestIfGetHashCodeReturnsInt()
        {
            State state = new State("TestState");
            int hash;

            hash = state.GetHashCode();

            Assert.IsType<int>(hash);
        }

        [Fact()]
        public void TestIfGetHashCodeReturnsTheSameHashCodeForTheSameState()
        {
            State state = new State("TestState");
            int hash1, hash2;

            hash1 = state.GetHashCode();
            hash2 = state.GetHashCode();

            Assert.Equal(hash1, hash2);
        }

        [Fact()]
        public void TestIfGetHashCodeReturnsDifferentHashCodesForDifferentState()
        {
            State state1 = new State("TestState1");
            State state2 = new State("TestState2");
            int hash1, hash2;

            hash1 = state1.GetHashCode();
            hash2 = state2.GetHashCode();

            Assert.NotEqual(hash1, hash2);
        }
    }
}