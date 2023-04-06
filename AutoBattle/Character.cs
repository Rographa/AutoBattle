using System;
using System.Collections.Generic;
using System.Linq;
using static AutoBattle.Types;

namespace AutoBattle
{
    public class Character
    {
        private const int RangedAttackDistance = 2;

        public string Name 
        {
            get
            {
                var index = CharacterIndex.ToString();                
                var id = _characterClass switch
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
                var className = Enum.GetName(typeof(CharacterClass), this._characterClass);
                var order = $" ({CharacterIndex} | ";
                var hp = $"HP: {_health})";

                return prefix + className + order + hp;
            }
        }

        private int _health;
        private int _maxHealth;
        private int _baseDamage;
        private CharacterCapabilities _capabilities;
        private CharacterClass _characterClass;
        private float _damageMultiplier;
        
        public GridBox CurrentBox;
        public int CharacterIndex;
        public readonly bool IsPlayerCharacter;
        public bool IsDead;

        private List<CharacterSkills> _meleeAttackSkills = new List<CharacterSkills>();
        private List<CharacterSkills> _rangedAttackSkills = new List<CharacterSkills>();
        private List<CharacterSkills> _supportSkills = new List<CharacterSkills>();
        
        private List<Effect> _currentEffects = new List<Effect>();
        private List<Effect> _basicAttackEffects = new List<Effect>();
        
        private GridBox Left => _battlefield.Grids.Find(box => box.Index == CurrentBox.Index - _battlefield.YLength);
        private GridBox Right => _battlefield.Grids.Find(box => box.Index == CurrentBox.Index + _battlefield.YLength);
        private GridBox Up => _battlefield.Grids.Find(box => box.Index == CurrentBox.Index - 1 && box.XIndex == CurrentBox.XIndex);
        private GridBox Down => _battlefield.Grids.Find(box => box.Index == CurrentBox.Index + 1 && box.XIndex == CurrentBox.XIndex);

        private List<GridBox> Surroundings => new List<GridBox>() { Left, Right, Up, Down };
        public Character Target { get; set; }

        private readonly Grid _battlefield;
        public Character(Grid grid, CharacterClass characterClass, bool isPlayerCharacter = false)
        {
            _battlefield = grid;
            _health = 100;
            _baseDamage = 10;
            _characterClass = characterClass;
            IsPlayerCharacter = isPlayerCharacter;
            _capabilities = DefaultCapabilities;
            SetupClassData();
        }

        private void SetupClassData()
        {
            var classData = _characterClass switch
            {
                CharacterClass.Paladin => _paladinClass,
                CharacterClass.Warrior => _warriorClass,
                CharacterClass.Cleric => _clericClass,
                CharacterClass.Archer => _archerClass,
                _ => throw new NotImplementedException(),
            };

            _maxHealth = _health = (int)(_health * classData.HpModifier);
            _damageMultiplier = classData.DamageModifier;
            _meleeAttackSkills = classData.Skills.Where(skill => skill.SkillType == SkillType.MeleeAttack).ToList();
            _rangedAttackSkills = classData.Skills.Where(skill => skill.SkillType == SkillType.RangedAttack).ToList();
            _supportSkills = classData.Skills.Where(skill => skill.SkillType == SkillType.Support).ToList();
            _basicAttackEffects = classData.BasicAttackEffects;
        }

        public void UpdateClosestTarget(List<Character> enemies)
        {
            var leastDistance = double.PositiveInfinity;
            Character currentTarget = null;

            foreach (var enemy in enemies)
            {
                currentTarget ??= enemy;
                var distance = CalculateDistance(enemy);
                if (distance < leastDistance)
                {
                    leastDistance = distance;
                    currentTarget = enemy;
                }
            }

            Target = currentTarget;
        }

        private void ApplyEffects(Character target, List<Effect> effects)
        {
            foreach (var effect in effects.Where(effect => EvaluateChance(effect.Chance)))
            {
                var description = $" applied {effect.Name} to ";
                var postDescription = $" for {effect.Duration.ToString()} turn(s).\n";
                WriteAttackText(target, description, postDescription);
                target.AddEffect(effect);
            }
        }

        private void AddEffect(Effect effect)
        {
            if (effect.Stackable || _currentEffects.All(ef => ef.Name != effect.Name)) 
            {
                _currentEffects.Add(effect);
                return;
            }

            var existingEffect = _currentEffects.FirstOrDefault(ef => ef.Name == effect.Name);
            if (effect.Duration <= existingEffect.Duration) return;
            
            var index = _currentEffects.IndexOf(existingEffect);
             existingEffect.Duration = effect.Duration;
            _currentEffects[index] = existingEffect;
        }

        private bool TakeDamage(int amount)
        {
            _health -= amount;
            if (_health > 0) return false;
            
            Die();
            return true;
        }

        private void Heal(int amount)
        {
            _health = Math.Clamp(_health + amount, 1, _maxHealth);
        }

        private void Die()
        {
            IsDead = true;
            var index = CurrentBox.Index;
            CurrentBox.OccupiedBy = null;
            _battlefield.Grids[index] = CurrentBox;            
        }

        private void Move()
        {
            if (!_capabilities.CanMove)
            {                
                return;
            }

            var nextBox = GetNextBox();

            if (nextBox.Index == CurrentBox.Index) return;
            
            CurrentBox.OccupiedBy = null;
            _battlefield.Grids[CurrentBox.Index] = CurrentBox;

            CurrentBox = nextBox;
            CurrentBox.OccupiedBy = this;
            _battlefield.Grids[CurrentBox.Index] = CurrentBox;

            _battlefield.DrawBattlefield();
        }
        private GridBox GetNextBox()
        {
            var list = new List<GridBox>(Surroundings);
            list.RemoveAll(box => box.Occupied || !box.InGrid);

            var nextGridBox = CurrentBox;
            var leastDistance = Target.CalculateDistance(nextGridBox);
            foreach (var gridBox in list)
            {
                var distance = Target.CalculateDistance(gridBox);
                if (distance < leastDistance)
                {
                    leastDistance = distance;
                    nextGridBox = gridBox;
                }
            }

            var direction = " didn't move this turn.\n";
            if (nextGridBox.Index == Up.Index)
            {
                direction = $" walked up.\n";
            } else if (nextGridBox.Index == Down.Index)
            {
                direction = $" walked down.\n";
            } else if (nextGridBox.Index == Left.Index)
            {
                direction = $" walked left.\n";
            } else if (nextGridBox.Index == Right.Index)
            {
                direction = $" walked right.\n";
            }
            WriteFullName();
            Console.WriteLine(direction);
            return nextGridBox;
        }
        public void StartTurn()
        {
            if (IsDead) return;

            CheckEffects();
            if (IsDead || _capabilities.IsStunned) return;

            if (TryCastingSupportSkills()) return;
            if (CheckCloseTargets(_battlefield))
            {
                if (!TryCastingMeleeSkills()) Attack(Target);
                return;
            } 
            if (!TryCastingRangedSkills()) Move();
        }

        private bool HasCondition(Conditions condition)
        {
            return _currentEffects.Any(effect => effect.ApplicableConditions.Any(c => c == condition));
        }

        private void CheckEffects()
        {
            _capabilities.CanAttack = true;
            _capabilities.CanMove = true;
            _capabilities.CanCast = true;
            _capabilities.IsStunned = false;
            _currentEffects.RemoveAll((ef) =>
            {
                if (ef.Duration <= 0) 
                {
                    foreach (var condition in ef.ApplicableConditions)
                    {
                        if (HasCondition(condition)) continue;
                        switch (condition)
                        {
                            case Conditions.Stun:
                                WriteFullName();
                                Console.Write($" is no longer stunned.\n");                                
                                break;
                            case Conditions.Disarm:
                                WriteFullName();
                                Console.Write($" is no longer disarmed.\n");                                
                                break;
                            case Conditions.Cripple:
                                WriteFullName();
                                Console.Write($" is no longer crippled.\n");                                
                                break;
                            case Conditions.Silence:
                                WriteFullName();
                                Console.Write($" is no longer silenced.\n");                                
                                break;
                            case Conditions.Bleed:
                                WriteFullName();
                                Console.Write($" is no longer bleeding.\n");
                                break;
                            default:
                                break;
                        }
                    }
                    return true;
                }
                var index = _currentEffects.IndexOf(ef);
                ef.Duration--;
                _currentEffects[index] = ef;
                foreach (var condition in ef.ApplicableConditions)
                {
                    var killedByEffect = TakeDamage(ef.Damage);
                    switch (condition)
                    {
                        case Conditions.Stun:
                            WriteFullName();
                            Console.Write($" is stunned.\n");
                            _capabilities.IsStunned = true;
                            break;
                        case Conditions.Disarm:
                            WriteFullName();
                            Console.Write($" is disarmed.\n");
                            _capabilities.CanAttack = false;                            
                            break;
                        case Conditions.Cripple:
                            WriteFullName();
                            Console.Write(killedByEffect
                                ? $" died in pain... Poor one.\n"
                                : $" is crippled and took {ef.Damage} damage.\n");
                            _capabilities.CanMove = false;
                            break;
                        case Conditions.Silence:
                            WriteFullName();
                            Console.Write($" is silenced.\n");
                            _capabilities.CanCast = false;
                            break;                        
                        case Conditions.Bleed:
                            WriteFullName();
                            Console.Write(killedByEffect
                                ? $" bled to death.\n"
                                : $" is bleeding and took {ef.Damage} damage.\n");
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
            if (_meleeAttackSkills.Count == 0) return false;

            if (!_capabilities.CanCast)
            {                
                return false;
            }            

            var rand = new Random();
            var randomSkill = _meleeAttackSkills[rand.Next(0, _meleeAttackSkills.Count)];

            if (!EvaluateChance(randomSkill.Chance)) return false;

            var targets = new List<Character>();
            switch (randomSkill.SkillTarget)
            {
                case SkillTarget.EnemyTarget:
                    targets.Add(Target);
                    break;                
                case SkillTarget.Area:
                    targets.AddRange(GetCharactersAround());
                    break;
                default:
                    break;
            }
            if (targets.Count == 0) return false;

            ExecuteSkill(randomSkill, targets);
            return true;
        }

        private bool TryCastingRangedSkills()
        {
            if (_rangedAttackSkills.Count == 0) return false;

            if (!_capabilities.CanCast)
            {                
                return false;
            }            

            var rand = new Random();
            var randomSkill = _rangedAttackSkills[rand.Next(0, _rangedAttackSkills.Count)];

            if (!EvaluateChance(randomSkill.Chance)) return false;

            var targets = new List<Character>();
            var targetInRange = ((int)Math.Round(CalculateDistance(Target))) <= RangedAttackDistance;
            if (!targetInRange) return false;

            switch (randomSkill.SkillTarget)
            {
                case SkillTarget.EnemyTarget:
                    targets.Add(Target);
                    break;
                case SkillTarget.Area:
                    targets.Add(Target);
                    targets.AddRange(Target.GetCharactersAround());
                    break;
                default:
                    break;
            }

            if (targets.Count == 0) return false;

            ExecuteSkill(randomSkill, targets);
            return true;
        }

        private bool TryCastingSupportSkills()
        {
            if (_supportSkills.Count == 0) return false;

            if (!_capabilities.CanCast)
            {                
                return false;
            }

            var rand = new Random();
            var randomSkill = _supportSkills[rand.Next(0, _supportSkills.Count)];

            if (!EvaluateChance(randomSkill.Chance)) return false;

            var targets = new List<Character>();
            var allies = GetAlliesAround();
            switch (randomSkill.SkillTarget)
            {
                case SkillTarget.Self:
                    targets.Add(this);
                    break;
                case SkillTarget.Ally:
                    if (allies.Count > 0)
                        targets.Add(allies[rand.Next(0, allies.Count)]);
                    break;
                case SkillTarget.Area:
                    if (allies.Count > 0)
                        targets.AddRange(allies);
                    break;
                default:
                    break;
            }

            if (targets.Count == 0) return false;

            ExecuteSkill(randomSkill, targets);
            return true;
        }

        private void ExecuteSkill(CharacterSkills skill, List<Character> targets)
        {
            var rand = new Random();
            foreach (var target in targets)
            {
                var damage = (int)Math.Round(rand.Next(skill.MinDamage, skill.MaxDamage) * _damageMultiplier);
                string description;
                string damageDescription;
                switch (skill.SkillType)
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
                if (!target.IsDead) ApplyEffects(target, skill.Effects);
            }
        }

        private List<Character> GetCharactersAround()
        {
            var list = new List<Character>
            {
                Left.OccupiedBy,
                Right.OccupiedBy,
                Up.OccupiedBy,
                Down.OccupiedBy,
                Left.Up().OccupiedBy,
                Left.Down().OccupiedBy,
                Right.Up().OccupiedBy,
                Right.Down().OccupiedBy
            };

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var element = list[i];
                if (element == null)
                    list.RemoveAt(i);
            }

            return list;
        }

        private List<Character> GetAlliesAround()
        {
            var list = new List<Character>
            {
                Left.OccupiedBy,
                Right.OccupiedBy,
                Up.OccupiedBy,
                Down.OccupiedBy,
                Left.Up().OccupiedBy,
                Left.Down().OccupiedBy,
                Right.Up().OccupiedBy,
                Right.Down().OccupiedBy
            };

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var element = list[i];
                if (element == null || element.IsPlayerCharacter != IsPlayerCharacter)
                    list.RemoveAt(i);
            }

            return list;
        }

        // Check in x and y directions if there is any character close enough to be a target.
        private bool CheckCloseTargets(Grid battlefield)
        {
            var list = new List<GridBox>(Surroundings);
            list.RemoveAll(box => !box.InGrid);
            return list.Any(box => box.OccupiedBy == Target);
        }

        private void Attack (Character target)
        {
            if (!_capabilities.CanAttack)
            {                
                return;
            }
            var rand = new Random();
            var damage = (int)Math.Round(rand.Next(0, (int)_baseDamage) * _damageMultiplier);
            var targetKilled = target.TakeDamage(damage);                        

            var description = targetKilled ? " just KILLED " : " is attacking ";
            var damageDescription = targetKilled ? $" with a final blow of {damage} damage!\n": $" and dealt {damage} damage.\n";
            
            WriteAttackText(target, description, damageDescription);
            if (!targetKilled) ApplyEffects(target, _basicAttackEffects);
        }

        private void WriteAttackText(Character target, string description, string damageDescription)
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
        
        private static bool EvaluateChance(float chance)
        {
            var rand = new Random();
            return chance >= rand.NextDouble();
        }

        private (GridBox box, bool isValid) GetPositionX(Grid battlefield, int index)
        {
            if (!battlefield.Grids.Exists(x => x.XIndex == index && x.YIndex == CurrentBox.YIndex)) return (CurrentBox, !CurrentBox.Occupied);

            var position = battlefield.Grids.Find(x => x.XIndex == index && x.YIndex == CurrentBox.YIndex);
            return (position, !position.Occupied);
        }

        private (GridBox box, bool isValid) GetPositionY(Grid battlefield, int index)
        {
            if (!battlefield.Grids.Exists(x => x.YIndex == index && x.XIndex == CurrentBox.XIndex)) return (CurrentBox, !CurrentBox.Occupied);

            var position = battlefield.Grids.Find(x => x.YIndex == index && x.XIndex == CurrentBox.XIndex);
            return (position, !position.Occupied);
        }

        public double CalculateDistance(Character other)
        {
            var x = CurrentBox.XIndex;
            var y = CurrentBox.YIndex;
            var otherX = other.CurrentBox.XIndex;
            var otherY = other.CurrentBox.YIndex;
            var distance = Math.Sqrt(Math.Pow(otherX - x, 2) + Math.Pow(otherY - y, 2));
            return distance;
        }

        public double CalculateDistance(GridBox box)
        {
            var x = CurrentBox.XIndex;
            var y = CurrentBox.YIndex;
            var otherX = box.XIndex;
            var otherY = box.YIndex;
            var distance = Math.Sqrt(Math.Pow(otherX - x, 2) + Math.Pow(otherY - y, 2));
            return distance;
        }

        private static CharacterClassSpecific _paladinClass;
        private static CharacterClassSpecific _warriorClass;
        private static CharacterClassSpecific _clericClass;
        private static CharacterClassSpecific _archerClass;

        private static List<CharacterSkills> _paladinSkills;
        private static List<CharacterSkills> _warriorSkills;
        private static List<CharacterSkills> _clericSkills;
        private static List<CharacterSkills> _archerSkills;       

        public static void SetupCharacterClasses()
        {
            _paladinSkills = new List<CharacterSkills>()
            {
                new CharacterSkills()
                {
                    Name = "Holy Wrath",
                    MinDamage = 12,
                    MaxDamage = 20,
                    Chance = 0.2f,
                    SkillType = SkillType.MeleeAttack,
                    SkillTarget = SkillTarget.EnemyTarget,
                    Effects = new List<Effect>()
                    {
                        StunEffect
                    }
                },
                new CharacterSkills()
                {
                    Name = "Healing Nova",
                    MinDamage = 8,
                    MaxDamage = 14,
                    Chance = 0.3f,
                    SkillType = SkillType.Support,
                    SkillTarget = SkillTarget.Area,
                    Effects = new List<Effect>()                    
                }
            };

            _warriorSkills = new List<CharacterSkills>()
            {
                new CharacterSkills()
                {
                    Name = "Heroic Strike",
                    MinDamage = 12,
                    MaxDamage = 24,
                    Chance = 0.5f,
                    SkillType = SkillType.MeleeAttack,
                    SkillTarget = SkillTarget.EnemyTarget,
                    Effects = new List<Effect>()
                    {
                        BleedEffect, DisarmEffect
                    }
                },
                new CharacterSkills()
                {
                    Name = "Whirlwind",
                    MinDamage = 10,
                    MaxDamage = 16,
                    Chance = 0.4f,
                    SkillType = SkillType.MeleeAttack,
                    SkillTarget = SkillTarget.Area,
                    Effects = new List<Effect>()
                    {
                        BleedEffect
                    }
                }
            };

            _clericSkills = new List<CharacterSkills>()
            {
                new CharacterSkills
                {
                    Name = "Healing Hands",
                    MinDamage = 8,
                    MaxDamage = 15,
                    Chance = 1f,
                    SkillType = SkillType.Support,
                    SkillTarget = SkillTarget.Ally,
                    Effects= new List<Effect>()                    
                },
                new CharacterSkills
                {
                    Name = "Divine Blast",
                    MinDamage = 5,
                    MaxDamage = 25,
                    Chance = 0.5f,
                    SkillType = SkillType.RangedAttack,
                    SkillTarget = SkillTarget.EnemyTarget,
                    Effects= new List<Effect>()
                    {
                        SilenceEffect
                    }
                }
            };

            _archerSkills = new List<CharacterSkills>()
            {
                new CharacterSkills
                {
                    Name = "Crippling Shot",
                    MinDamage = 9,
                    MaxDamage = 14,
                    Chance = 0.8f,
                    SkillType = SkillType.RangedAttack,
                    SkillTarget = SkillTarget.EnemyTarget,
                    Effects= new List<Effect>()
                    {
                        CrippleEffect
                    }
                },
                new CharacterSkills
                {
                    Name = "Aimed Shot",
                    MinDamage = 15,
                    MaxDamage = 20,
                    Chance = 1f,
                    SkillType = SkillType.RangedAttack,
                    SkillTarget = SkillTarget.EnemyTarget,
                    Effects= new List<Effect>()
                    {
                        BleedEffect
                    }
                }
            };

            _paladinClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Paladin,
                HpModifier = 1.5f,
                DamageModifier = 1.2f,
                Skills = new List<CharacterSkills>(_paladinSkills.ToList()),
                BasicAttackEffects =  new List<Effect>() { BasicAttackDisarmEffect }
            };

            _warriorClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Warrior,
                HpModifier = 1.3f,
                DamageModifier = 1.5f,
                Skills = new List<CharacterSkills>(_warriorSkills.ToList()),
                BasicAttackEffects =  new List<Effect>() { BasicAttackBleedEffect }
            };

            _clericClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Cleric,
                HpModifier = 1f,
                DamageModifier = 1.4f,
                Skills = new List<CharacterSkills>(_clericSkills.ToList()),
                BasicAttackEffects =  new List<Effect>() { BasicAttackSilenceEffect }
            };

            _archerClass = new CharacterClassSpecific()
            {
                CharacterClass = CharacterClass.Archer,
                HpModifier = 1f,
                DamageModifier = 1.6f,
                Skills = new List<CharacterSkills>(_archerSkills.ToList()),
                BasicAttackEffects =  new List<Effect>() { BasicAttackCrippleEffect }
            };
        }

    }
}
