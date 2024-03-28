using Xunit;
using turma.Models.Simulation;
using System;
using System.Collections.Generic;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation.Tests
{
    public class TapeTests
    {
        [Fact()]
        public void TestTapeConstructurSetTapeCapacityCorrect()
        {
            Tape tape = new Tape();

            Assert.Equal(150,tape.TapeCapacity);
        }

        [Fact()]
        public void TestTapeConstructorListOfCellsLengthEqualTapeCapacity()
        {
            Tape tape = new Tape();

            Assert.Equal(150, tape.Cells.Count);
        }

        [Fact()]
        public void TestIfTapeCapacityExtendsTwentyCellsWhenMoveLeft()
        {
            Tape tape = new Tape();

            int direction = -1; 
            int tapeCapacitiyBefore = tape.TapeCapacity;

            tape.ExtendTapeCapacity(direction);

            Assert.Equal(tapeCapacitiyBefore + 75, tape.TapeCapacity);
        }

        [Fact()]
        public void TestIfTapeCellsListExtendsTwentyCellsWhenMoveLeft()
        {
            Tape tape = new Tape();

            int direction = -1;
            int tapeCapacitiyBefore = tape.TapeCapacity;

            tape.ExtendTapeCapacity(direction);

            Assert.Equal(tapeCapacitiyBefore + 75, tape.Cells.Count);
        }

        [Fact()]
        public void TestIfTapeCapacityExtendsTwentyCellsWhenMoveRight()
        {
            Tape tape = new Tape();

            int direction = 1; 
            int tapeCapacitiyBefore = tape.TapeCapacity;

            tape.ExtendTapeCapacity(direction);
            
            Assert.Equal(tapeCapacitiyBefore + 75, tape.TapeCapacity);
        }

        [Fact()]
        public void TestIfTapeCellsListExtendsTwentyCellsWhenMoveRight()
        {
            Tape tape = new Tape();

            int direction = 1;
            int tapeCapacitiyBefore = tape.TapeCapacity;

            tape.ExtendTapeCapacity(direction);

            Assert.Equal(tapeCapacitiyBefore + 75, tape.Cells.Count);
        }

        [Fact()]
        public void TestIfExtendTapeCapacityReturnsTwenty()
        {
            Tape tape = new Tape();

            int testExtensionAmount = tape.ExtendTapeCapacity(1);

            Assert.Equal(75, testExtensionAmount);
        }
    }
}