using Avalonia.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace turma.Models.TuringMachine
{
    public interface ITuringMachine
    {   
        public int TapeCount { get; set; }
        public List<Transition> Transitions { get; set; }
        public StateSet StateSet { get; set; }
        public TapeAlphabet TapeAlphabet { get; set; }

        public void TransformToEquivalentMachine();
    }
}
