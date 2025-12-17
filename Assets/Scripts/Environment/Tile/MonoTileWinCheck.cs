using Game.Environment.Tile.Data;
using Game.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Environment.Tile
{
    public class MonoTileWinCheck : MonoBehaviour
    {
        private SceneSwitching sceneSwitching;
        private MonoLevel level;
        private MonoTile tile;

        //called when the script is being loaded
        private void Awake()
        {
            sceneSwitching = FindObjectOfType<SceneSwitching>(); //get the object that controls scene switching
            level = FindObjectOfType<MonoLevel>(); //get the level object
            tile = GetComponent<MonoTile>(); //get the tile object this is attached to

            MonoTileInput tileInput = GetComponent<MonoTileInput>();
            tileInput.InputUpdate.Event += OnInputUpdate; //add OnInputUpdate as event listener
        }

        //is called each time a valid input is given
        private void OnInputUpdate()
        {
            //if the tile isn't a player, early return
            if (tile.CheckProperties(TileProperty.Player) == false)
                return;

            //get the tileStack at the tiles position
            List<MonoTile> tileStack = level.GetTileStack(tile.Position);

            //loop through the tileStack
            foreach (MonoTile tileAtPos in tileStack)
            {
                if (tileAtPos.CheckProperties(TileProperty.Win))
                {
                    Debug.Log("win");
                    sceneSwitching.NextScene(); //skip to the next scene
                }
            }
        }
    }
}