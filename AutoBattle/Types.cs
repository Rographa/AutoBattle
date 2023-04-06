using System;
using System.Collections.Generic;
using System.Text;

namespace AutoBattle
{
    public static class Types
    {
        public struct CharacterClassSpecific
        {
            public CharacterClass CharacterClass;
            public float HpModifier;
            public float DamageModifier;
            public List<CharacterSkills> Skills;
            public List<Effect> BasicAttackEffects;
        }

        public struct GridBox
        {
            public readonly int XIndex;
            public readonly int YIndex;
            public readonly int Index;
            
            public Character OccupiedBy;
            public bool Occupied => OccupiedBy != null;
            public bool InGrid => _grid != null;
            private readonly Grid _grid;
            public GridBox(int x, int y, Character occupiedBy, int index, Grid grid)
            {
                XIndex = x;
                YIndex = y;
                this.OccupiedBy = occupiedBy;
                this.Index = index;
                _grid = grid;
                
            }
            public GridBox Left()
            {
                var index = Index;
                var grid = _grid;
                return grid != null ? grid.Grids.Find(box => box.Index == index - grid.YLength) : new GridBox();
            }
            public GridBox Right()
            {
                var index = Index;
                var grid = _grid;
                return grid != null ? _grid.Grids.Find(box => box.Index == index + grid.YLength) : new GridBox();
            }
            public GridBox Up()
            {
                var index = Index;
                var xIndex = XIndex;
                var grid = _grid;
                return grid != null ? _grid.Grids.Find(box => box.Index == index - 1 && box.XIndex == xIndex) : new GridBox();
            }
            public GridBox Down()
            {
                var index = Index;
                var xIndex = XIndex;
                var grid = _grid;
                return grid != null ? _grid.Grids.Find(box => box.Index == index + 1 && box.XIndex == xIndex) : new GridBox();
            }
        }

        public struct Effect
        {
            public string Name;
            public int Duration;
            public int Damage;
            public float Chance;
            public bool Stackable;
            public List<Conditions> ApplicableConditions;
        }

        public struct CharacterSkills
        {
            public string Name;
            public int MinDamage;
            public int MaxDamage;
            public float Chance;
            public SkillType SkillType;
            public SkillTarget SkillTarget;
            public List<Effect> Effects;
        }

        public struct CharacterCapabilities
        {
            public bool CanAttack;
            public bool CanMove;
            public bool CanCast;
            public bool IsStunned;
        }       

        public enum CharacterClass : uint
        {
            Paladin = 1,
            Warrior = 2,
            Cleric = 3,
            Archer = 4
        }

        public enum SkillType
        {
            MeleeAttack, RangedAttack, Support
        }

        public enum SkillTarget
        {
            EnemyTarget, Self, Ally, Area
        }

        public enum Conditions
        {
            None, Stun, Disarm, Cripple, Bleed, Silence
        }

        public static CharacterCapabilities DefaultCapabilities = new CharacterCapabilities()
        {
            CanAttack = true,
            CanMove = true,
            CanCast = true,
            IsStunned = false
        };

        #region Default Effects

        public static Effect StunEffect = new Effect()
        {
            Name = "Stun",
            Damage = 0,
            Duration = 1,
            Chance = 0.4f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Stun
            }
        };

        public static Effect DisarmEffect = new Effect()
        {
            Name = "Disarm",
            Damage = 0,
            Duration = 1,
            Chance = 0.5f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Disarm
            }
        };

        public static Effect CrippleEffect = new Effect()
        {
            Name = "Cripple",
            Damage = 2,
            Duration = 1,
            Chance = 0.7f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Cripple
            }
        };

        public static Effect BleedEffect = new Effect()
        {
            Name = "Bleed",
            Damage = 5,
            Duration = 3,
            Chance = 1f,
            Stackable = true,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Bleed
            }
        };

        public static Effect SilenceEffect = new Effect()
        {
            Name = "Silence",
            Damage = 0,
            Duration = 2,
            Chance = 0.8f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {      
                Conditions.Silence,
            }
        };

        #endregion
        #region Basic Attack Effects
        public static Effect BasicAttackDisarmEffect = new Effect()
        {
            Name = "Disarm",
            Damage = 0,
            Duration = 1,
            Chance = 0.2f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Disarm
            }
        };

        public static Effect BasicAttackCrippleEffect = new Effect()
        {
            Name = "Cripple",
            Damage = 2,
            Duration = 1,
            Chance = 0.15f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Cripple
            }
        };

        public static Effect BasicAttackBleedEffect = new Effect()
        {
            Name = "Bleed",
            Damage = 3,
            Duration = 2,
            Chance = 0.3f,
            Stackable = true,
            ApplicableConditions = new List<Conditions>()
            {
                Conditions.Bleed
            }
        };

        public static Effect BasicAttackSilenceEffect = new Effect()
        {
            Name = "Silence",
            Damage = 0,
            Duration = 2,
            Chance = 0.4f,
            Stackable = false,
            ApplicableConditions = new List<Conditions>()
            {      
                Conditions.Silence,
            }
        };
        #endregion
    }
}
