using Game.Environment.Tile.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.Tile
{
    public class TileMovement
    {
        private readonly MonoLevel level;
        private readonly MonoTile tile;


        public TileMovement(MonoTile _tile, MonoLevel _level)
        {
            tile = _tile;
            level = _level;

            //add this tile to the tileStack at the right position if it is not in that position
            List<MonoTile> tileStack = level.GetTileStack(tile.Position);
            if (tileStack.Contains(tile) == false)
                tileStack.Add(tile);
        }

        /// <summary>
        /// moves the tile
        /// </summary>
        /// <param name="_relativePos">the relative position from the tile to which it must move</param>
        public void Move(Vector2Int _relativePos)
        {
            Vector2Int newPos;

            //if the tile can't move, early return
            if (CanMove(_relativePos) == false)
                return;

            //calculate the new position
            newPos = tile.Position + _relativePos;

            //loop through the tiles which are located at the moved to positions
            foreach (MonoTile moveToTile in level.GetTileStack(newPos).ToArray()) //uses .ToArray() to duplicate the collection so it's not modified whilst looping
            {
                if (moveToTile.CheckProperties(TileProperty.Movable)) //check if the tile moved to is a movable tile
                    moveToTile.Movement.Move(_relativePos); //move the tile with the same relative position as this tile
            }
            //update the level
            level.GetTileStack(tile.Position).Remove(tile); //remove the current position from the tile stack
            level.GetTileStack(newPos).Add(tile); //add the tile at the new position to the tile stack

            //update the internal position
            tile.Position = newPos;
        }

        /// <summary>
        /// checks whether the tile can move <paramref name="_relativePos"/>
        /// </summary>
        /// <param name="_relativePos">the relative position from the current position which the tile is moving to</param>
        private bool CanMove(Vector2Int _relativePos)
        {
            bool canMove; //whether the current tile can move    to the position
            List<MonoTile> movedToTiles; //the tiles located at the position trying to move to
            Vector2Int newPos = tile.Position + _relativePos; //calculate the new position

            //checks whether the position is a valid one
            if (level.IsValidPosition(newPos) == false)
                return false;

            movedToTiles = level.GetTileStack(newPos); //gets the tiles at the new position
            canMove = true; //initially assumes the tile can move

            //loops through all the tiles at the new position (skips this step if there are none)
            foreach (MonoTile movedToTile in movedToTiles)
            {
                //if we can't move at any point within the loop, break
                if (canMove == false)
                    break;

                //if the current tile and the moved to tile are players
                if (tile.CheckProperties(TileProperty.Player) && movedToTile.CheckProperties(TileProperty.Player))
                {
                    canMove &= movedToTile.Movement.CanMove(_relativePos); //check whether this player can move
                    continue;
                }

                //if the tile is movable, set canMove to that tile moving's result.
                if (movedToTile.CheckProperties(TileProperty.Movable))
                {
                    canMove &= movedToTile.Movement.CanMove(_relativePos); //if any tile can't move, canMove = false
                    continue; //continue the loop
                }

                //if the moved to tile collides, set canMove to false
                if (movedToTile.CheckProperties(TileProperty.Collides))
                {
                    canMove = false;
                }
            }

            //return the results
            return canMove;
        }
    }
}