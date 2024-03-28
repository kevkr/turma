using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.Simulation
{
    public interface ISimulationState
    {
        void StartSimulation();
        void StopSimulation();
        bool IsSimulationActive();
        void PauseSimulation();
        void StepBack();
        void StepForward();
        bool? IsWordAccepted();
        void changeAnimationSpeed(double speed);

    }
}
