#if DEBUG
using Game.Environment;
using Game.Environment.Tile;
using UnityEngine;

namespace Game.Utility.DEBUG
{
    public class UPDATE_UNITY_POS : MonoBehaviour
    {
        private void OnDrawGizmosSelected()
        {
            MonoLevel level = FindObjectOfType<MonoLevel>(); //get the level
            MonoTile tile = GetComponent<MonoTile>(); //get the tile

            //if either are null, don't bother running the rest
            if (level == null || tile == null)
                return;

            Vector3 origin = level.transform.position;
            origin.x -= level.LevelSize.x / 2f;
            origin.y -= level.LevelSize.y / 2f;

            float x = tile.Position.x; //implicit cast: X from int to float
            float y = tile.Position.y; //implicit cast: X from int to float

            transform.position = new Vector3(x, y) + origin;
        }
    }
}
#endif
