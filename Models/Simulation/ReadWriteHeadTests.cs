using Xunit;
using turma.Models.Simulation;
using System;
using System.Collections.Generic;
using System.Text;
using turma.Models.TuringMachine;

namespace turma.Models.Simulation.Tests
{
    public class ReadWriteHeadTests
    {
        [Fact()]
        public void ReadWriteHeadConstructorTestIfCellIndexIsSetCorret()
        {
            Tape tape = new Tape();

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);

            Assert.Equal(75, readWriteHead.CellIndex);
        }

        [Fact()]
        public void ReadWriteHeadConstructorTestIfCurrentCellEqualsCellsFromTapeFromTapeIndex()
        {
            Tape tape = new Tape();

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);

            Assert.Equal(readWriteHead.CurrentCell, tape.Cells[readWriteHead.CellIndex]);
        }

        [Fact()]
        public void TestIfCellIndexDecreaseWhenMoveLeft()
        {
            Tape tape = new Tape();

            Transition.Direction direction = new Transition.Direction();
            direction = Transition.Direction.left;

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);

            int CellIndexBefore = readWriteHead.CellIndex;

            readWriteHead.MoveIn(direction);

            Assert.Equal(CellIndexBefore - 1, readWriteHead.CellIndex);
        }

        [Fact()]
        public void TestIfCurrentCellEqualsTapeCellWithNewDecreaseCellIndexMoveLeft()
        {
            Tape tape = new Tape();

            Transition.Direction direction = new Transition.Direction();
            direction = Transition.Direction.left;

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);

            readWriteHead.MoveIn(direction);

            Assert.Equal(readWriteHead.CurrentCell, tape.Cells[readWriteHead.CellIndex]);
        }

        [Fact()]
        public void TestIfCellIndexIncreaseWhenMoveRight()
        {
            Tape tape = new Tape();

            Transition.Direction direction = new Transition.Direction();
            direction = Transition.Direction.right;

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);

            int CellIndexBefore = readWriteHead.CellIndex;

            readWriteHead.MoveIn(direction);

            Assert.Equal(CellIndexBefore + 1,readWriteHead.CellIndex);
            Assert.Equal(readWriteHead.CurrentCell, tape.Cells[readWriteHead.CellIndex]);
        }

        [Fact()]
        public void TestIfCurrentCellEqualsTapeCellWithNewIncreaseCellIndexMoveRight()
        {
            Tape tape = new Tape();

            Transition.Direction direction = new Transition.Direction();
            direction = Transition.Direction.right;

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);

            readWriteHead.MoveIn(direction);

            Assert.Equal(readWriteHead.CurrentCell, tape.Cells[readWriteHead.CellIndex]);
        }

        [Fact()]
        public void TestIfWritenSymbolIsTheTapeSymbolOfTheCurrentCell()
        {
            Tape tape = new Tape();

            ReadWriteHead readWriteHead = new ReadWriteHead(tape);
            char symbol = 'a';

            readWriteHead.writeToCell(symbol);

            Assert.Equal('a', readWriteHead.CurrentCell.TapeSymbol);
        }
    }
}