using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
    public class ReadWriteHead
    {
        private Tape tape;

        public delegate void TapeEventHandler(EventArgs e);
        public event TapeEventHandler? TapeEvent;

        public Cell CurrentCell { get; set; }
        public int CellIndex { get; set; }

        public ReadWriteHead(Tape tape)
        {
            this.CellIndex = tape.Cells.Count / 2;
            this.tape = tape;
            this.CurrentCell = tape.Cells[CellIndex];
        }

        public void MoveIn(Transition.Direction direction)
        {
            if (CellIndex <= 50 && direction == Transition.Direction.left)
            {
                CellIndex += tape.ExtendTapeCapacity(-1);
            }
            if (CellIndex >= tape.TapeCapacity - 50 && direction == Transition.Direction.right)
            {
                tape.ExtendTapeCapacity(1);
            }
            CellIndex += (int)direction;
            Debug.WriteLine("ReadWriteHead Cellindex: " + CellIndex);
            this.CurrentCell = tape.Cells[CellIndex];
            if (TapeEvent != null)
            {
                TapeEvent(new EventArgs());
            }
        }

        public void writeToCell(char symbol)
        {
            CurrentCell.TapeSymbol = symbol;
            if (TapeEvent != null)
            {
                TapeEvent(new EventArgs());
            }
        }

        public void redrawTape()
        {
            if (TapeEvent != null)
            {
                TapeEvent(new EventArgs());
            }
        }
    }
}
