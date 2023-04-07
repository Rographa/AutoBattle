using static AutoBattle.Input;

namespace AutoBattle
{
    public static class Stage
    {
        public static Grid Grid;
        
        public static Grid SetupBattlefield()
        {
            var size = GetBattlefieldSize();
            Grid = new Grid(size.x, size.y);
            return Grid;
        }
        
        public static void AllocateCharacter(Character character)
        {
            var randomLocation = Grid.RandomUnoccupiedGridBox;
            randomLocation.OccupiedBy = character;
            character.CurrentBox = randomLocation;
                
            var index = randomLocation.Index;
            Grid.Grids[index] = randomLocation;
        }
    }
}