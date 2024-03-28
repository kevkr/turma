using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{

    public class SimulationContext : ISimulationContext
    {
        private State? _currentState;
        private Transition? _currentTransition;
        private ISimulationState? _currentSimulationState;
        private double _animationSpeed;

        public Tape[] Tapes { get; set; }

        public SimulationContext(ITuringMachine turingMachine)
        {
            this.TuringMachine = turingMachine;
            this.CurrentState = turingMachine.StateSet.StartingState;
            this.Tapes = new Tape[turingMachine.TapeCount];
            this.ReadWriteHeads = new ReadWriteHead[turingMachine.TapeCount];
            for (int i = 0; i < turingMachine.TapeCount; i++)
            {
                Tapes[i] = new Tape();
                ReadWriteHeads[i] = new ReadWriteHead(Tapes[i]);
            }
            this.AnimationSpeed = 1;
            this.TransitionHistory = new TransitionHistory();
            SimulationState = new InitialState(this);
        }

        public void writeInputWordToTape(string? inputWord)
        {
            if (inputWord != null)
            {
                int x = Tapes[0].TapeCapacity / 2;

                if (x < inputWord.Length)
                {
                    for (int i = 0; i - x < inputWord.Length; i += Tapes[0].ExtendTapeCapacity((int)Transition.Direction.right)) ;
                }

                for (int i = 0; i < inputWord.Length; i++)
                {
                    Tapes[0].Cells[ReadWriteHeads[0].CellIndex + i].TapeSymbol = inputWord[i];
                }
            }
            ReadWriteHeads[0].redrawTape();
            SimulationState = new InitialState(this);
        }

        public delegate void CurrentStateChangedHandler(State? s);
        public event CurrentStateChangedHandler? CurrentStateChanged;
        public delegate void CurrentTransitionChangedHandler(Transition? t);
        public event CurrentTransitionChangedHandler? CurrentTransitionChanged;
        public delegate void CurrentSimulationStateChangedHandler(ISimulationState? ss);
        public event CurrentSimulationStateChangedHandler? CurrentSimulationStateChanged;

        public void startSimulation()
        {
            if (SimulationState != null)
                SimulationState.StartSimulation();
        }
        public void stopSimulation()
        {
            if (SimulationState != null)
                SimulationState.StopSimulation();
        }
        public bool isSimulationActive()
        {
            if (SimulationState != null)
                return SimulationState.IsSimulationActive();
            return false;
        }
        public void pauseSimulation()
        {
            if (SimulationState != null)
                SimulationState.PauseSimulation();
        }
        public void stepBack()
        {
            if (SimulationState != null)
                SimulationState.StepBack();
        }
        public void stepForward()
        {
            if (SimulationState != null)
                SimulationState.StepForward();
        }
        public bool? isWordAccepted()
        {
            if (SimulationState != null)
                return SimulationState.IsWordAccepted();
            return null;
        }

        public ITuringMachine TuringMachine
        {
            get; set;
        }

        public TransitionHistory TransitionHistory
        {
            get;
        }
        public State? CurrentState { 
            get { return _currentState; } 
            set { _currentState = value; if (CurrentStateChanged != null) { CurrentStateChanged(value); } } 
        }
        public ReadWriteHead[] ReadWriteHeads
        {
            get; set;
        }
        public Transition? CurrentTransition
        {
            get { return _currentTransition; }
            set { _currentTransition = value; if (CurrentTransitionChanged != null) { CurrentTransitionChanged(value); } }
        }
        public double AnimationSpeed
        {
            get { return _animationSpeed; }
            set { _animationSpeed = value; 
                if (SimulationState != null) 
                    SimulationState.changeAnimationSpeed(value); 
            }
        }
        public ISimulationState? SimulationState
        {
            get { return _currentSimulationState; }
            set { _currentSimulationState = value; if (CurrentSimulationStateChanged != null) { CurrentSimulationStateChanged(value); } }
        }
    }

}
