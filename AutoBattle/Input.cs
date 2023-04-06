using System;

namespace AutoBattle
{
    public static class Input
    {
        public static (int x, int y) GetBattlefieldSize()
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

                return (width, height);
            }
        }
        
        public static int GetBattleChoice()
        {
            while (true)
            {
                Console.WriteLine("Choose a type of battle:\n");
                Console.WriteLine("[1] 1x1, [2] 2x2, [3] 3x3, [4] 4x4");
                var battleChoice = Console.ReadLine();

                switch (battleChoice)
                {
                    case "1":
                        return 1;
                    case "2":
                        return 2;
                    case "3":
                        return 3;
                    case "4":
                        return 4;
                    default:
                        continue;
                }
            }
        }

        public static void GetPlayerCharacter(Action<int> onCharacterSelectedCallback)
        {
            while (true)
            {
                //asks for the player to choose between for possible classes via console.
                Console.WriteLine("Choose Between One of this Classes:\n");
                Console.WriteLine("[1] Paladin, [2] Warrior, [3] Cleric, [4] Archer");
                //store the player choice in a variable
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        onCharacterSelectedCallback?.Invoke(1);
                        break;
                    case "2":
                        onCharacterSelectedCallback?.Invoke(2);
                        break;
                    case "3":
                        onCharacterSelectedCallback?.Invoke(3);
                        break;
                    case "4":
                        onCharacterSelectedCallback?.Invoke(4);
                        break;
                    default:
                        continue;
                }
                break;
            }
        }
        
        public static void StartBattleInput(Action callback)
        {
            Console.Write(Environment.NewLine + Environment.NewLine);
            Console.WriteLine("Press any key to start the battle!\n");
            Console.Write(Environment.NewLine + Environment.NewLine);
            var key = Console.ReadKey();
            callback?.Invoke();
        }

        public static void WaitForNextTurnInput(Action callback)
        {
            Console.Write(Environment.NewLine + Environment.NewLine);
            Console.WriteLine("Click on any key to start the next turn...\n");
            Console.Write(Environment.NewLine + Environment.NewLine);

            var key = Console.ReadKey();
            callback?.Invoke();
        }
    }
}