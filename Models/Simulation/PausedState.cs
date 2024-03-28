using System;
using System.Collections.Generic;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
    public class PausedState : TransitionExecutor, ISimulationState
    {
        private ISimulationContext context;

        public PausedState(ISimulationContext context)
        {
            this.context = context;
        }

        public void changeAnimationSpeed(double speed)
        {
            return;
        }

        public bool IsSimulationActive()
        {
            return true;
        }

        public bool? IsWordAccepted()
        {
            return null;
        }

        public void PauseSimulation()
        {
            return;
        }

        public void StartSimulation()
        {
            context.SimulationState = new RunningState(context);
        }

        public void StepBack()
        {
            StepBack(context);
        }

        public void StepForward()
        {
            StepForward(context);
        }

        public void StopSimulation()
        {
            context.SimulationState = new InitialState(context);
        }
    }
}
