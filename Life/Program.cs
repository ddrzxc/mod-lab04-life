using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace cli_life
{
    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }
        public int Generation;

        public Board(int width, int height, int cellSize, double liveDensity = .1)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
            Randomize(liveDensity);
            Generation = 0;
        }
        public Board(string fileName)
        {
            string[] strings = File.ReadAllLines(fileName);
            int fileColumns = strings.Min(s => s.Length);
            int fileRows = strings.Length;
            Cells = new Cell[fileColumns, fileRows];
            for (int x = 0; x < fileColumns; x++)
                for (int y = 0; y < fileRows; y++)
                {
                    Cells[x, y] = new Cell
                    {
                        IsAlive = (strings[y][x] == '*')
                    };
                }
            ConnectNeighbors();
            Generation = 0;
        }

        readonly Random rand = new Random();
        public void Randomize(double liveDensity)
        {
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < liveDensity;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
            Generation++;
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
        public static Board WithSettings(string fileName)
        {
            Board board;
            dynamic settings = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(fileName));
            board = new Board(
                settings.width,
                settings.height,
                settings.cellSize,
                settings.liveDensity
            );
            return board;
        }
        public void Save(string fileName)
        {
            string[] strings = new string[Rows];
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (Cells[x, y].IsAlive)
                        strings[y] += '*';
                    else
                        strings[y] += ' ';
                }
            }
            File.WriteAllLines(fileName, strings);
        }
        public int CountElement(int[,] element)
        {
            int count = 0;
            List<(int, int)> counted = new List<(int, int)>();
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    for (int i = 0; i < 4; i++)
                    {
                        int[,] submatrix = Submatrix(x, y, element.GetLength(1), element.GetLength(0));
                        submatrix = submatrix.Rotate90();
                        if (element.SequenceEquals(submatrix) && !counted.Contains((x, y)))
                        {
                            counted.Add((x, y));
                            count++;
                        }
                    }
            return count;
        }
        public int[,] Submatrix(int x, int y, int width, int height)
        {
            int[,] res = new int[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    int k = x + i, l = y + j;
                    if (k >= Columns) k -= Columns;
                    if (l >= Rows) l -= Rows;
                    res[i, j] = Convert.ToInt32(Cells[k, l].IsAlive);
                }
            return res;
        }
    }
    public static class Extensions
    {
        public static bool SequenceEquals<T>(this T[,] a, T[,] b) => a.Rank == b.Rank
            && Enumerable.Range(0, a.Rank).All(d=> a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());
        public static int[,] Rotate90(this int[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] result = new int[cols, rows];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    result[col, rows - row - 1] = matrix[row, col];
                }
            }
            return result;
        }
    }
    class Program
    {
        static Board board;
        static private void Reset()
        {
            board = new Board(
                width: 50,
                height: 20,
                cellSize: 1,
                liveDensity: 0.5);
        }
        static void Render()
        {
            for (int row = 0; row < board.Rows; row++)
            {
                for (int col = 0; col < board.Columns; col++)   
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
        }
        static void Main(string[] args)
        {
            //Reset();
            board = Board.WithSettings("settings.json");
            int[,] block = {
                {0, 0, 0, 0},
                {0, 1, 1, 0},
                {0, 1, 1, 0},
                {0, 0, 0, 0}
            };
            while(true)
            {
                Console.Clear();
                Render();
                Console.WriteLine($"Поколение {board.Generation}");
                int liveNeighbors = board.Cells.Cast<Cell>().Where(x => x.IsAlive).Count();
                Console.WriteLine($"Живых клеток {liveNeighbors}");
                int count = board.CountElement(block);
                Console.WriteLine($"Block: {count}");
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey();
                    switch (key.KeyChar)
                    {
                    case 'l':
                        board = new Board("board.txt");
                        break;
                    case 's':
                        board.Save("save.txt");
                        break;
                    }
                }
                board.Advance();
                Thread.Sleep(100);
            }
        }
    }
}