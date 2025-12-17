using Game.Environment.Tile.Data;
using Game.Utility;
using System;
using UnityEngine;

namespace Game.Environment.Tile
{
    public class MonoTileInput : MonoBehaviour
    {
        public EventManager InputUpdate { get; private set; }
        private MonoTile tile;
        private bool lockInput = false;

        private void Awake()
        {
            tile = GetComponent<MonoTile>();

            EventManager[] eventManagers = FindObjectsOfType<EventManager>();
            foreach (EventManager eventManager in eventManagers)
            {
                if (eventManager.EventName == "OnInputUpdate")
                {
                    InputUpdate = eventManager;
                    break;
                }
            }
        }

        private void Update()
        {
            if (CanReadInput() == false)
                return;

            float horizontalAxis = Input.GetAxisRaw("Horizontal");
            float verticalAxis = Input.GetAxisRaw("Vertical");
            bool horizontal = horizontalAxis != 0;
            bool vertical = verticalAxis != 0;

            if (InputUpdate.LockEvent == false)
            {
                InputUpdate.CallEvent(); //invoke the InputEvent
                InputUpdate.LockEvent = true;
            }

            if ((horizontal || vertical) && lockInput == false)
            {
                MoveTile(new Vector2(horizontalAxis, verticalAxis));
                lockInput = true; //lock the input
            }
            else if ((horizontal || vertical) == false) //if both horizontal and vertical has no input
            {
                lockInput = false; //unlock input
                InputUpdate.LockEvent = false; //unlock the InputEvent
            }
        }

        /// <summary>
        /// moves the tile using the relative vector <paramref name="_direction"/>, first moves horizontally, then vertically
        /// </summary>
        private void MoveTile(Vector2 _direction)
        {
            //split vector into integers
            int x = Convert.ToInt32(_direction.x);
            int y = Convert.ToInt32(_direction.y);

            //make two new vectors for the different axes
            Vector2Int horizontal = new(x, 0);
            Vector2Int vertical = new(0, y);

            if (horizontal != Vector2Int.zero) //if the horizontal axis contains a value
                tile.Movement.Move(horizontal); //move the tile

            if (vertical != Vector2Int.zero) //if the vertical axis contains a value
                tile.Movement.Move(vertical); //move the tile
        }

        private bool CanReadInput()
        {
            bool canMove = true; //initially assume we can move
            canMove &= tile.CheckProperties(TileProperty.Player); //check whether the tile is a player
            return canMove; //return results
        }
    }
}