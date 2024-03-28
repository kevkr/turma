using Xunit;
using turma.Models.TuringMachine;
using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.TuringMachine.Tests
{
    public class StateSetTests
    {
        //StateSet constructor tests without parameter
        [Fact()]
        public void TestStateSetConstructorWithoutParametersIfAcceptingStateIsListOfStates()
        {
            StateSet stateSet = new StateSet();

            Assert.IsType<List<State>>(stateSet.AcceptingStates);
        }

        [Fact()]
        public void TestStateSetConstructorWithoutParametersIfAllStatesIsListOfStates()
        {
            StateSet stateSet = new StateSet();

            Assert.IsType<List<State>>(stateSet.AllStates);
        }

        [Fact()]
        public void TestStateSetConstructorWithoutParametersIfStartingStateIsNull()
        {
            StateSet stateSet = new StateSet();

            Assert.Null(stateSet.StartingState);
        }

        [Fact()]
        public void TestStateSetConstructorWithoutParametersIfLeftStatesIsListOfStates()
        {
            StateSet stateSet = new StateSet();

            Assert.IsType<List<State>>(stateSet.LeftStates);
        }

        [Fact()]
        public void TestStateSetConstructorWithoutParametersIfRightStatesIsListOfStates()
        {
            StateSet stateSet = new StateSet();

            Assert.IsType<List<State>>(stateSet.RightStates);
        }


        //StateSet Constructor tests with parameters
        [Fact()]
        public void TestStateSetConstructorWithParametersIfStateSetAcceptingStateIsAcceptingStatesFromParameters()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);

            StateSet stateSet = new StateSet(states, startingState, acceptingStates);

            Assert.Equal(acceptingStates, stateSet.AcceptingStates);
        }

        [Fact()]
        public void TestStateSetConstructorWithParametersIfStateSetAllStatesIsStatesFromParameters()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);

            StateSet stateSet = new StateSet(states, startingState, acceptingStates);

            Assert.Equal(states, stateSet.AllStates);
        }

        [Fact()]
        public void TestStateSetConstructorWithParametersIfStateSetStartingStateIsStartingStateFromParameters()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);

            StateSet stateSet = new StateSet(states, startingState, acceptingStates);

            Assert.Equal(startingState, stateSet.StartingState);
        }

        [Fact()]
        public void TestStateSetConstructorWithParametersIfLeftStatesIsListOfStates()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);

            StateSet stateSet = new StateSet(states, startingState, acceptingStates);

            Assert.IsType<List<State>>(stateSet.LeftStates);
        }

        [Fact()]
        public void TestStateSetConstructorWithParametersIfRightStatesIsListOfStates()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);

            StateSet stateSet = new StateSet(states, startingState, acceptingStates);

            Assert.IsType<List<State>>(stateSet.RightStates);
        }


        [Fact()]
        public void TestEqualsIfFalseGetsReturnedWhenParameterIsNull()
        {
            StateSet stateSet = new StateSet();
            bool test;

            test = stateSet.Equals(null);

            Assert.False(test);
        }

        [Fact()]
        public void TestEqualsIfFalseGetsReturnedWhenParameterIsAnotherStateSet()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states1 = new List<State>();
            states1.Add(state1);
            states1.Add(state2);
            states1.Add(state3);
            List<State> states2 = new List<State>();
            states2.Add(state1);
            states2.Add(state2);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);
            StateSet stateSet1 = new StateSet(states1, startingState, acceptingStates);
            StateSet stateSet2 = new StateSet(states2, startingState, acceptingStates);
            bool test;

            test = stateSet1.Equals(stateSet2);

            Assert.False(test);
        }

        [Fact()]
        public void TestEqualsIfTrueGetsReturnedWhenParameterIsTheSameStateSet()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);
            StateSet stateSet1 = new StateSet(states, startingState, acceptingStates);
            StateSet stateSet2 = new StateSet(states, startingState, acceptingStates);
            bool test;

            test = stateSet1.Equals(stateSet2);

            Assert.True(test);
        }

        [Fact()]
        public void TestIfGetHashCodeReturnsInt()
        {
            StateSet stateSet = new StateSet();
            int hash;

            hash = stateSet.GetHashCode();

            Assert.IsType<int>(hash);
        }

        [Fact()]
        public void TestIfGetHashCodeReturnsTheSameHashCodeForTheSameStateSet()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);
            StateSet stateSet = new StateSet(states, startingState, acceptingStates);
            int hash1, hash2;

            hash1 = stateSet.GetHashCode();
            hash2 = stateSet.GetHashCode();

            Assert.Equal(hash1, hash2);
        }

        [Fact()]
        public void TestIfGetHashCodeReturnsDifferentHashCodesForDifferentStateSets()
        {
            State state1 = new State("test1");
            State state2 = new State("test2");
            State state3 = new State("test3");
            State startingState = state1;
            List<State> states = new List<State>();
            states.Add(state1);
            states.Add(state2);
            states.Add(state3);
            List<State> acceptingStates = new List<State>();
            acceptingStates.Add(state2);
            StateSet stateSet1 = new StateSet(states, startingState, acceptingStates);
            StateSet stateSet2 = new StateSet(states, startingState, acceptingStates);
            int hash1, hash2;

            hash1 = stateSet1.GetHashCode();
            hash2 = stateSet2.GetHashCode();

            Assert.NotEqual(hash1, hash2);
        }
    }
}