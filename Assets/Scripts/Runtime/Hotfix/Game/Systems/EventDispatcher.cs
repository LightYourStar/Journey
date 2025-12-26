using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
public delegate void Callback<T, U, V, W>(T arg1, U arg2, V arg3, W arg4);
namespace JO
{
    public class MsgDispatcher
    {

        private Dictionary<int, Delegate> m_EventTable;
        private List<int> m_PermanentMessages;
        public MsgDispatcher()
        {
            m_EventTable = new Dictionary<int, Delegate>();
            m_PermanentMessages = new List<int>();
        }
        public void MarkAsPermanent(int eventType)
        {
#if SHOW_MESSAGE_LOG
		Debug.Log('Messenger MarkAsPermanent \t'' + eventType + ''');
#endif

            if (!m_PermanentMessages.Contains(eventType))
            {
                m_PermanentMessages.Add(eventType);
            }
        }
        public void DebugLogMsgId()
        {
            foreach (KeyValuePair<int, Delegate> eventData in m_EventTable)
            {
                Debug.Log("Msgid" + eventData.Key);
            }

        }
        public void Cleanup()
        {
#if SHOW_MESSAGE_LOG
		Debug.Log('MESSENGER Cleanup. Make sure that none of necessary listeners are removed.');
#endif

            List<int> messagesToRemove = new List<int>();

            foreach (KeyValuePair<int, Delegate> pair in m_EventTable)
            {
                bool wasFound = false;

                foreach (int message in m_PermanentMessages)
                {
                    if (pair.Key == message)
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                    messagesToRemove.Add(pair.Key);
            }

            foreach (int message in messagesToRemove)
            {
                m_EventTable.Remove(message);
            }
        }

        private void OnListenerAdding(int eventType, Delegate listenerBeingAdded)
        {
#if SHOW_MESSAGE_LOG || LOG_ADD_LISTENER
            Debug.Log("MESSENGER OnListenerAdding \t" + eventType + "\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
#endif

            if (!m_EventTable.ContainsKey(eventType))
            {
                m_EventTable.Add(eventType, null);
            }

            Delegate d = m_EventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        private bool OnListenerRemoving(int eventType, Delegate listenerBeingRemoved)
        {
#if SHOW_MESSAGE_LOG
		Debug.Log('MESSENGER OnListenerRemoving \t'' + eventType + ''\t{' + listenerBeingRemoved.Target + ' -> ' +listenerBeingRemoved.Method + '}');
#endif

            if (m_EventTable.ContainsKey(eventType))
            {
                Delegate d = m_EventTable[eventType];

                if (d == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void OnListenerRemoved(int eventType)
        {
            if (m_EventTable[eventType] == null)
            {
                m_EventTable.Remove(eventType);
            }
        }

        private void OnBroadcasting(int eventType)
        {
            //#if REQUIRE_LISTENER
            //            if (!m_EventTable.ContainsKey(eventType))
            //            {
            //#if SHOW_MESSAGE_LOG
            //                throw new BroadcastException(string.Format("Broadcasting message '{0}' but no listener found. Try marking the message with MsgDispatcher.MarkAsPermanent.", eventType));
            //#endif
            //            }
            //#endif
        }

        private BroadcastException CreateBroadcastSignatureException(int eventType)
        {
            return new BroadcastException(string.Format("Broadcasting message '{0}' but listeners have a different signature than the broadcaster.", eventType));
        }

        private class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        private class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }





        public void RegEvent(int eventType, Callback handler, bool addListener)
        {
            if (addListener)
            {
                AddListener(eventType, handler);
            }
            else
            {
                RemoveListener(eventType, handler);
            }

        }

        public void RegEvent<T>(int eventType, Callback<T> handler, bool addListener)
        {
            if (addListener)
            {
                AddListener<T>(eventType, handler);
            }
            else
            {
                RemoveListener<T>(eventType, handler);
            }
        }
        public void RegEvent<T, U>(int eventType, Callback<T, U> handler, bool addListener)
        {
            if (addListener)
            {
                AddListener<T, U>(eventType, handler);
            }
            else
            {
                RemoveListener<T, U>(eventType, handler);
            }
        }
        public void RegEvent<T, U, V>(int eventType, Callback<T, U, V> handler, bool addListener)
        {
            if (addListener)
            {
                AddListener<T, U, V>(eventType, handler);
            }
            else
            {
                RemoveListener<T, U, V>(eventType, handler);
            }
        }
        public void RegEvent<T, U, V, W>(int eventType, Callback<T, U, V, W> handler, bool addListener)
        {
            if (addListener)
            {
                AddListener<T, U, V, W>(eventType, handler);
            }
            else
            {
                RemoveListener<T, U, V, W>(eventType, handler);
            }
        }

        private void AddListener(int eventType, Callback handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback)m_EventTable[eventType] + handler;
        }

        private void AddListener<T>(int eventType, Callback<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T>)m_EventTable[eventType] + handler;
        }
        private void AddListener<T, U>(int eventType, Callback<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U>)m_EventTable[eventType] + handler;
        }
        private void AddListener<T, U, V>(int eventType, Callback<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U, V>)m_EventTable[eventType] + handler;
        }
        private void AddListener<T, U, V, W>(int eventType, Callback<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            m_EventTable[eventType] = (Callback<T, U, V, W>)m_EventTable[eventType] + handler;
        }

        private void RemoveListener(int eventType, Callback handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        private void RemoveListener<T>(int eventType, Callback<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        private void RemoveListener<T, U>(int eventType, Callback<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        private void RemoveListener<T, U, V>(int eventType, Callback<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U, V>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        private void RemoveListener<T, U, V, W>(int eventType, Callback<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_EventTable[eventType] = (Callback<T, U, V, W>)m_EventTable[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        public void Broadcast(int eventType)
        {
#if SHOW_MESSAGE_LOG
            Debug.Log("MESSENGER\t" + "\t\t\tInvoking \t" + eventType);
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback callback = d as Callback;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        //Single parameter
        public void Broadcast<T>(int eventType, T arg1)
        {
#if SHOW_MESSAGE_LOG
            Debug.Log("MESSENGER\t" + "\t\t\tInvoking \t" + eventType);
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T> callback = d as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }
        public void Broadcast<T, U>(int eventType, T arg1, U arg2)
        {
#if SHOW_MESSAGE_LOG
            Debug.Log("MESSENGER\t" + "\t\t\tInvoking \t" + eventType);
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U> callback = d as Callback<T, U>;

                if (callback != null)
                {
                    callback(arg1, arg2);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }
        public void Broadcast<T, U, V>(int eventType, T arg1, U arg2, V arg3)
        {
#if SHOW_MESSAGE_LOG
            Debug.Log("MESSENGER\t" + "\t\t\tInvoking \t" + eventType);
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V> callback = d as Callback<T, U, V>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }

        public void Broadcast<T, U, V, W>(int eventType, T arg1, U arg2, V arg3, W arg4)
        {
#if SHOW_MESSAGE_LOG
            Debug.Log("MESSENGER\t" + "\t\t\tInvoking \t" + eventType);
#endif
            OnBroadcasting(eventType);

            Delegate d;
            if (m_EventTable.TryGetValue(eventType, out d))
            {
                Callback<T, U, V, W> callback = d as Callback<T, U, V, W>;

                if (callback != null)
                {
                    callback(arg1, arg2, arg3, arg4);
                }
                else
                {
                    throw CreateBroadcastSignatureException(eventType);
                }
            }
        }
    }
}