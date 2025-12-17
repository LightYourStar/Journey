using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Environment.Tile.ScriptTiles
{
    public class MonoScriptEqualTile : MonoScriptTile
    {
        protected override void OnInputUpdate()
        {
            base.OnInputUpdate();
            TryWriteProperty(Vector2Int.up);
            TryWriteProperty(Vector2Int.left);
        }

        private bool TryWriteProperty(Vector2Int _rTypePos)
        {
            if (CheckPositions(_rTypePos) == false)
                return false;

            MonoScriptTypeTile typeTile = (MonoScriptTypeTile)level.GetTileStack(_rTypePos + Position)[0];
            MonoScriptPropertyTile propertyTile = (MonoScriptPropertyTile)level.GetTileStack(-_rTypePos + Position)[0];

            typeTile.StoredType.TileProperties.AddProperties(propertyTile.Properties.Raw);

            return true;
        }

        private bool CheckPositions(Vector2Int _rTypePos)
        {
            return CheckPosition<MonoScriptTypeTile>(_rTypePos) && CheckPosition<MonoScriptPropertyTile>(_rTypePos * -1);
        }

        private bool CheckPosition<T>(Vector2Int _rPos)
        {
            //if the given position is even valid
            if (level.IsValidPosition(_rPos + Position) == false)
                return false;

            List<MonoTile> tileStack = level.GetTileStack(_rPos + Position); //gets the tile stack at the position
            return tileStack.OfType<T>().Any(); //filters the list for types and checks if there are any items in the list
        }
    }
}
