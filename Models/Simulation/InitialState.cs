using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.Simulation
{
    public class InitialState : ISimulationState
    {
        private ISimulationContext context;

        public InitialState(ISimulationContext context)
        {
            this.context = context;
        }

        public void changeAnimationSpeed(double speed)
        {
            return;
        }

        public bool IsSimulationActive()
        {
            return false;
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
            if (context.TuringMachine.StateSet.StartingState != null)
            {
                context.CurrentState = context.TuringMachine.StateSet.StartingState;
                context.SimulationState = new RunningState(context);
            } else
            {
                throw new NullReferenceException();
            }
            
        }

        public void StepBack()
        {
            return;
        }

        public void StepForward()
        {
            return;
        }

        public void StopSimulation()
        {
            return;
        }
    }
}
