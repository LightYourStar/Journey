using Game.Environment.Tile.Data;
using UnityEngine;

namespace Game.Environment.Tile.ScriptTiles
{
    public class MonoScriptPropertyTile : MonoScriptTile
    {
        [SerializeField] private TileProperty writeProperties; //the properties written to the type with equal

        public TileProperties Properties { get; private set; }

        //called once the script is being loaded
        protected override void Awake()
        {
            base.Awake(); //call the parent's awake function
            Properties = new TileProperties(writeProperties);
        }
    }
}