using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.Simulation
{
    public class RunningState : ISimulationState
    {
        private ISimulationContext context;
        private SimulationThread simulationThread;

        public RunningState(ISimulationContext context)
        {
            this.context = context;
            simulationThread = new SimulationThread(context);
        }

        public void changeAnimationSpeed(double speed)
        {
            simulationThread.changeSpeed(speed);
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
            context.SimulationState = new PausedState(context);
            simulationThread.StopSimulation();
        }

        public void StartSimulation()
        {
            return;
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
            context.SimulationState = new InitialState(context); 
            simulationThread.StopSimulation();
        }
    }
}
