using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using turma.Models.Simulation;
using turma.Models.TuringMachine;
using turma.ViewModels;

namespace turma.Views
{
    public partial class TapeView : UserControl
    {
        public ISimulationContext? Ctx { get; set; }
        public int tapeIndex;
        private List<Border> cellBoxes = new List<Border>();
        private readonly int CELL_WIDTH = 40;

        public TapeView()
        {
            InitializeComponent(this.Width);
        }

        public TapeView(ISimulationContext simulationContext, int tapeIndex, double width)
        {

            Debug.WriteLine("TapeView added. Tapeindex="+tapeIndex);

            InitializeComponent(width);

            this.Ctx = simulationContext;
            this.tapeIndex = tapeIndex;

            Ctx.ReadWriteHeads[tapeIndex].TapeEvent += new ReadWriteHead.TapeEventHandler(HandleTapeEvent);

            updateTape();
        }

        public void InitializeComponent(double width)
        {
            AvaloniaXamlLoader.Load(this);
            this.Width = width;
            int numberOfCells = (int) width / CELL_WIDTH;
            if(numberOfCells % 2 == 0)
            {
                numberOfCells--;
            }

            WrapPanel wrapPanel = this.Find<WrapPanel>("Tape");

            cellBoxes.Clear();
            for(int i = 0; i < numberOfCells; i++)
            {
                Border b = new Border();
                b.BorderBrush = Brushes.Black;
                b.BorderThickness = new Thickness(1);
                b.Width = CELL_WIDTH;
                b.Height = CELL_WIDTH;

                if (i % 2 == 0)
                {
                    b.Background = Brushes.LightGray;
                }
                else
                {
                    b.Background = Brushes.White;
                }

                TextBlock t = new TextBlock();
                t.FontSize = 20;
                t.FontStyle = FontStyle.Normal;
                t.FontWeight = FontWeight.Bold;
                t.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                t.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                b.Child = t;
                cellBoxes.Add(b);
                wrapPanel.Children.Add(b);
            }
        }

        public void HandleTapeEvent(EventArgs e)
        {
            updateTape();
        }

        public void updateTape()
        {
            if (Ctx != null)
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    for (int i = 0; i < cellBoxes.Count; i++)
                    {
                        TextBlock t = (TextBlock) cellBoxes[i].Child;
                        int index = Ctx.ReadWriteHeads[tapeIndex].CellIndex + i - cellBoxes.Count / 2;
                        char symbol = Ctx.Tapes[tapeIndex].Cells[index].TapeSymbol;
                        if (symbol == TapeAlphabet.Epsilon) symbol = ' ';
                        t.Text = symbol.ToString();

                        Border b = cellBoxes[i];
                      
                        if (index % 2 == 0)
                        {
                           b.Background = Brushes.LightGray;
                        }
                        else
                        {
                           b.Background = Brushes.White;
                        }
                    }
                });
            }
        }
    }
}
