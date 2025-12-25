using System;
using UnityEngine;

namespace Game.Utility
{
    public class EventManager : MonoBehaviour
    {
        [SerializeField] private string eventName;

        /// <summary>
        /// the event stored by the manager
        /// </summary>
        public event Action Event;

        /// <summary>
        /// the name of the event set
        /// </summary>
        public string EventName { get => eventName; }

        /// <summary>
        /// if <see langword="true"/>, the event is prevented to be ran with <see cref="CallEvent"/>
        /// </summary>
        public bool LockEvent { get; set; }


        /// <summary>
        /// invokes <see cref="Event"/>
        /// </summary>
        public void CallEvent()
        {
            if (LockEvent == false)
                Event?.Invoke();
        }
    }
}
