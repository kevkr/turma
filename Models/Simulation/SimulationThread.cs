using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
    class SimulationThread : TransitionExecutor
    {
        private Timer timer;
        public SimulationThread(ISimulationContext context)
        {
            timer = new Timer(TransitionExecutionCallback, context, TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(context.AnimationSpeed));
        }

        /*Callback Function for the Timer, to periodically find and execute the next transition*/
        public void TransitionExecutionCallback(object? context)
        {
            if (!StepForward(context)) StopSimulation();
        }

        public void changeSpeed(double speed)
        {
            timer.Change(TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(speed));
        }

        public void StopSimulation() {
            timer.Dispose();
        }
    }
}
