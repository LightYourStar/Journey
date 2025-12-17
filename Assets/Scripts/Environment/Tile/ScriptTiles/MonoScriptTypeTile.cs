using Game.Environment.Tile.Data;
using UnityEngine;

namespace Game.Environment.Tile.ScriptTiles
{
    public class MonoScriptTypeTile : MonoScriptTile
    {
        public TileType StoredType { get; private set; }

        [SerializeField] private string storedType;
        [SerializeField] private Sprite typeSprite;

        //called before the first frame
        protected override void Awake()
        {
            level = FindObjectOfType<MonoLevel>();

            if (level.TileTypes.ContainsKey(storedType)) //if the level already contains this type
            {
                StoredType = level.TileTypes[storedType]; //sets the type to the type contained in the level
                base.Awake(); //call the parent's awake function
                return; //early return
            }

            //initialize the tile type
            StoredType = new TileType(typeSprite);

            //adds the type to the
            level.TileTypes.Add(storedType, StoredType);

            base.Awake(); //call the parent's awake function
        }

        //called on every frame
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate(); //call parent update

            //don't bother if this is a system type
            if (StoredType.TileProperties.CheckProperties(TileProperty.System))
                return;

            //clear properties
            StoredType.TileProperties.RemoveProperties(TileProperty.Everything);
        }
    }
}