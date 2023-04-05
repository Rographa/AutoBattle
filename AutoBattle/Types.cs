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
            public List<AppliableConditions> AppliableConditions;
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

        public struct AppliableConditions
        {
            public Conditions Condition;
            public float Chance;
        }

        public struct CharacterCapabilities
        {
            public bool CanAttack;
            public bool CanMove;
            public bool CanCast;
        }

        public static CharacterCapabilities DefaultCapabilities = new CharacterCapabilities()
        {
            CanAttack = true,
            CanMove = true,
            CanCast = true
        };

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
            EnemyTarget, Self, Ally 
        }

        public enum Conditions
        {
            None, Stun, Disarm, Cripple, Bleed, Silence
        }

    }
}
