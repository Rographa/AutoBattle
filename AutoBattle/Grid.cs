using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Grid
    {
        public GridBox RandomUnoccupiedGridBox 
        {
            get 
            {
                var list = grids.FindAll(box => !box.Occupied);
                var random = new Random();
                int index = random.Next(list.Count - 1);
                return list[index];
            }
        }
        public List<GridBox> grids = new List<GridBox>();
        public int xLength;
        public int yLength;
        public Grid(int Lines, int Columns)
        {
            xLength = Lines;
            yLength = Columns;
            Console.WriteLine("The battlefield has been created\n");
            for (int i = 0; i < Lines; i++)
            {                    
                for(int j = 0; j < Columns; j++)
                {
                    GridBox newBox = new GridBox(i, j, null, (Columns * i + j));
                    grids.Add(newBox);
                    Console.Write($"{newBox.Index}\n");
                }
                
            }
        }

        // prints the matrix that indicates the tiles of the battlefield
        public void DrawBattlefield()
        {
            for (int i = 0; i < yLength; i++)
            {
                for (int j = 0; j < xLength; j++)
                {
                    GridBox currentgrid = grids.Find(box => box.xIndex == j && box.yIndex == i);
                    if (currentgrid.Occupied)
                    {
                        var character = currentgrid.occupiedBy;
                        var consoleColor = character.IsPlayerCharacter ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.ForegroundColor = consoleColor;
                        Console.Write($"[{character.Name}]\t");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"[{currentgrid.Index}]\t");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
        }

    }
}
