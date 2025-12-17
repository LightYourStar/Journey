using System;

namespace Game.Environment.Tile.Data
{
    [Flags]
    public enum TileProperty : int
    {
        //short-hand properties
        Everything = -1,        //all bits
        None = 0,               //no bits

        //type properties
        System = int.MinValue,  //signed bit
        Player = 1,
        Collides = 2,
        Movable = 4,
        Win = 8,
    }
}