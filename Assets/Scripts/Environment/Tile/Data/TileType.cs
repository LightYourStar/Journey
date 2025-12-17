using UnityEngine;

namespace Game.Environment.Tile.Data
{
    public class TileType
    {
        public TileProperties TileProperties { get; private set; }
        public Sprite Sprite { get; private set; }

        public TileType(Sprite _sprite, TileProperty _defaultProperties = TileProperty.None)
        {
            TileProperties = new TileProperties(_defaultProperties);
            Sprite = _sprite;
        }
    }
}