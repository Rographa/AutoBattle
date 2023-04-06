# AutoBattle

## Rules:
- Player must select a battlefield size. (e.g. 5x5, 10x8)
- Player must select a battle type. (e.g. 1vs1, 2vs2)
- Player must select a class for each of him/her/their characters.
- Opponent will randomly select a class for each character as well.
- Turn order will be random among all characters and it is set at the beginning of the game.
- Each turn, every character will try to perform up to one action. (Moving, Attacking and Casting Spells are actions.)
- Player team will automatically fight enemy team until one of those are defeated.

## Actions (in priority order):
- Cast Support Spells: Character will randomly select a skill amongst all of its Support type skills and evaluate the chance to execute it, if conditions are valid. Otherwise, it will skip and try next action.
- Cast Melee Spells: Character will randomly select a skill amongst all of its Melee type skills and evaluate the chance to execute it, if conditions are valid. Otherwise, it will skip and try next action.
- Attack: Character will try to attack its target. A target is only valid to be attacked by Basic Attack if it is in any of character's surroundings (up, down, left, right).
- Cast Ranged Spells: Character will randomly select a skill amongst all of its Ranged type skills and evaluate the chance to execute it, if conditions are valid. Otherwise, it will skip and try next action.
- Move: Character will try to move to one of its cardinal boxes (up, down, left, right) where its distance from target enemy is smaller.

Note: if Character couldn't perform any of its actions, its turn will be skipped.

## Classes:
- ### [1] **Paladin**
  - Health : 150
  - Damage Multiplier : 120%
  - Basic Attack Effect: Disarm (20%, 1t)
  - Skills:
    - **Holy Wrath** (Melee Attack | Enemy Target):
      - Damage : 12-20
      - Chance : 20%
      - Effects : Stun (40%, 1t)
    - **Healing Nova** (Support | Area):
      - Damage : 8-14
      - Chance : 30%
      - Effects : None      
- ### [2] **Warrior**:
  - Health : 130
  - Damage Multiplier : 150%
  - Basic Attack Effect: Bleed (30%, 2t, 3 DMG)
  - Skills:
    - **Heroic Strike** (Melee Attack | Enemy Target):
      - Damage : 12-24
      - Chance : 50%
      - Effects : Bleed (100%, 3t, 5 DMG), Disarm (50%, 1t)
    - **Whirlwind** (Melee Attack | Area):
      - Damage : 10-16
      - Chance : 40%
      - Effects : Bleed (100%, 3t, 5 DMG)
- ### [3] **Cleric**:
  - Health : 100
  - Damage Multiplier : 140%
  - Basic Attack Effect: Silence (40%, 2t)
  - Skills:
    - **Healing Hands** (Support | Ally):
      - Damage : 8-15
      - Chance : 100%
      - Effects : None
    - **Divine Blast** (Ranged Attack | Enemy Target):
      - Damage : 5-25
      - Chance : 50%
      - Effects : Silence (80%, 2t)
- ### [4] **Archer**:
  - #### Health : 100
  - #### Damage Multiplier : 160%
  - #### Basic Attack Effect: Cripple (15%, 1t, 2 DMG)
  - #### Skills:
    - **Crippling Shot** (Ranged Attack | Enemy Target):
      - Damage : 9-14
      - Chance : 80%
      - Effects : Cripple (70%, 1t, 2 DMG)
    - **Aimed Shot** (Ranged Attack | Enemy Target):
      - Damage : 15-20
      - Chance : 100%
      - Effects : Bleed (100%, 3t, 5 DMG)