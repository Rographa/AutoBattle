using System;
using static AutoBattle.Character;
using static AutoBattle.Grid;
using static AutoBattle.Input;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;

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
                SetupCharacterClasses();
                GetPlayerChoices();
                GenerateEnemies();
                StartGame();
            }

            void GetPlayerChoices()
            {
                var size = GetBattlefieldSize();
                grid = new Grid(size.x, size.y);
                partySize = GetBattleChoice();
                for (var i = playerCharacters.Count; i < partySize; i++)
                {
                    GetPlayerCharacter(CreatePlayerCharacter);
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
                var randomInteger = random.Next(1, 5);
                var enemyClass = (CharacterClass)randomInteger;
                Console.WriteLine($"Enemy Class Choice: {enemyClass}");                
                var character = new Character(grid, enemyClass);       
                enemyCharacters.Add(character);
            }

            void StartGame()
            {                
                allPlayers = playerCharacters.Concat(enemyCharacters).ToList();
                SetTurnOrder();                                
                StartBattleInput(StartTurn);
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
                
                Console.Write(Environment.NewLine + Environment.NewLine);
                Console.WriteLine($"End of turn {currentTurn.ToString()}.");
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
                
                WaitForNextTurnInput(StartTurn);
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
