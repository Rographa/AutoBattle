using System;
using System.Collections.Generic;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Grid
    {
        // Getter property to automatically get an unoccupied GridBox randomly.
        public GridBox RandomUnoccupiedGridBox 
        {
            get 
            {
                var list = Grids.FindAll(box => !box.Occupied);
                var random = new Random();
                var index = random.Next(list.Count - 1);
                return list[index];
            }
        }
        public readonly List<GridBox> Grids = new List<GridBox>();
        public readonly int XLength;
        public readonly int YLength;
        public Grid(int lines, int columns)
        {
            XLength = lines;
            YLength = columns;
            
            for (var i = 0; i < lines; i++)
            {                    
                for(var j = 0; j < columns; j++)
                {
                    var newBox = new GridBox(i, j, null, (columns * i + j), this);
                    Grids.Add(newBox);
                    Console.Write($"{newBox.Index.ToString()}\n");
                }
                
            }
            Console.WriteLine("The battlefield has been created.\n");
        }

        // prints the matrix that indicates the tiles of the battlefield
        public void DrawBattlefield()
        {
            for (var i = 0; i < YLength; i++)
            {
                for (var j = 0; j < XLength; j++)
                {
                    var currentBox = Grids.Find(box => box.XIndex == j && box.YIndex == i);
                    if (currentBox.Occupied)
                    {
                        var character = currentBox.OccupiedBy;
                        var consoleColor = character.IsPlayerCharacter ? ConsoleColor.Green : ConsoleColor.Red;
                        Console.ForegroundColor = consoleColor;
                        Console.Write($"[{character.Name}]\t");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"[  ]\t");
                    }
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            Console.Write(Environment.NewLine + Environment.NewLine);
        }

    }
}
