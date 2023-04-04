using System;
using static AutoBattle.Character;
using static AutoBattle.Grid;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            Grid grid;                        
            Character PlayerCharacter;
            Character EnemyCharacter;
            List<Character> AllPlayers = new List<Character>();
            int currentTurn = 0;
            
            Setup(); 

            void Setup()
            {
                GetBattlefieldSize();
                GetPlayerChoice();
            }

            void GetBattlefieldSize()
            {
                Console.WriteLine("Choose Battlefield Width: ");
                string input = Console.ReadLine();

                if (!Int32.TryParse(input, out var width))
                {
                    Console.WriteLine("Invalid parameter.");
                    GetBattlefieldSize();
                    return;
                }

                Console.WriteLine("Choose Battlefield Heigth: ");
                input = Console.ReadLine();

                if (!Int32.TryParse(input, out var height))
                {
                    Console.WriteLine("Invalid parameter.");
                    GetBattlefieldSize();
                    return;
                }

                grid = new Grid(width, height);

            }
            int numberOfPossibleTiles = grid.grids.Count;

            void GetPlayerChoice()
            {
                //asks for the player to choose between for possible classes via console.
                Console.WriteLine("Choose Between One of this Classes:\n");
                Console.WriteLine("[1] Paladin, [2] Warrior, [3] Cleric, [4] Archer");
                //store the player choice in a variable
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    case "2":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    case "3":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    case "4":
                        CreatePlayerCharacter(Int32.Parse(choice));
                        break;
                    default:
                        GetPlayerChoice();
                        break;
                }
            }

            void CreatePlayerCharacter(int classIndex)
            {
               
                CharacterClass characterClass = (CharacterClass)classIndex;
                Console.WriteLine($"Player Class Choice: {characterClass}");
                PlayerCharacter = new Character(grid, characterClass, true);                
                CreateEnemyCharacter();

            }

            void CreateEnemyCharacter()
            {
                //randomly choose the enemy class and set up vital variables
                var rand = new Random();
                int randomInteger = rand.Next(1, 4);
                CharacterClass enemyClass = (CharacterClass)randomInteger;
                Console.WriteLine($"Enemy Class Choice: {enemyClass}");
                EnemyCharacter = new Character(grid, enemyClass);                                
                StartGame();

            }

            void StartGame()
            {
                EnemyCharacter.Target = PlayerCharacter;
                PlayerCharacter.Target = EnemyCharacter;
                AllPlayers.Add(PlayerCharacter);
                AllPlayers.Add(EnemyCharacter);
                SetTurnOrder();
                //populates the character variables and targets
                
                AlocatePlayers();
                StartTurn();

            }

            void SetTurnOrder()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ROLLING FOR INITIATIVE...");
                Console.ResetColor();

                Random rnd = new Random();
                AllPlayers = AllPlayers.OrderBy(x => rnd.Next()).ToList();
                Console.WriteLine(" -- Turn Order -- ");
                for (var i = 0; i < AllPlayers.Count; i++)
                {
                    var character = AllPlayers[i];
                    character.CharacterIndex = i + 1;
                    character.WriteFullName();
                    Console.Write(Environment.NewLine);
                }
                Console.Write(Environment.NewLine);
            }

            void StartTurn(){

                if (currentTurn == 0)
                {
                    currentTurn++;
                    HandleTurn();
                    return;
                }

                foreach(Character character in AllPlayers)
                {
                    character.StartTurn();
                }

                currentTurn++;
                HandleTurn();
            }

            void HandleTurn()
            {
                if(PlayerCharacter.IsDead)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    Console.WriteLine("Defeat...");

                    Console.Write(Environment.NewLine + Environment.NewLine);
                    return;
                } else if (EnemyCharacter.IsDead)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    Console.WriteLine("Victory!");

                    Console.Write(Environment.NewLine + Environment.NewLine);

                    return;
                } else
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    Console.WriteLine("Click on any key to start the next turn...\n");
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    ConsoleKeyInfo key = Console.ReadKey();
                    StartTurn();
                }
            }

            int GetRandomInt(int min, int max)
            {
                var rand = new Random();
                int index = rand.Next(min, max);
                return index;
            }

            void AlocatePlayers()
            {
                foreach (Character character in AllPlayers)
                {
                    AlocateCharacter(character);
                }
                grid.DrawBattlefield();
            }

            void AlocateCharacter(Character character)
            {
                GridBox RandomLocation = grid.RandomUnoccupiedGridBox;
                //character.Alocate(RandomLocation);
                RandomLocation.occupiedBy = character;
                character.currentBox = RandomLocation;
                
                int index = RandomLocation.Index;
                grid.grids[RandomLocation.Index] = RandomLocation;                
                
            }
        }
    }
}
