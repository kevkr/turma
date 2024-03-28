using System;
using Xunit;
using turma.Models.Simulation;
using turma.Models.TuringMachine;
using System.Collections.Generic;

namespace turmaTest.Models.Simulation
{
    public class EndStateTests
    {
       
        char[] testConditionTapeSymbols = { 'a', 'B', 'u' };
        char[] testInstructionTapeSymbols = { 'X', 'Y', 'Z' };
        Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };


        [Fact]
        public void EndStateTest_IsNotNull()
        {
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            EndState end = new EndState(simulationContext);
              
            // check if endstate is not null 
            Assert.NotNull(end);

        }
        [Fact]
        public void IsSimulationActive_StateUnderTest_ReturningBool()
        {
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);
            // Arrange
            EndState end = new EndState(simulationContext);

            // Act
            bool result = end.IsSimulationActive(); // test if simulation is currently active 

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void IsWordAccepted_StateUnderTest_ReturningBool()
        {
            // Arrange
           
            TuringMachine turingMachine = new TuringMachine(); 
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            EndState endState = new EndState(simulationContext);

            List<State> allStates = new List<State>();
            allStates = simulationContext.TuringMachine.StateSet.AllStates;
            List<State> acceptingStates = new List<State>();
            acceptingStates = simulationContext.TuringMachine.StateSet.AcceptingStates;
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            allStates.Add(q1);
            allStates.Add(q2);
            allStates.Add(q3);
            acceptingStates.Add(q3); 
         
            // Act
            simulationContext.CurrentState = q3;
            bool? result = endState.IsWordAccepted();
            
            Assert.True(result);
        

        }

        [Fact]
        public void PauseSimulation_StateUnderTest_NoExceptions()
        {
            // Arrange

            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            EndState end = new EndState(simulationContext);

            // Act
            var exception = Record.Exception(() => end.PauseSimulation()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void StartSimulation_StateUnderTest_NoExceptions()
        {
            // Arrange


            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            EndState end = new EndState(simulationContext);

            // Act
            var exception = Record.Exception(() => end.StartSimulation()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void StepBack_SimulationStateUnderTest_IsPausedState()
        {
            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var endState = new EndState(simulationContext);
          
            List<State> allStates = new List<State>();
            List<State> acceptingStates = new List<State>();
            List<Transition> transitions = new List<Transition>();
            TransitionHistory transitionHistory = new TransitionHistory();
            State q1 = new State("q1");
            State q2 = new State("q2");
            State q3 = new State("q3");
            char[] duplicateConditionTapeSymbols = { 'a', 'B', 'u' };
            Transition t1 = new Transition(q1, q3, testConditionTapeSymbols, testInstructionTapeSymbols, testDirections, "Test Transition");
          
            allStates.Add(q1);
            allStates.Add(q2);
            allStates.Add(q3);
            transitions.Add(t1);
        
            turingMachine.StateSet = new StateSet(allStates, q1, acceptingStates);
           
            simulationContext.TransitionHistory.Push(t1);
           

            // Act
           endState.StepBack();

            // Assert  
            Assert.IsType<PausedState>(simulationContext.SimulationState);
        }

        [Fact]
        public void StepForward_StateUnderTest_NoExceptions()
        {

            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            EndState end = new EndState(simulationContext);

            // Act
            var exception = Record.Exception(() => end.StepForward()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);

        }

        [Fact]
        public void StopSimulation_StateUnderTest_NoExceptions()
        {
            // Arrange

            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            EndState end = new EndState(simulationContext);

            // Act
            var exception = Record.Exception(() => end.StopSimulation()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }
    }
}
