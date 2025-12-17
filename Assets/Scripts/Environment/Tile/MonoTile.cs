using System;
using UnityEngine;
using Game.Environment.Tile.Data;
using System.Collections.Generic;

namespace Game.Environment.Tile
{
    public class MonoTile : MonoBehaviour
    {
        [SerializeField] private string typeIdentifier;         //the identifier to find the tile type with
        [SerializeField] private Vector2Int levelPosition;      //the level position of the tile
        private SpriteRenderer spriteRenderer;  //renders the tile's sprite
        protected MonoLevel level;              //stores the level

        /// <summary>
        /// contains the different properties of the tile
        /// </summary>
        public TileType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// the identifier which the tile is stored with in <see cref="MonoLevel.TileTypes"/>
        /// </summary>
        public string TypeIdentifier
        {
            get => typeIdentifier;
        }

        /// <summary>
        /// controls the tile's movement
        /// </summary>
        public TileMovement Movement
        {
            get;
            private set;
        }

        /// <summary>
        /// holds the level-position of the tile
        /// </summary>
        public Vector2Int Position
        {
            [SerializeField] get => levelPosition;
            [SerializeField] set => levelPosition = level.IsValidPosition(value) ? value : throw new ArgumentOutOfRangeException("this is not a valid position within the level");
        }

        /// <summary>
        /// updates the stored type 
        /// </summary>
        public void SetType(string _typeIdentifier)
        {
            //if the level doesn't contain this type, add it
            if (level.TileTypes.ContainsKey(_typeIdentifier) == false)
            { 
                //create a new type using the sprite given in the sprite renderer
                TileType type = new(spriteRenderer.sprite);   

                //add the type to the level
                level.TileTypes.Add(_typeIdentifier, type);
            }

            //get the type from the level and store it as this type
            Type = level.TileTypes[_typeIdentifier];
        }

        //called when the script is loaded
        protected virtual void Awake()
        {
            if (level == null) //if level is null
                level = FindObjectOfType<MonoLevel>(); //find the level object

            spriteRenderer = GetComponent<SpriteRenderer>();

            SetType(typeIdentifier); //initialise type

            Movement = new TileMovement(this, level); //initialize tile movement class
        }

        //called before the first frame
        protected virtual void Start()
        {
            OnInputUpdate();

            MonoTileInput tileInput = GetComponent<MonoTileInput>();
            tileInput.InputUpdate.Event += OnInputUpdate; //add OnInputUpdate as event listener
        }


        /// <summary>
        /// called every time the player gave input
        /// </summary>
        protected virtual void OnInputUpdate()
        {
            UpdateUnityPosition(); //set the correct unity position

            //if this is a system type, don't bother with sprite checks
            if (Type.TileProperties.CheckProperties(TileProperty.System))
                return;

            //if the current sprite doesn't match the type's sprite, update sprite 
            if (spriteRenderer.sprite != Type.Sprite)
                spriteRenderer.sprite = Type.Sprite;
        }

        /// <summary>
        /// updates the gameObject's position in unity from the level position
        /// </summary>
        private void UpdateUnityPosition()
        {
            float x = Position.x; //implicit cast: X from int to float
            float y = Position.y; //implicit cast: Y from int to float
            float z;

            List<MonoTile> tileStack = level.GetTileStack(Position);
            int index = tileStack.IndexOf(this);
            z = tileStack.Count - index;

            transform.position = new Vector3(x, y, z) + level.Origin; //set the object's unity position, correct for the level origin
        }

        /// <inheritdoc cref="TileProperties.CheckProperties(TileProperty)"/>
        public bool CheckProperties(TileProperty _filter) => Type.TileProperties.CheckProperties(_filter);
    }
}