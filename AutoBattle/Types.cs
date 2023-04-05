using System;
using System.Collections.Generic;
using System.Text;

namespace AutoBattle
{
    public class Types
    {

        public struct CharacterClassSpecific
        {
            public CharacterClass CharacterClass;
            public float hpModifier;
            public float damageModifier;
            public List<CharacterSkills> skills;
        }

        public struct GridBox
        {
            public int xIndex;
            public int yIndex;
            public Character occupiedBy;
            public bool Occupied => occupiedBy != null;
            public int Index;

            public GridBox(int x, int y, Character occupiedBy, int index)
            {
                xIndex = x;
                yIndex = y;
                this.occupiedBy = occupiedBy;
                this.Index = index;
            }

        }

        public struct Effect
        {
            public string Name;
            public int Duration;
            public int Damage;
            public float Chance;
            public List<Conditions> AppliableConditions;
        }

        public struct CharacterSkills
        {
            public string Name;
            public int minDamage;
            public int maxDamage;
            public float chance;
            public SkillType skillType;
            public SkillTarget skillTarget;
            public List<Effect> effects;
        }

        public struct CharacterCapabilities
        {
            public bool CanAttack;
            public bool CanMove;
            public bool CanCast;
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
            CanCast = true
        };

        #region Default Effects

        public static Effect StunEffect = new Effect()
        {
            Name = "Stun",
            Damage = 0,
            Duration = 1,
            Chance = 0.4f,
            AppliableConditions = new List<Conditions>()
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
            AppliableConditions = new List<Conditions>()
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
            AppliableConditions = new List<Conditions>()
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
            AppliableConditions = new List<Conditions>()
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
            AppliableConditions = new List<Conditions>()
            {      
                Conditions.Silence,
            }
        };

        #endregion
    }
}
