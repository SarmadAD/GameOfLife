using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Security.Cryptography;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DevExpress.Blazor.Internal;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.Tracing;

namespace GameOfLife.ViewModel
{
    public class GameOfLifeViewModel
    {
        /* Game of life Regeln
         * 1.Eine tote Zelle mit genau drei lebenden Nachbarn wird in der Folgegeneration neu geboren.
         * 2.Lebende Zellen mit weniger als zwei lebenden Nachbarn sterben in der Folgegeneration an Einsamkeit.
         * 3.Eine lebende Zelle mit zwei oder drei lebenden Nachbarn bleibt in der Folgegeneration am Leben.
         * 4.Lebende Zellen mit mehr als drei lebenden Nachbarn sterben in der Folgegeneration an Überbevölkerung.
        */
        public event PropertyChangedEventHandler? PropertyChanged;

        public string CellColor { get; set; } = string.Empty;
        public bool FirstDrawOfCells { get; set; } = false;
        public bool SelfDraw { get; set; } = false;
        public int Generation { get; set; } = 0;
        public int LivingCells { get; set; } = 0;
        public int BeginnCells { get; set; } = 0;
        public bool HoldGameOfLifeAlive { get; set; } = true;
        public List<int> CellsList { get; set; } = new List<int>();
        private KeyValuePair<int, string> CurrentCell { get; set; } = new KeyValuePair<int, string>();
        public SortedDictionary<int, string> DictOfCells { get; set; } = new SortedDictionary<int, string>();
        private bool Init { get; set; } = false;
        public KeyValuePair<int, string>[,] CellArray = new KeyValuePair<int, string>[37, 65];
        public void NotifyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));

        public async Task StartTheGameOfLife()
        {
            try
            {
                //FirstDrawOfCells = true;
                HoldGameOfLifeAlive = true;
                await GenerateCells();
                LivingCells = DictOfCells.Count(x => x.Value == "LivingCell");
                do
                {
                    Generation++;
                    await Task.Delay(100);
                    FillDictionaryToTwoDimensionalArray();
                    FirstRule();
                    SecondRule();
                    ThirdRule();
                    FourthRule();
                    PutArrayToDict();
                    NotifyChanged();

                } while (HoldGameOfLifeAlive);
            }
            catch (Exception ex)
            { }
            finally
            {
            }
        }

        public void StopGameOfLife()
        {
            HoldGameOfLifeAlive = false;
            CellArray = new KeyValuePair<int, string>[37, 65];
            DictOfCells.Clear();
            Generation = 0;
            NotifyChanged();
        }

        private void PutArrayToDict() //Den Array in das Dict packen
        {
            foreach (var aCell in CellArray)
                if (!DictOfCells.Contains(aCell))
                    DictOfCells[aCell.Key] = aCell.Value;
        }

        private void FillDictionaryToTwoDimensionalArray()
        {
            var counter = 0;
            for (int i = 0; i < CellArray.GetLength(0); i++)
            {
                for (int j = 0; j < CellArray.GetLength(1); j++)
                {
                    Init = true;
                    if (CellArray[i, j].Key != 0 && !string.IsNullOrEmpty(CellArray[i, j].Value) || Init == true)
                    {
                        CellArray[i, j] = DictOfCells.ElementAt(counter);
                        counter++;
                        Init = false;
                    }
                    else break;
                }
            }
        }

        //1.Eine tote Zelle mit genau drei lebenden Nachbarn wird in der Folgegeneration neu geboren.
        private void FirstRule()
        {
            for (int y = 0; y < CellArray.GetLength(0); y++)
            {
                for (int x = 0; x < CellArray.GetLength(1); x++)
                {
                    CurrentCell = CellArray[y, x];
                    if (CurrentCell.Value == "DeadCell")
                    {
                        var livingNeigbors = CheckIfCellsAreNeighbors(y, x, "LivingCell");
                        if (livingNeigbors == 3)
                        {
                            var newCellPair = new KeyValuePair<int, string>(CurrentCell.Key, "LivingCell");
                            CellArray.SetValue(newCellPair, y, x);
                        }
                        else
                        {
                            //Nicht genug Nachbarn oder zu viele Nachbarn
                        }
                    }
                    else
                    {
                        //LivingCell
                    }
                }
            }
        }

        private int CheckIfCellsAreNeighbors(int y, int x, string stateOfCell)
        {
            var cellNeighborsCounter = 0;
            //ObenLinksEcke
            if (y == 0 && x == 0)
            {
                if (CellArray[y, x + 1].Value == stateOfCell) //Rechts
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x].Value == stateOfCell) //Unten
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x + 1].Value == stateOfCell) //UntenRechts
                    cellNeighborsCounter++;
            }
            //ObenRechtsEcke
            else if (y == 0 && x == 64)
            {
                if (CellArray[y, x - 1].Value == stateOfCell) //Links
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x].Value == stateOfCell) //Unten
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x - 1].Value == stateOfCell) //UntenLinks
                    cellNeighborsCounter++;
            }
            //UntenLinksEcke
            else if (y == 36 && x == 0)
            {
                if (CellArray[y - 1, x].Value == stateOfCell) //Oben
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x + 1].Value == stateOfCell) //ObenRechts
                    cellNeighborsCounter++;
                if (CellArray[y, x + 1].Value == stateOfCell) //Rechts
                    cellNeighborsCounter++;
            }
            //UntenRechtsEcke
            else if (y == 36 && x == 64)
            {
                if (CellArray[y - 1, x].Value == stateOfCell) //Oben
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x - 1].Value == stateOfCell) //ObenLinks
                    cellNeighborsCounter++;
                if (CellArray[y, x - 1].Value == stateOfCell) //Links
                    cellNeighborsCounter++;
            }
            //ObereGrenze
            else if ((y - 1 == -1) && (y == 0 && x != 0) && (y == 0 && x != 64))
            {
                if (CellArray[y, x - 1].Value == stateOfCell) //Links
                    cellNeighborsCounter++;
                if (CellArray[y, x + 1].Value == stateOfCell) //Rechts
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x].Value == stateOfCell) //Unten
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x + 1].Value == stateOfCell) //UntenRechts
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x - 1].Value == stateOfCell) //UntenLinks
                    cellNeighborsCounter++;
            }
            //LinkeGrenze
            else if ((x - 1 == -1) && (y != 0 && x == 0) && (y != 36 && x == 0))
            {
                if (CellArray[y, x + 1].Value == stateOfCell) //Rechts
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x].Value == stateOfCell) //Unten
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x].Value == stateOfCell) //Oben
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x + 1].Value == stateOfCell) //UntenRechts
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x + 1].Value == stateOfCell) //ObenRechts
                    cellNeighborsCounter++;
            }
            //UntereGrenze
            else if ((y + 1 == CellArray.GetLength(0)) && (y == 36 && x != 0) && (y == 36 && x != 64))
            {
                if (CellArray[y, x + 1].Value == stateOfCell) //Rechts
                    cellNeighborsCounter++;
                if (CellArray[y, x - 1].Value == stateOfCell) //Links
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x].Value == stateOfCell) //Oben
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x + 1].Value == stateOfCell) //ObenRechts
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x - 1].Value == stateOfCell) //ObenLinks
                    cellNeighborsCounter++;
            }
            //RechteGrenze
            else if ((x + 1 == CellArray.GetLength(1)) && (y != 36 && x == 64) && (y != 0 && x == 64))
            {
                if (CellArray[y, x - 1].Value == stateOfCell) //Links
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x].Value == stateOfCell) //Oben
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x - 1].Value == stateOfCell) //ObenLinks
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x].Value == stateOfCell) //Unten
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x - 1].Value == stateOfCell) //UntenLinks
                    cellNeighborsCounter++;
            }
            //Mitte
            else
            {
                if (CellArray[y - 1, x].Value == stateOfCell) //Oben
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x + 1].Value == stateOfCell) //ObenRechts
                    cellNeighborsCounter++;
                if (CellArray[y - 1, x - 1].Value == stateOfCell) //ObenLinks
                    cellNeighborsCounter++;

                if (CellArray[y + 1, x].Value == stateOfCell) //Unten
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x + 1].Value == stateOfCell) //UntenRechts
                    cellNeighborsCounter++;
                if (CellArray[y + 1, x - 1].Value == stateOfCell) //UntenLinks
                    cellNeighborsCounter++;

                if (CellArray[y, x - 1].Value == stateOfCell) //Links
                    cellNeighborsCounter++;
                if (CellArray[y, x + 1].Value == stateOfCell) //Rechts
                    cellNeighborsCounter++;
            }
            return cellNeighborsCounter;
        }

        //* 2.Lebende Zellen mit weniger als zwei lebenden Nachbarn sterben in der Folgegeneration an Einsamkeit.
        private void SecondRule()
        {
            for (int y = 0; y < CellArray.GetLength(0); y++)
            {
                for (int x = 0; x < CellArray.GetLength(1); x++)
                {
                    CurrentCell = CellArray[y, x];
                    if (CurrentCell.Value == "LivingCell")
                    {
                        var cellNeighbors = CheckIfCellsAreNeighbors(y, x, "LivingCell");
                        if (cellNeighbors < 2)
                        {
                            var newCellPair = new KeyValuePair<int, string>(CurrentCell.Key, "DeadCell");
                            CellArray.SetValue(newCellPair, y, x);
                        }
                        else
                        {
                            //Zu wenige Nachbarn
                        }
                    }
                    else
                    {
                        //Tote Zelle
                    }
                }
            }
        }

        //* 3.Eine lebende Zelle mit zwei oder drei lebenden Nachbarn bleibt in der Folgegeneration am Leben.
        private void ThirdRule()
        {
            for (int y = 0; y < CellArray.GetLength(0); y++)
            {
                for (int x = 0; x < CellArray.GetLength(1); x++)
                {
                    CurrentCell = CellArray[y, x];
                    if (CurrentCell.Value == "LivingCell")
                    {
                        var cellNeighbors = CheckIfCellsAreNeighbors(y, x, "LivingCell");
                        if (cellNeighbors == 2 || cellNeighbors == 3)
                        {
                            var newCellPair = new KeyValuePair<int, string>(CurrentCell.Key, "LivingCell");
                            CellArray.SetValue(newCellPair, y, x);
                        }
                        else
                        {
                            //Zu wenige Nachbarn
                        }
                    }
                    else
                    {
                        //Tote Zelle
                    }
                }
            }

        }

        //* 4.Lebende Zellen mit mehr als drei lebenden Nachbarn sterben in der Folgegeneration an Überbevölkerung.
        private void FourthRule()
        {
            for (int y = 0; y < CellArray.GetLength(0); y++)
            {
                for (int x = 0; x < CellArray.GetLength(1); x++)
                {
                    CurrentCell = CellArray[y, x];
                    if (CurrentCell.Value == "LivingCell")
                    {
                        var cellNeighbors = CheckIfCellsAreNeighbors(y, x, "LivingCell");
                        if (cellNeighbors > 3)
                        {
                            var newCellPair = new KeyValuePair<int, string>(CurrentCell.Key, "DeadCell");
                            CellArray.SetValue(newCellPair, y, x);
                        }
                        else
                        {
                            //Zu wenige Nachbarn
                        }
                    }
                    else
                    {
                        //Tote Zelle
                    }
                }
            }
        }

        private async Task GenerateCells()
        {
            var rnd = new Random();
            DictOfCells.Clear();
            for (int i = 0; i < BeginnCells; i++)
            {
                var cell = rnd.Next(1, 2405);
                if (!DictOfCells.ContainsKey(cell))
                    DictOfCells.Add(cell, "LivingCell");
            }
            AddDeadCells();
        }

        private void AddDeadCells()
        {
            for (int i = 0; i < 2405; i++)
            {
                if (!DictOfCells.ContainsKey(i))
                    DictOfCells.Add(i, "DeadCell");
            }
        }

        public void SetCells(MouseEventArgs args)
        {
            SelfDraw = true;
        }
    }
}
