using Game.Environment.Tile.Data;
using UnityEngine;

namespace Game.Environment.Tile.ScriptTiles
{
    public class MonoScriptTile : MonoTile
    {
        private const TileProperty Properties = TileProperty.System | TileProperty.Movable | TileProperty.Collides; //the standard properties of a script tile

        //called when the script is loaded
        protected override void Awake()
        {
            base.Awake(); //call the parent's awake function
            Type.TileProperties.AddProperties(Properties); //set the properties of a script tile to the standard properties
        }
    }
}