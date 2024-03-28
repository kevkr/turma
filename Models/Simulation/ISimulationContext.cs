using System;
using System.Collections.Generic;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
    public interface ISimulationContext
    {
        public void writeInputWordToTape(string inputWord);
        public void startSimulation();
        public void stopSimulation();
        public bool isSimulationActive();
        public void pauseSimulation();
        public void stepBack();
        public void stepForward();
        bool? isWordAccepted();
        public ITuringMachine TuringMachine { get; set; }
        public State? CurrentState { get; set; }
        public Transition? CurrentTransition { get; set; }
        public TransitionHistory TransitionHistory { get; }
        public double AnimationSpeed { get; set; }
        public ISimulationState? SimulationState { get; set; }
        public Tape[] Tapes { get; set; }
        public ReadWriteHead[] ReadWriteHeads { get; set; }

    }
}
