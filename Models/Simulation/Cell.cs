using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.Simulation
{
    public class Cell
    {
        public Cell(char tapeSymbol)
        {
            this.TapeSymbol = tapeSymbol;
        }

        public char TapeSymbol { get; set; }
    }
}
