using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using static AutoBattle.Types;
using System.Collections.ObjectModel;

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
        public int Health;
        public int MaxHealth;
        public int BaseDamage;
        public CharacterCapabilities Capabilities;
        public CharacterClass CharacterClass;
        public float DamageMultiplier { get; set; }
        public GridBox currentBox;
        public int CharacterIndex;
        public bool IsPlayerCharacter;
        public bool IsDead;

        public List<CharacterSkills> MeleeAttackSkills = new List<CharacterSkills>();
        public List<CharacterSkills> RangedAttackSkills = new List<CharacterSkills>();
        public List<CharacterSkills> SupportSkills = new List<CharacterSkills>();

        public List<Effect> CurrentEffects = new List<Effect>();

        public Character Target { get; set; }

        private Grid battlefield;
        public Character(Grid grid, CharacterClass characterClass, bool isPlayerCharacter = false)
        {
            battlefield = grid;
            Health = 100;
            BaseDamage = 10;
            CharacterClass = characterClass;
            IsPlayerCharacter = isPlayerCharacter;
            Capabilities = DefaultCapabilities;
            SetupClassData();
        }

        private void SetupClassData()
        {
            var classData = CharacterClass switch
            {
                CharacterClass.Paladin => PaladinClass,
                CharacterClass.Warrior => WarriorClass,
                CharacterClass.Cleric => ClericClass,
                CharacterClass.Archer => ArcherClass,
                _ => throw new NotImplementedException(),
            };

            MaxHealth = Health = (int)(Health * classData.hpModifier);
            DamageMultiplier = classData.damageModifier;
            MeleeAttackSkills = classData.skills.Where(skill => skill.skillType == SkillType.MeleeAttack).ToList();
            RangedAttackSkills = classData.skills.Where(skill => skill.skillType == SkillType.RangedAttack).ToList();
            SupportSkills = classData.skills.Where(skill => skill.skillType == SkillType.Support).ToList();

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

        public void ApplyEffects(Character target, List<Effect> effects)
        {            
            foreach (Effect effect in effects)
            {
                if (!EvaluateChance(effect.Chance)) continue;
                target.AddEffect(effect);
            }
        }

        public void AddEffect(Effect effect)
        {
            CurrentEffects.Add(effect);
            return;

            if (!CurrentEffects.Any(ef => ef.Name == effect.Name)) 
            {
                
            }

            var existingEffect = CurrentEffects.FirstOrDefault(ef => ef.Name == effect.Name);

        }

        public bool TakeDamage(int amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        public void Heal(int amount)
        {
            Health = Math.Clamp(Health + amount, 1, MaxHealth);
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

        public double CalculateDistance(Character other)
        {
            var boxIndex = currentBox.Index;
            var enemyBoxIndex = other.currentBox.Index;

            var x = boxIndex / battlefield.yLength;
            var y = boxIndex % battlefield.yLength;

            var otherX = enemyBoxIndex / battlefield.yLength;
            var otherY = enemyBoxIndex % battlefield.yLength;

            return Math.Sqrt(Math.Pow(otherX - x, 2) + Math.Pow(otherY - y, 2));
        }

        public void StartTurn()
        {
            if (IsDead) return;

            CheckEffects();

            if (IsDead) return;
            if (CheckCloseTargets(battlefield)) 
            {
                if (!TryCastingMeleeSkills()) Attack(Target);
                return;
            }
            else
            {   // if there is no target close enough, calculates in wich direction this character should move to be closer to a possible target
                Move();                
            }
        }

        private void CheckEffects()
        {
            Capabilities.CanAttack = true;
            Capabilities.CanMove = true;
            Capabilities.CanCast = true;

            CurrentEffects.RemoveAll((ef) =>
            {
                if (ef.Duration == 0) return true;

                ef.Duration--;
                foreach (var condition in ef.AppliableConditions)
                {
                    switch (condition)
                    {
                        case Conditions.Stun:
                            WriteFullName();
                            Console.Write($" is stunned.\n");
                            Capabilities.CanAttack = false;
                            Capabilities.CanCast = false;
                            Capabilities.CanMove = false;
                            break;
                        case Conditions.Disarm:
                            WriteFullName();
                            Console.Write($" is disarmed.\n");
                            Capabilities.CanAttack = false;                            
                            break;
                        case Conditions.Cripple:
                            WriteFullName();
                            Console.Write($" is crippled.\n");
                            Capabilities.CanMove = false;
                            break;
                        case Conditions.Silence:
                            WriteFullName();
                            Console.Write($" is silenced.\n");
                            Capabilities.CanCast = false;
                            break;                        
                        case Conditions.Bleed:

                            WriteFullName();
                            if (TakeDamage(ef.Damage)) Console.Write($" bled to death.\n");
                            else Console.Write($" is bleeding and took {ef.Damage} damage.\n");
                            
                            break;
                        default:
                            break;
                    }
                }

                return false;
            });
        }

        private bool TryCastingMeleeSkills()
        {
            if (MeleeAttackSkills.Count == 0) return false;

            var rand = new Random();
            var randomSkill = MeleeAttackSkills[rand.Next(0, MeleeAttackSkills.Count)];

            if (!EvaluateChance(randomSkill.chance)) return false;

            List<Character> targets = new List<Character>();
            switch (randomSkill.skillTarget)
            {
                case SkillTarget.EnemyTarget:
                    targets.Add(Target);
                    break;                
                case SkillTarget.Area:
                    targets.AddRange(GetEnemiesAround());
                    break;
                default:
                    break;
            }

            ExecuteSkill(randomSkill, targets);
            return true;
        }

        private void ExecuteSkill(CharacterSkills skill, List<Character> targets)
        {
            var rand = new Random();
            string description;
            string damageDescription;
            foreach (var target in targets)
            {
                int damage = (int)Math.Round(rand.Next(skill.minDamage, skill.maxDamage) * DamageMultiplier);

                switch (skill.skillType)
                {
                    case SkillType.MeleeAttack:
                    case SkillType.RangedAttack:
                        if (target.TakeDamage(damage))
                        {
                            description = $" used his skill, {skill.Name} on ";
                            damageDescription = $" and it turned him to ashes. He's gone.\n";
                        }
                        else
                        {
                            description = $" used his skill, {skill.Name} on ";
                            damageDescription = $" dealing {damage} damage.\n";
                        }
                        break;                    
                    case SkillType.Support:
                        target.Heal(damage);                        
                        description = $" used his skill, {skill.Name} on ";
                        damageDescription = $" and healed him in {damage} health points.\n";                        
                        break;
                    default:
                        continue;
                }                

                WriteAttackText(target, description, damageDescription);
                ApplyEffects(target, skill.effects);
            }
        }

        private bool EvaluateChance(float chance)
        {
            var rand = new Random();
            return chance >= rand.NextDouble();
        }

        private List<Character> GetEnemiesAround()
        {
            var list = new List<Character>
            {
                battlefield.grids.Find(box => box.Index == currentBox.Index - battlefield.yLength).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index + battlefield.yLength).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index - 1).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index + 1).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index - battlefield.yLength - 1).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index - battlefield.yLength + 1).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index + battlefield.yLength - 1).occupiedBy,
                battlefield.grids.Find(box => box.Index == currentBox.Index + battlefield.yLength + 1).occupiedBy,
            };

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var element = list[i];
                if (element == null)
                    list.RemoveAt(i);
            }

            return list;
        }

        // Check in x and y directions if there is any character close enough to be a target.
        bool CheckCloseTargets(Grid battlefield)
        {
            var list = new List<GridBox>();

            var leftGridBox = battlefield.grids.Find(box => box.Index == currentBox.Index - battlefield.yLength);
            var rightGridBox = battlefield.grids.Find(box => box.Index == currentBox.Index + battlefield.yLength);
            var upGridBox = battlefield.grids.Find(box => box.Index == currentBox.Index - 1 && box.xIndex == currentBox.xIndex);
            var downGridBox = battlefield.grids.Find(box => box.Index == currentBox.Index + 1 && box.xIndex == currentBox.xIndex);

            list.Add(leftGridBox);
            list.Add(rightGridBox);
            list.Add(upGridBox);
            list.Add(downGridBox);

            if (list.Any(box => box.occupiedBy == Target))
                return true;            
            return false; 
        }

        public void Attack (Character target)
        {
            var rand = new Random();
            var damage = (int)Math.Round(rand.Next(0, (int)BaseDamage) * DamageMultiplier);
            var targetKilled = target.TakeDamage(damage);                        

            var description = targetKilled ? " just KILLED " : " is attacking ";
            var damageDescription = targetKilled ? $" with a final blow of {damage} damage!\n": $" and did {damage} damage.\n";

            WriteAttackText(target, description, damageDescription);
            
        }

        public void WriteAttackText(Character target, string description, string damageDescription)
        {
            WriteFullName();
            Console.Write(description);
            target.WriteFullName();
            Console.Write(damageDescription);
        }

        public void WriteFullName() 
        {
            var consoleColor = IsPlayerCharacter ? ConsoleColor.Green : ConsoleColor.Red;
            Console.ForegroundColor = consoleColor;
            Console.Write(FullName);
            Console.ResetColor();
        }

        public static CharacterClassSpecific PaladinClass;
        public static CharacterClassSpecific WarriorClass;
        public static CharacterClassSpecific ClericClass;
        public static CharacterClassSpecific ArcherClass;

        public static List<CharacterSkills> PaladinSkills;
        public static List<CharacterSkills> WarriorSkills;
        public static List<CharacterSkills> ClericSkills;
        public static List<CharacterSkills> ArcherSkills;       

        public static void SetupCharacterClasses()
        {
            PaladinSkills = new List<CharacterSkills>()
            {
                new CharacterSkills()
                {
                    Name = "Holy Wrath",
                    minDamage = 12,
                    maxDamage = 20,
                    chance = 0.2f,
                    skillType = SkillType.MeleeAttack,
                    skillTarget = SkillTarget.EnemyTarget,
                    effects = new List<Effect>()
                    {
                        StunEffect
                    }
                },
                new CharacterSkills()
                {
                    Name = "Healing Nova",
                    minDamage = 8,
                    maxDamage = 14,
                    chance = 0.3f,
                    skillType = SkillType.Support,
                    skillTarget = SkillTarget.Area,
                    effects = new List<Effect>()                    
                }
            };

            WarriorSkills = new List<CharacterSkills>()
            {
                new CharacterSkills()
                {
                    Name = "Heroic Strike",
                    minDamage = 12,
                    maxDamage = 24,
                    chance = 0.5f,
                    skillType = SkillType.MeleeAttack,
                    skillTarget = SkillTarget.EnemyTarget,
                    effects = new List<Effect>()
                    {
                        BleedEffect, DisarmEffect
                    }
                },
                new CharacterSkills()
                {
                    Name = "Whirlwind",
                    minDamage = 10,
                    maxDamage = 16,
                    chance = 0.4f,
                    skillType = SkillType.MeleeAttack,
                    skillTarget = SkillTarget.Area,
                    effects = new List<Effect>()
                    {
                        BleedEffect
                    }
                }
            };

            ClericSkills = new List<CharacterSkills>()
            {
                new CharacterSkills
                {
                    Name = "Healing Hands",
                    minDamage = 8,
                    maxDamage = 15,
                    chance = 1f,
                    skillType = SkillType.Support,
                    skillTarget = SkillTarget.Ally,
                    effects= new List<Effect>()                    
                },
                new CharacterSkills
                {
                    Name = "Divine Blast",
                    minDamage = 5,
                    maxDamage = 25,
                    chance = 0.5f,
                    skillType = SkillType.RangedAttack,
                    skillTarget = SkillTarget.EnemyTarget,
                    effects= new List<Effect>()
                    {
                        SilenceEffect
                    }
                }
            };

            ArcherSkills = new List<CharacterSkills>()
            {
                new CharacterSkills
                {
                    Name = "Crippling Shot",
                    minDamage = 9,
                    maxDamage = 14,
                    chance = 0.8f,
                    skillType = SkillType.RangedAttack,
                    skillTarget = SkillTarget.EnemyTarget,
                    effects= new List<Effect>()
                    {
                        CrippleEffect
                    }
                },
                new CharacterSkills
                {
                    Name = "Aimed Shot",
                    minDamage = 15,
                    maxDamage = 20,
                    chance = 1f,
                    skillType = SkillType.RangedAttack,
                    skillTarget = SkillTarget.EnemyTarget,
                    effects= new List<Effect>()
                    {
                        BleedEffect
                    }
                }
            };

            PaladinClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Paladin,
                hpModifier = 1.5f,
                damageModifier = 1.2f,
                skills = new List<CharacterSkills>(PaladinSkills.ToList())
            };

            WarriorClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Warrior,
                hpModifier = 1.3f,
                damageModifier = 1.5f,
                skills = new List<CharacterSkills>(WarriorSkills.ToList())
            };

            ClericClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Cleric,
                hpModifier = 1f,
                damageModifier = 1.4f,
                skills = new List<CharacterSkills>(ClericSkills.ToList())
            };

            ArcherClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Archer,
                hpModifier = 1f,
                damageModifier = 1.6f,
                skills = new List<CharacterSkills>(ArcherSkills.ToList())
            };
        }

    }
}
