using System;
using static AutoBattle.Character;
using static AutoBattle.Input;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;
using static AutoBattle.Stage;

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
                ShowIntroText();
                SetupCharacterClasses();
                grid = SetupBattlefield();
                GetPlayerChoices();
                GenerateEnemies();
                StartGame();
            }

            void ShowIntroText()
            {
                Console.WriteLine("Welcome to AutoBattle! Choose a battlefield size, battle type and your team and watch them fight for the victory!");
            }
            
            
            void GetPlayerChoices()
            {
                // First we get the battle type (1x1, 2x2, 3x3, 4x4)
                partySize = GetBattleChoice();
                // Then, ask a new input for player to choose a class for each player character and create it.
                for (var i = playerCharacters.Count; i < partySize; i++)
                {
                    GetPlayerCharacter(CreatePlayerCharacter);
                }
            }

            // Create a Character for Player's team, based on class input.
            void CreatePlayerCharacter(int classIndex)
            {
                var characterClass = (CharacterClass)classIndex;
                Console.WriteLine($"Player Class Choice: {characterClass}");
                var character = new Character(characterClass, true);
                playerCharacters.Add(character);
            }

            void CreateEnemyCharacter()
            {
                // Randomly choose the enemy class
                var randomInteger = random.Next(1, 5);
                var enemyClass = (CharacterClass)randomInteger;
                Console.WriteLine($"Enemy Class Choice: {enemyClass}");                
                var character = new Character(enemyClass);       
                enemyCharacters.Add(character);
            }
            
            void GenerateEnemies()
            {
                for (var i = 0; i < partySize; i++)
                {
                    CreateEnemyCharacter();
                }
            }

            void StartGame()
            {  
                // Merge both player and enemy character into a unique list.
                allPlayers = playerCharacters.Concat(enemyCharacters).ToList();
                SetTurnOrder();                                
                StartBattleInput(StartTurn);
            }

            // Randomly sorts the turn order for each character.
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
                // For each character in game, get the closest target and start its turn.
                foreach (var character in allPlayers)
                {
                    character.UpdateClosestTarget(character.IsPlayerCharacter ? enemyCharacters : playerCharacters);
                    character.StartTurn();
                }
                
                // After all characters' turns, increase the turn number and check victory/defeat.
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

                    Console.WriteLine("Defeat...\n");

                    Console.Write(Environment.NewLine + Environment.NewLine);
                    
                    Console.WriteLine("Press any key to close the game.");
                    var key = Console.ReadKey();
                    return;
                }
                if (enemyCharacters.Count == 0)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);

                    Console.WriteLine("Victory!\n");

                    Console.Write(Environment.NewLine + Environment.NewLine);
                    
                    Console.WriteLine("Press any key to close the game.");
                    var key = Console.ReadKey();
                    return;
                } 
                
                WaitForNextTurnInput(StartTurn);
            }

            // Checks if a character is dead, and if so, removes it from your original list.
            void UpdateCharacters()
            {
                // Note: *for* is reversed, so we can modify the list from its last element to the first.
                
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

            // Sets all character into random locations and then draw battlefield.
            void AllocatePlayers()
            {
                foreach (var character in allPlayers)
                {
                    AllocateCharacter(character);
                }
                grid.DrawBattlefield();
            }
        }
    }
}
