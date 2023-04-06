using System;
using static AutoBattle.Character;
using static AutoBattle.Grid;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;
using System.Collections.ObjectModel;

namespace AutoBattle
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Grid grid;
            var random = new Random();
            var playerCharacters = new List<Character>(); 
            var enemyCharacters = new List<Character>(); 
            var allPlayers = new List<Character>();
            var currentTurn = 0;
            int partySize;

            Setup(); 

            void Setup()
            {
                GetBattlefieldSize();
                SetupCharacterClasses();
                GetPlayerChoices();
                GenerateEnemies();
                StartGame();
            }

            void GetBattlefieldSize()
            {
                while (true)
                {
                    Console.WriteLine("Choose Battlefield Width: ");
                    var input = Console.ReadLine();

                    if (!int.TryParse(input, out var width))
                    {
                        Console.WriteLine("Invalid parameter.");
                        continue;
                    }

                    Console.WriteLine("Choose Battlefield Height: ");
                    input = Console.ReadLine();

                    if (!int.TryParse(input, out var height))
                    {
                        Console.WriteLine("Invalid parameter.");
                        continue;
                    }

                    grid = new Grid(width, height);

                    break;
                }
            }

            void GetPlayerChoices()
            {
                GetBattleChoice();
                GetPlayerCharacters();
            }

            void GetBattleChoice()
            {
                while (true)
                {
                    Console.WriteLine("Choose a type of battle:\n");
                    Console.WriteLine("[1] 1x1, [2] 2x2, [3] 3x3, [4] 4x4");
                    var battleChoice = Console.ReadLine();

                    switch (battleChoice)
                    {
                        case "1":
                            partySize = 1;
                            break;
                        case "2":
                            partySize = 2;
                            break;
                        case "3":
                            partySize = 3;
                            break;
                        case "4":
                            partySize = 4;
                            break;
                        default:
                            continue;
                    }
                    break;
                }
            }

            void GetPlayerCharacters()
            {                
                for (var i = playerCharacters.Count; i < partySize; i++)
                {
                    //asks for the player to choose between for possible classes via console.
                    Console.WriteLine("Choose Between One of this Classes:\n");
                    Console.WriteLine("[1] Paladin, [2] Warrior, [3] Cleric, [4] Archer");
                    //store the player choice in a variable
                    var choice = Console.ReadLine();
                    switch (choice)
                    {
                        case "1":
                            CreatePlayerCharacter(int.Parse(choice));
                            break;
                        case "2":
                            CreatePlayerCharacter(int.Parse(choice));
                            break;
                        case "3":
                            CreatePlayerCharacter(int.Parse(choice));
                            break;
                        case "4":
                            CreatePlayerCharacter(int.Parse(choice));
                            break;
                        default:
                            GetPlayerCharacters();
                            break;
                    }
                }
            }

            void GenerateEnemies()
            {
                for (var i = 0; i < partySize; i++)
                {
                    CreateEnemyCharacter();
                }
            }

            void CreatePlayerCharacter(int classIndex)
            {
               
                var characterClass = (CharacterClass)classIndex;
                Console.WriteLine($"Player Class Choice: {characterClass}");
                var character = new Character(grid, characterClass, true);
                playerCharacters.Add(character);                

            }

            void CreateEnemyCharacter()
            {
                //randomly choose the enemy class and set up vital variables
                var randomInteger = random.Next(1, 4);
                var enemyClass = (CharacterClass)randomInteger;
                Console.WriteLine($"Enemy Class Choice: {enemyClass}");                
                var character = new Character(grid, enemyClass);       
                enemyCharacters.Add(character);
            }

            void StartGame()
            {                
                allPlayers = playerCharacters.Concat(enemyCharacters).ToList();
                SetTurnOrder();                                
                StartBattleInput();
            }

            void SetTurnOrder()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ROLLING FOR INITIATIVE...");
                Console.ResetColor();
                
                allPlayers = allPlayers.OrderBy(x => random.Next()).ToList();
                Console.WriteLine(" -- Turn Order -- ");
                for (var i = 0; i < allPlayers.Count; i++)
                {
                    var character = allPlayers[i];
                    character.CharacterIndex = i + 1;
                    character.WriteFullName();
                    Console.Write(Environment.NewLine);
                }
                Console.Write(Environment.NewLine);
                AllocatePlayers();
            }

            void StartBattleInput()
            {
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Press any key to start the battle!\n");
                Console.Write(Environment.NewLine + Environment.NewLine);
                var key = Console.ReadKey();
                StartTurn();
            }

            void StartTurn()
            {
                foreach (var character in allPlayers)
                {
                    character.UpdateClosestTarget(character.IsPlayerCharacter ? enemyCharacters : playerCharacters);
                    character.StartTurn();
                }

                currentTurn++;
                HandleTurn();
            }

            void HandleTurn()
            {
                UpdateCharacters();

                if(playerCharacters.Count == 0)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    Console.WriteLine("Defeat...");

                    Console.Write(Environment.NewLine + Environment.NewLine);
                    return;
                }
                if (enemyCharacters.Count == 0)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    Console.WriteLine("Victory!");

                    Console.Write(Environment.NewLine + Environment.NewLine);

                    return;
                } 
                
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Click on any key to start the next turn...\n");
                Console.Write(Environment.NewLine + Environment.NewLine);

                var key = Console.ReadKey();
                StartTurn();
            }

            void UpdateCharacters()
            {
                for (var i = playerCharacters.Count - 1; i >= 0; i--)
                {
                    var character = playerCharacters[i];
                    if (!character.IsDead) continue;

                    playerCharacters.Remove(character);                    
                }

                for (var i = enemyCharacters.Count - 1; i >= 0; i--)
                {
                    var character = enemyCharacters[i];
                    if (!character.IsDead) continue;

                    enemyCharacters.Remove(character);
                }
            }

            void AllocatePlayers()
            {
                foreach (var character in allPlayers)
                {
                    AllocateCharacter(character);
                }
                grid.DrawBattlefield();
            }

            void AllocateCharacter(Character character)
            {
                var randomLocation = grid.RandomUnoccupiedGridBox;
                randomLocation.OccupiedBy = character;
                character.CurrentBox = randomLocation;
                
                var index = randomLocation.Index;
                grid.Grids[index] = randomLocation;
            }
        }
    }
}
