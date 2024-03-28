using System.Collections.Generic;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation
{
    public class Tape
    {
        private double tapeExtensionFactor = 0.5;
        private int extensionAmount;

        public List<Cell> Cells { get; set; }

        public int TapeCapacity { get; set; }

        public Tape()
        {
            TapeCapacity = 150; //Initial TapeCapacity
            extensionAmount = (int)(TapeCapacity * tapeExtensionFactor);
            Cells = new List<Cell>();
            for (int i = 0; i < TapeCapacity; i++)
            {
                Cells.Add(new Cell(TapeAlphabet.Epsilon));
            }
        }

        public int ExtendTapeCapacity(int direction)
        {
            for (int i = 0; i < extensionAmount; i++)
            {
                if (direction >= 0)
                {
                    Cells.Add(new Cell(TapeAlphabet.Epsilon));
                }
                else
                {
                    Cells.Insert(0,new Cell(TapeAlphabet.Epsilon));
                }
            }

            TapeCapacity += extensionAmount;
            return extensionAmount;

        }
    }
}
