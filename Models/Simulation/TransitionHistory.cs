using System;
using System.Collections.Generic;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
    public class TransitionHistory
    {
        public Stack<Transition> transitionStack;

        public TransitionHistory()
        {
            transitionStack = new Stack<Transition>();
        }

        public void Push(Transition transition) 
        {
            transitionStack.Push(transition);
        }

        public Transition Pop()
        {
            return transitionStack.Pop();
        }

        public int Count
        {
            get { return transitionStack.Count; }
        }
    }
}
