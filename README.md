# AutoBattle

## Rules:
- Player must select a battlefield size. _(e.g. 5x5, 10x8)_
- Player must select a battle type. _(e.g. 1vs1, 2vs2)_
- Player must select a class for each of him/her/their characters.
- Opponent will randomly select a class for each character as well.
- Turn order will be random among all characters and it is set at the beginning of the game.
- Each turn, every character will try to perform up to one action. (Moving, Attacking and Casting Spells are actions.)
- Player team will automatically fight enemy team until one of those are defeated.

## Actions (in priority order):
- ### Cast Support Spells:
  Character will randomly select a skill amongst all of its Support type skills and evaluate the chance to execute it, if conditions are valid. Otherwise, it will skip and try next action.
- ### Cast Melee Spells:
  Character will randomly select a skill amongst all of its Melee type skills and evaluate the chance to execute it, if conditions are valid. Otherwise, it will skip and try next action.
- ### Attack:
  Character will try to attack its target. A target is only valid to be attacked by Basic Attack if it is in any of character's surroundings (up, down, left, right).
- ### Cast Ranged Spells:
  Character will randomly select a skill amongst all of its Ranged type skills and evaluate the chance to execute it, if conditions are valid. Otherwise, it will skip and try next action.
- ### Move:
  Character will try to move to one of its cardinal boxes (up, down, left, right) where its distance from target enemy is smaller.

_Note: if Character couldn't perform any of its actions, its turn will be skipped._

## Classes:
- ### **Paladin**
  - **Health:** 150
  - **Damage Multiplier:** 120%
  - **Basic Attack Effect:** Disarm (20%, 1t)
  - **Skills:**
    - **Holy Wrath** _(Melee Attack | Enemy Target)_:
      - Damage : 12-20
      - Chance : 20%
      - Effects : Stun (40%, 1t)
    - **Healing Nova** _(Support | Area)_:
      - Damage : 8-14
      - Chance : 30%
      - Effects : None      
- ### **Warrior**:
  - **Health:** 130
  - **Damage Multiplier:** 150%
  - **Basic Attack Effect:** Bleed (30%, 2t, 3 DMG)
  - **Skills:**
    - **Heroic Strike** _(Melee Attack | Enemy Target)_:
      - Damage : 12-24
      - Chance : 50%
      - Effects : Bleed (100%, 3t, 5 DMG), Disarm (50%, 1t)
    - **Whirlwind** _(Melee Attack | Area)_:
      - Damage : 10-16
      - Chance : 40%
      - Effects : Bleed (100%, 3t, 5 DMG)
- ### **Cleric**:
  - **Health:** 100
  - **Damage Multiplier:** 140%
  - **Basic Attack Effect:** Silence (40%, 2t)
  - **Skills:**
    - **Healing Hands** _(Support | Ally)_:
      - Damage : 8-15
      - Chance : 100%
      - Effects : None
    - **Divine Blast** _(Ranged Attack | Enemy Target)_:
      - Damage : 5-25
      - Chance : 50%
      - Effects : Silence (80%, 2t)
- ### **Archer**:
  - **Health:** 100
  - **Damage Multiplier:** 160%
  - **Basic Attack Effect:** Cripple (15%, 1t, 2 DMG)
  - **Skills:**
    - **Crippling Shot** _(Ranged Attack | Enemy Target)_:
      - Damage : 9-14
      - Chance : 80%
      - Effects : Cripple (70%, 1t, 2 DMG)
    - **Aimed Shot** _(Ranged Attack | Enemy Target)_:
      - Damage : 15-20
      - Chance : 100%
      - Effects : Bleed (100%, 3t, 5 DMG)

## Effects:
Effects are used in skills and basic attacks. These are applied ~~(if chance roll succeeds)~~ to targets of these actions. Effects can be stackable, meaning that the Character may suffer its conditions more than once each turn. If the effect is not stackable and the Character receives an existing effect, the effect with higher duration will be kept. 
  - **Stun:** Affected Character can't perform any actions. Not stackable.
  - **Disarm:** Affected Character can't attack. Not stackable.
  - **Cripple:** Affected Character can't move and takes Damage per Turn. Not stackable.
  - **Bleed:** Affected Character takes Damage per Turn. Stackable.
  - **Silence:** Affected Character can't cast spells. Not stackable.

## Tips:
  - If a skill target type is _Area_, it means the skill will target every character **around** that point, not restricted to cardinal directions. 
    - _Example_:
     
      [ X ]  [ X ]  [ X ]

      [ X ]  [ C ]  [ X ]

      [ X ]  [ X ]  [ X ]
    

      "C" represents the Character and "X" represents all possible targets for an area skill.

  - Support skills **always** have priority over other actions and have the same range as an _Area_ target type. That means if the cleric is adjacent to any of its allies, the cleric will heal the closest one with _Healing Hands_ skill.
  - Characters' targets are updated at the beginning of the correspondent character's turn. It will always select the closest enemy as the target.
  - When there are no valid enemies or allies in range for a skill, the character will try to move. The pathfinding system checks any possible movements for the character (up, down, right, left) and filter them by the least distance to the target.

