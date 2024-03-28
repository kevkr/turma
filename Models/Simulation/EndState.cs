using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.Simulation
{
   public class EndState : TransitionExecutor, ISimulationState
    {
        private ISimulationContext context;

        public EndState(ISimulationContext context)
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
            if (context.CurrentState == null)
            {
                throw new NullReferenceException();
            }
            if (context.TuringMachine.StateSet.AcceptingStates.Contains(context.CurrentState)){ 
                return true; 
            }
            else return false;
        }

        public void PauseSimulation()
        {
              return;
        }

        public void StartSimulation()
        {
            return;
        }

        public void StepBack()
        {
            StepBack(context);
            context.SimulationState = new PausedState(context);
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
