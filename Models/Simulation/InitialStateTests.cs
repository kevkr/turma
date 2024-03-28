using System;
using Xunit;
using turma.Models.Simulation;
using turma.Models.TuringMachine;
using System.Collections.Generic;

namespace turmaTest.Models.Simulation
{
    public class InitialStateTests
    {
        char[] testConditionTapeSymbols = { 'a', 'B', 'u' };
        char[] testInstructionTapeSymbols = { 'X', 'Y', 'Z' };
        Transition.Direction[] testDirections = { Transition.Direction.left, Transition.Direction.right, Transition.Direction.stay };

        [Fact]
        public void InitialState_IsNotNull()
        {
            //Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            InitialState initialState = new InitialState(simulationContext);

            //Assert
            Assert.NotNull(initialState);// check if endstate is not null 

        }

        [Fact]
        public void IsSimulationActive_StateUnderTest_ReturningBool()
        {

            // Arrange

            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);

            // Act
            var result = initialState.IsSimulationActive();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsWordAccepted_StateUnderTest_ReturningBool()
        {
            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            InitialState initialState = new InitialState(simulationContext);

            // Act
            bool? result = initialState.IsWordAccepted();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void PauseSimulation_StateUnderTest_NoExceptions()  
        {

            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);

            // Act
            var exception = Record.Exception(() => initialState.PauseSimulation()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void StartSimulation_CurrentStateUnderTest_IsStartingState()
        {
            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);
            
            List<State> allStates = new List<State>();
            List<State> acceptingStates = new List<State>();
            List<Transition> transitions = new List<Transition>();
            
            State q1 = new State("q1");

            simulationContext.TuringMachine.StateSet.StartingState = q1;
            allStates.Add(q1);
    
            turingMachine.StateSet = new StateSet(allStates, q1, acceptingStates);
           
            // Act
            
            initialState.StartSimulation();

            // Assert
            Assert.Equal(q1, simulationContext.CurrentState);
           
        }

        [Fact]
        public void StartSimulation_SimulationStateUnderTest_IsRunningState()
        {
            //Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);

            List<State> allStates = new List<State>();
            List<State> acceptingStates = new List<State>();
          
            State q1 = new State("q1");
   
            allStates.Add(q1);          

            turingMachine.StateSet = new StateSet(allStates, q1, acceptingStates);
           
            // Act
            initialState.StartSimulation();

            // Assert

            Assert.IsType<RunningState>(simulationContext.SimulationState);

        }

            [Fact]
        public void StepBack_StateUnderTest_NoExceptions()  // test functions are not throwing exceptions
        {
            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);

            // Act
            var exception = Record.Exception(() => initialState.StepBack()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void StepForward_StateUnderTest_NoExceptions()  // test functions are not throwing exceptions
        {
            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);

            // Act
            var exception = Record.Exception(() => initialState.StepForward()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void StopSimulation_StateUnderTest_NoExceptions()  // test functions are not throwing exceptions
        {
            // Arrange
            TuringMachine turingMachine = new TuringMachine();
            SimulationContext simulationContext = new SimulationContext(turingMachine);

            var initialState = new InitialState(simulationContext);

            // Act
            var exception = Record.Exception(() => initialState.StopSimulation()); // test functions are not throwing exceptions

            // Assert
            Assert.Null(exception);
        }
    }
}
