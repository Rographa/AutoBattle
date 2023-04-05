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
            List<Character> PlayerCharacters = new List<Character>(); 
            List<Character> EnemyCharacters = new List<Character>(); 
            List<Character> AllPlayers = new List<Character>();
            int currentTurn = 0;
            int partySize;

            Setup(); 

            void Setup()
            {
                GetBattlefieldSize();
                GetPlayerChoices();
                GenerateEnemies();
                StartGame();
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

                Console.WriteLine("Choose Battlefield Height: ");
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

            void GetPlayerChoices()
            {
                GetBattleChoice();
                GetPlayerCharacters();
            }

            void GetBattleChoice()
            {
                Console.WriteLine("Choose a type of battle:\n");
                Console.WriteLine("[1] 1x1, [2] 2x2, [3] 3x3, [4] 4x4");
                string battleChoice = Console.ReadLine();                

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
                        GetBattleChoice();
                        break;
                }
            }

            void GetPlayerCharacters()
            {                
                for (int i = PlayerCharacters.Count; i < partySize; i++)
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
                            GetPlayerCharacters();
                            break;
                    }
                }
            }

            void GenerateEnemies()
            {
                for (int i = 0; i < partySize; i++)
                {
                    CreateEnemyCharacter();
                }
            }

            void CreatePlayerCharacter(int classIndex)
            {
               
                CharacterClass characterClass = (CharacterClass)classIndex;
                Console.WriteLine($"Player Class Choice: {characterClass}");
                var character = new Character(grid, characterClass, true);
                PlayerCharacters.Add(character);                

            }

            void CreateEnemyCharacter()
            {
                //randomly choose the enemy class and set up vital variables
                var rand = new Random();
                int randomInteger = rand.Next(1, 4);
                CharacterClass enemyClass = (CharacterClass)randomInteger;
                Console.WriteLine($"Enemy Class Choice: {enemyClass}");                
                var character = new Character(grid, enemyClass);       
                EnemyCharacters.Add(character);
            }

            void UpdateTargets()
            {
                var leastDistance = double.PositiveInfinity;
                Character currentTarget = null;

                foreach (var enemy in EnemyCharacters)
                {
                    foreach (var character in PlayerCharacters)
                    {
                        currentTarget ??= character;
                        var distance = enemy.CalculateDistance(character);
                        if (distance < leastDistance)
                        {
                            leastDistance = distance;
                            currentTarget = character;
                        }
                    }
                    enemy.Target = currentTarget;
                }

                currentTarget = null;

                foreach (var character in PlayerCharacters)
                {
                    foreach (var enemy in EnemyCharacters)
                    {
                        currentTarget ??= enemy;
                        var distance = character.CalculateDistance(enemy);
                        if (distance < leastDistance)
                        {
                            leastDistance = distance;
                            currentTarget = enemy;
                        }
                    }
                    character.Target = currentTarget;
                }
            }

            void StartGame()
            {                
                AllPlayers = PlayerCharacters.Concat(EnemyCharacters).ToList();
                SetTurnOrder();                                
                StartBattleInput();
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

                AlocatePlayers();
            }

            void StartBattleInput()
            {
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine("Press any key to start the battle!\n");
                Console.Write(Environment.NewLine + Environment.NewLine);

                ConsoleKeyInfo key = Console.ReadKey();
                StartTurn();
            }

            void StartTurn()
            {
                UpdateTargets();
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
                UpdateCharacters();

                if(PlayerCharacters.Count == 0)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    Console.WriteLine("Defeat...");

                    Console.Write(Environment.NewLine + Environment.NewLine);
                    return;
                } else if (EnemyCharacters.Count == 0)
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

            void UpdateCharacters()
            {
                for (int i = PlayerCharacters.Count - 1; i >= 0; i--)
                {
                    var character = PlayerCharacters[i];
                    if (!character.IsDead) continue;

                    PlayerCharacters.Remove(character);                    
                }

                for (int i = EnemyCharacters.Count - 1; i >= 0; i--)
                {
                    var character = EnemyCharacters[i];
                    if (!character.IsDead) continue;

                    EnemyCharacters.Remove(character);
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
