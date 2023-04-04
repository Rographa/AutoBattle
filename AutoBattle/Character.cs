using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Character
    {
        public string Name 
        {
            get
            {
                var index = CharacterIndex.ToString();                
                var id = CharacterClass switch
                {
                    CharacterClass.Paladin => "P",
                    CharacterClass.Warrior => "W",
                    CharacterClass.Cleric => "C",
                    CharacterClass.Archer => "A",
                    _ => "X",
                };

                id += index;
                return id;
            }
        }

        public string FullName
        {
            get
            {
                var prefix = IsPlayerCharacter ? "[Player] " : "[Enemy] ";
                var className = Enum.GetName(typeof(CharacterClass), this.CharacterClass);
                var order = $" ({CharacterIndex} | ";
                var hp = $"HP: {Health})";

                return prefix + className + order + hp;
            }
        }
        public float Health;
        public float BaseDamage;
        public CharacterClass CharacterClass;
        public float DamageMultiplier { get; set; }
        public GridBox currentBox;
        public int CharacterIndex;
        public bool IsPlayerCharacter;
        public bool IsDead;
        public Character Target { get; set; }

        private Grid battlefield;
        public Character(Grid grid, CharacterClass characterClass, bool isPlayerCharacter = false)
        {
            battlefield = grid;
            Health = 100;
            BaseDamage = 20;
            CharacterClass = characterClass;
            IsPlayerCharacter = isPlayerCharacter;
        }

        public void SetIndex(int index)
        {
            CharacterIndex = index;
        }

        public void Alocate(GridBox box)
        {
            box.occupiedBy = this;
            currentBox = box;
        }


        public bool TakeDamage(float amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        public void Die()
        {
            IsDead = true;
            var index = currentBox.Index;
            currentBox.occupiedBy = null;
            battlefield.grids[index] = currentBox;            
        }

        public void Move()
        {
            var nextBox = GetNextBox();

            currentBox.occupiedBy = null;
            battlefield.grids[currentBox.Index] = currentBox;

            currentBox = nextBox;
            currentBox.occupiedBy = this;
            battlefield.grids[currentBox.Index] = currentBox;

            battlefield.DrawBattlefield();
        }

        private GridBox GetNextBox()
        {
            var targetBox = Target.currentBox;
            var desiredIndex = 0;
            
            // Move Left
            if (currentBox.xIndex > targetBox.xIndex)
            {
                desiredIndex = currentBox.xIndex - 1;
                var position = GetPositionX(battlefield, desiredIndex);

                if (position.isValid)
                {
                    WriteFullName();
                    Console.WriteLine($" walked left.\n");
                    return position.box;
                }
            } 
            // Move Right
            else if (currentBox.xIndex < targetBox.xIndex)
            {
                desiredIndex = currentBox.xIndex + 1;
                var position = GetPositionX(battlefield, desiredIndex);

                if (position.isValid)
                {
                    WriteFullName();    
                    Console.WriteLine($" walked right.\n");
                    return position.box;
                }
            }

            // Move Up
            if (currentBox.yIndex > targetBox.yIndex)
            {
                desiredIndex = currentBox.yIndex - 1;
                var position = GetPositionY(battlefield, desiredIndex);

                if (position.isValid)
                {
                    WriteFullName();
                    Console.WriteLine($" walked up.\n");
                    return position.box;
                }
            } 
            // Move Down
            else if (currentBox.yIndex < targetBox.yIndex)
            {
                desiredIndex = currentBox.yIndex + 1;
                var position = GetPositionY(battlefield, desiredIndex);

                if (position.isValid)
                {
                    WriteFullName();
                    Console.WriteLine($" walked down.\n");
                    return position.box;
                }
            }

            return currentBox;
        }

        private (GridBox box, bool isValid) GetPositionX(Grid battlefield, int index)
        {
            if (!battlefield.grids.Exists(x => x.xIndex == index && x.yIndex == currentBox.yIndex)) return (currentBox, !currentBox.Occupied);

            var position = battlefield.grids.Find(x => x.xIndex == index && x.yIndex == currentBox.yIndex);
            return (position, !position.Occupied);
        }

        private (GridBox box, bool isValid) GetPositionY(Grid battlefield, int index)
        {
            if (!battlefield.grids.Exists(x => x.yIndex == index && x.xIndex == currentBox.xIndex)) return (currentBox, !currentBox.Occupied);

            var position = battlefield.grids.Find(x => x.yIndex == index && x.xIndex == currentBox.xIndex);
            return (position, !position.Occupied);
        }

        public void StartTurn()
        {
            if (IsDead) return;

            if (CheckCloseTargets(battlefield)) 
            {
                Attack(Target);
                return;
            }
            else
            {   // if there is no target close enough, calculates in wich direction this character should move to be closer to a possible target
                Move();                
            }
        }

        // Check in x and y directions if there is any character close enough to be a target.
        bool CheckCloseTargets(Grid battlefield)
        {
            bool left = (battlefield.grids.Find(x => x.Index == currentBox.Index - battlefield.yLength).Occupied);
            bool right = (battlefield.grids.Find(x => x.Index == currentBox.Index + battlefield.yLength).Occupied);
            bool up = (battlefield.grids.Find(x => x.Index == currentBox.Index - 1).Occupied);
            bool down = (battlefield.grids.Find(x => x.Index == currentBox.Index + 1).Occupied);

            if (left || right || up || down) 
            {
                return true;
            }
            return false; 
        }

        public void Attack (Character target)
        {
            var rand = new Random();
            var damage = rand.Next(0, (int)BaseDamage);
            var targetKilled = target.TakeDamage(damage);                        

            var description = targetKilled ? " just KILLED " : " is attacking ";
            var damageDescription = targetKilled ? $" with a final blow of {damage} damage!\n": $" and did {damage} damage.\n";

            WriteFullName();
            Console.Write(description);
            Target.WriteFullName();
            Console.Write(damageDescription);
            
        }

        private void WriteFullName() 
        {
            var consoleColor = IsPlayerCharacter ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = consoleColor;
            Console.Write(FullName);
            Console.ResetColor();
        }
    }
}
