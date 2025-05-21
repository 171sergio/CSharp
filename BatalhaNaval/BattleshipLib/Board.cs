using System;

namespace BattleshipLib
{
    public class Board
    {
        private readonly char[,] grid;
        private int shipCount;

        public Board()
        {
            grid = new char[10, 10];
            for (int r = 0; r < 10; r++)
                for (int c = 0; c < 10; c++)
                    grid[r, c] = '~';
            shipCount = 0;
        }

        public void Print(bool showShips)
        {
            Console.Write("   ");
            for (int c = 0; c < 10; c++) 
                Console.Write($"{c} ");
            Console.WriteLine();
            
            for (int r = 0; r < 10; r++)
            {
                Console.Write($"{(char)('A' + r)}  ");
                for (int c = 0; c < 10; c++)
                {
                    char cell = grid[r, c];
                    Console.Write(!showShips && cell == '*' ? "~ " : $"{cell} ");
                }
                Console.WriteLine();
            }
        }

        public void PlaceShipsRandomly(int n)
        {
            var rnd = new Random();
            int placed = 0;
            while (placed < n)
            {
                int r = rnd.Next(10), c = rnd.Next(10);
                if (grid[r, c] == '~')
                {
                    grid[r, c] = '*';
                    placed++;
                    shipCount++;
                }
            }
        }

        public bool PlaceShipManually(string coord)
        {
            try
            {
                var (row, col) = ParseCoordinate(coord);
                if (grid[row, col] != '~')
                    return false;
                
                grid[row, col] = '*';
                shipCount++;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool IsShip(int row, int col)
        {
            return grid[row, col] == '*';
        }

        public void MarkHit(int row, int col)
        {
            if (grid[row, col] == '*')
            {
                grid[row, col] = 'X';
                shipCount--;
            }
        }

        public void MarkMiss(int row, int col)
        {
            if (grid[row, col] == '~')
                grid[row, col] = 'O';
        }

        public bool AreAllShipsSunk()
        {
            return shipCount == 0;
        }

        public static (int row, int col) ParseCoordinate(string coord)
        {
            if (string.IsNullOrEmpty(coord) || coord.Length < 2)
                throw new ArgumentException("Invalid coordinate format");

            int row = char.ToUpper(coord[0]) - 'A';
            if (!int.TryParse(coord.Substring(1), out int col))
                throw new ArgumentException("Invalid coordinate format");

            if (row < 0 || row > 9 || col < 0 || col > 9)
                throw new ArgumentException("Coordinate out of range");

            return (row, col);
        }
    }
} 