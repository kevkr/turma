using Xunit;
using turma.Models.Simulation;
using System;
using System.Collections.Generic;
using System.Text;

using turma.Models.TuringMachine;

namespace turma.Models.Simulation.Tests
{
    public class TransitionHistoryTests
    {
        [Fact()]
        public void TestTransitionHistoryContructorIfTranitionStackIsStackOfTransitions()
        {
            TransitionHistory transitionHistory = new TransitionHistory();

            Assert.IsType<Stack<Transition>>(transitionHistory.transitionStack);
        }

        [Fact()]
        public void TestIfParameterTransitionGetsPushedOnStackOfTransitions()
        {
            TransitionHistory transitionHistory = new TransitionHistory();

            Transition transition = new Transition();

            transitionHistory.Push(transition);

            Assert.Equal(transitionHistory.transitionStack.Pop(), transition);
        }

        [Fact()]
        public void TestIfPushTransitionGetPopedFromStackOfTransitions()
        {
            TransitionHistory transitionHistory = new TransitionHistory();

            Transition transition = new Transition();
            Transition popTransition = new Transition();
            transitionHistory.Push(transition);

            popTransition = transitionHistory.Pop();

            Assert.Equal(popTransition, transition);
        }
    }
}