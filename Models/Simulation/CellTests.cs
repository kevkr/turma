using Xunit;
using turma.Models.Simulation;
using System;
using System.Collections.Generic;
using System.Text;

namespace turma.Models.Simulation.Tests
{
    public class CellTests
    {
        [Fact()]
        public void TestIfCellConstruktorSetTapeSymbolParameterAsTapeSymbolOfTheCell()
        {
            Cell cell = new Cell('a');

            Assert.Equal('a', cell.TapeSymbol);
        }
    }
}