 using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
  public class TransitionExecutor
    {   
       public void ExecuteTransition(Transition transition, ISimulationContext simulationContext, bool reverse = false)
        {
            simulationContext.CurrentTransition = transition;
            for (int i = 0; i < simulationContext.Tapes.Length; i++)
            {
                if(reverse) 
                {
                    simulationContext.ReadWriteHeads[i].MoveIn(transition.Directions[i]);
                    simulationContext.ReadWriteHeads[i].writeToCell(transition.InstructionTapeSymbol[i]);
                }
                else 
                {
                    simulationContext.ReadWriteHeads[i].writeToCell(transition.InstructionTapeSymbol[i]);
                    simulationContext.ReadWriteHeads[i].MoveIn(transition.Directions[i]);
                }
            }
            simulationContext.CurrentState = transition.InstructionState;
        }

        public Transition? FindTransition(char[] tapeSymbol, State state, ISimulationContext simulationContext)
        {
            return simulationContext.TuringMachine.Transitions.Find(t => t.ConditionState != null && t.ConditionState.Equals(state) && Enumerable.SequenceEqual(t.ConditionTapeSymbol,tapeSymbol)); 
        }

        public bool StepForward(object? context)
        {
            ISimulationContext? simulationContext = context as ISimulationContext;
            
            if (simulationContext != null)
            {
                char[] currentTapeSymbols = new char[simulationContext.Tapes.Length];
                for (int i = 0; i < simulationContext.ReadWriteHeads.Length; i++)
                {
                    currentTapeSymbols[i] = simulationContext.ReadWriteHeads[i].CurrentCell.TapeSymbol;
                    Debug.WriteLine("Transition Executor: reading currentTapeSymbol for tape"+i+" = "+currentTapeSymbols[i]);
                }
                if (simulationContext.CurrentState == null) { throw new NullReferenceException(); }
                Debug.WriteLine("Transition Executor: currentState->" + simulationContext.CurrentState.Name);

                if (isAcceptingState(simulationContext.CurrentState, simulationContext))
                {
                    Debug.WriteLine("Transition Executor: currentState is accepting state. Changing to EndState");
                    simulationContext.SimulationState = new EndState(simulationContext);
                    return false;
                }

                Transition? nextTransition = FindTransition(currentTapeSymbols, simulationContext.CurrentState, simulationContext);
                if (nextTransition == null)
                {
                    Debug.WriteLine("Transition Executor: No nextTransition was found! Changing to Endstate.");
                    simulationContext.SimulationState = new EndState(simulationContext);
                    return false;
                }

                Debug.WriteLine("Transition Executor: \nnextTransition = "+nextTransition.ToString());

                ExecuteTransition(nextTransition, simulationContext);
                simulationContext.TransitionHistory.Push(nextTransition);
                return true;
            }
            else
            {
                throw new NullReferenceException();
            }

        }

        public bool StepBack(object? context) 
        {
            ISimulationContext? simulationContext = context as ISimulationContext;
            if (simulationContext != null)
            {
                if (simulationContext.TransitionHistory.Count == 0) return false;

                Transition? lastTransition = simulationContext.TransitionHistory.Pop();

                Transition.Direction[] reverseDirections = new Transition.Direction[lastTransition.Directions.Length];

                for (int i = 0; i < lastTransition.Directions.Length; i++) 
                {
                    reverseDirections[i] = (Transition.Direction)(-1 * (int)lastTransition.Directions[i]);
                }
                if (lastTransition.InstructionState == null || lastTransition.ConditionState == null)
                {
                    throw new NullReferenceException();
                }
                ExecuteTransition(new Transition(lastTransition.InstructionState, lastTransition.ConditionState, lastTransition.InstructionTapeSymbol, 
                    lastTransition.ConditionTapeSymbol, reverseDirections, lastTransition.Description), simulationContext, true);
                
                return true;
            }
            else
            {
                throw new NullReferenceException();
            }
        }


        public bool isAcceptingState(State s, ISimulationContext ctx)
        {
            return ctx.TuringMachine.StateSet.AcceptingStates.Contains<State>(s);
        }
    }
}