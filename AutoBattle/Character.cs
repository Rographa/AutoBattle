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
                var id = IsPlayerCharacter ? $"P{index}" : $"E{index}";
                return id;
            }
        }
        public float Health;
        public float BaseDamage;
        public CharacterClass CharacterClass;
        public float DamageMultiplier { get; set; }
        public GridBox currentBox;
        public int CharacterIndex;
        public bool IsPlayerCharacter;
        public Character Target { get; set; } 
        public Character(CharacterClass characterClass, bool isPlayerCharacter = false)
        {
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
            if((Health -= amount) <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        public void Die()
        {
            //TODO >> maybe kill him?
        }

        public void WalkTO(bool CanWalk)
        {

        }

        public void Move(Grid battlefield)
        {
            var nextBox = GetNextBox(battlefield);

            currentBox.occupiedBy = null;
            battlefield.grids[currentBox.Index] = currentBox;

            currentBox = nextBox;
            currentBox.occupiedBy = this;
            battlefield.grids[currentBox.Index] = currentBox;

            battlefield.DrawBattlefield();
        }

        private GridBox GetNextBox(Grid battlefield)
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
                    Console.WriteLine($"{Name} walked left\n");
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
                    Console.WriteLine($"{Name} walked right\n");
                    return position.box;
                }
            }

            if (currentBox.yIndex > targetBox.yIndex)
            {
                desiredIndex = currentBox.yIndex - 1;
                var position = GetPositionY(battlefield, desiredIndex);

                if (position.isValid)
                {
                    Console.WriteLine($"{Name} walked up\n");
                    return position.box;
                }
            } else if (currentBox.yIndex < targetBox.yIndex)
            {
                desiredIndex = currentBox.yIndex + 1;
                var position = GetPositionY(battlefield, desiredIndex);

                if (position.isValid)
                {
                    Console.WriteLine($"{Name} walked down\n");
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

        public void StartTurn(Grid battlefield)
        {

            if (CheckCloseTargets(battlefield)) 
            {
                Attack(Target);
                

                return;
            }
            else
            {   // if there is no target close enough, calculates in wich direction this character should move to be closer to a possible target
                Move(battlefield);                
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
            target.TakeDamage(damage);            
            Console.WriteLine($"{Name} is attacking the player {Target.Name} and did {damage} damage\n");
            
        }
    }
}
