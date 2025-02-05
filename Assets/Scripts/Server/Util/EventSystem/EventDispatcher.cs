using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

namespace Almond.Util
{
    public class EventController
    {
        private Dictionary<string, Delegate> m_theRouter = new Dictionary<string, Delegate>();

        public Dictionary<string, Delegate> TheRouter
        {
            get { return m_theRouter; }
        }

        /// <summary>
        /// 영구등록 이벤트
        /// </summary>
        private List<string> m_permanentEvents = new List<string>();

        /// <summary>
        /// 이벤트를 영구적으로 등록한다
        /// </summary>
        /// <param name="eventType"></param>
        public void MarkAsPermanent(string eventType)
        {
            m_permanentEvents.Add(eventType);
        }

        /// <summary>
        /// 이벤트 등록여부
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool ContainsEvent(string eventType)
        {
            return m_theRouter.ContainsKey(eventType);
        }

        /// <summary>
        /// 이벤트 등록 정보를 초기화 한다
        /// </summary>
        public void Cleanup()
        {
            List<string> eventToRemove = new List<string>();

            foreach (KeyValuePair<string, Delegate> pair in m_theRouter)
            {
                bool wasFound = false;
                for (int i = 0; i < m_permanentEvents.Count; i++ )
                {
                    if (pair.Key == m_permanentEvents[i])
                    {
                        wasFound = true;
                        break;
                    }
                }

                if (!wasFound)
                    eventToRemove.Add(pair.Key);
            }

            foreach (string Event in eventToRemove)
            {
                m_theRouter.Remove(Event);
            }
        }

        /// <summary>
        /// 이벤트 등록정보를 체크한다.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingAdded"></param>
        private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                m_theRouter.Add(eventType, null);
            }

            Delegate d = m_theRouter[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new EventException(string.Format(
                       "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                       eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        /// <summary>
        /// 이벤트 등록 델리게이트를 삭제한다.
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="listenerBeingRemoved"></param>
        private bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (!m_theRouter.ContainsKey(eventType))
            {
                return false;
            }

            Delegate d = m_theRouter[eventType];
            if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
            {
                throw new EventException(string.Format(
                    "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                    eventType, d.GetType(), listenerBeingRemoved.GetType()));
            }
            else
                return true;
        }

        /// <summary>
        /// 이벤트 등록정보를 삭제한다.
        /// </summary>
        /// <param name="eventType"></param>
        private void OnListenerRemoved(string eventType)
        {
            if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
            {
                m_theRouter.Remove(eventType);
            }
        }

        #region 이벤트 등록
        /// <summary>
        ///  이벤트를 등록하며 파라미터 없음
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener(string eventType, Action handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  이벤트를 등록하며 파라미터 1개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  이벤트를 등록하며 파라미터 2개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  이벤트를 등록하며 파라미터 3개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] + handler;
        }

        /// <summary>
        ///  이벤트를 등록하며 파라미터 4개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            OnListenerAdding(eventType, handler);
            m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] + handler;
        }
        #endregion

        #region 이벤트 삭제

        /// <summary>
        ///  이벤트 삭제 파라미터 없음
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(string eventType, Action handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 1개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 2개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 3개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 4개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            if (OnListenerRemoving(eventType, handler))
            {
                m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] - handler;
                OnListenerRemoved(eventType);
            }
        }
        #endregion

        #region 이벤트 발생
        /// <summary>
        ///  이벤트 발생, 파라미터 없음
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent(string eventType)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }

            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action callback = callbacks[i] as Action;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }

                try
                {

                    callback();
                }
                catch (Exception ex)
                {
                 //   LoggerHelper.Except(ex);
                }
            }
        }

        /// <summary>
        ///  이벤트 발생, 파라미터 1개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T>(string eventType, T arg1)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }

            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T> callback = callbacks[i] as Action<T>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }

                try
                {
                    callback(arg1);
                }
                catch (Exception ex)
                {
               //     LoggerHelper.Except(ex);
                }
            }
        }

        /// <summary>
        ///  이벤트 발생, 파라미터 2개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U> callback = callbacks[i] as Action<T, U>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }

                try
                {
                    callback(arg1, arg2);
                }
                catch (Exception ex)
                {
                //    LoggerHelper.Except(ex);
                }
            }
        }

        /// <summary>
        ///  이벤트 발생, 파라미터 3개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U, V> callback = callbacks[i] as Action<T, U, V>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                 //   LoggerHelper.Except(ex);
                }
            }
        }

        /// <summary>
        ///  이벤트 발생, 파라미터 4개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            Delegate d;
            if (!m_theRouter.TryGetValue(eventType, out d))
            {
                return;
            }
            var callbacks = d.GetInvocationList();
            for (int i = 0; i < callbacks.Length; i++)
            {
                Action<T, U, V, W> callback = callbacks[i] as Action<T, U, V, W>;

                if (callback == null)
                {
                    throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
                }
                try
                {
                    callback(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                 //   LoggerHelper.Except(ex);
                }
            }
        }

        #endregion
    }

    public class EventDispatcher
    {
        private static EventController m_eventController = new EventController();

        public static Dictionary<string, Delegate> TheRouter
        {
            get { return m_eventController.TheRouter; }
        }

        /// <summary>
        /// 영구등록 이벤트
        /// </summary>
        /// <param name="eventType"></param>
        static public void MarkAsPermanent(string eventType)
        {
            m_eventController.MarkAsPermanent(eventType);
        }

        /// <summary>
        /// 清除非永久性注册的事件
        /// </summary>
        static public void Cleanup()
        {
            m_eventController.Cleanup();
        }

        #region 이벤트 등록
        /// <summary>
        ///  이벤트 등록 파라미터 없음
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener(string eventType, Action handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 등록 파라미터 1개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T>(string eventType, Action<T> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 등록 파라미터 2개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 등록 파라미터 3개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 등록 파라미터 4개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void AddEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            m_eventController.AddEventListener(eventType, handler);
        }
        #endregion

        #region 이벤트 삭제
        /// <summary>
        ///  이벤트 삭제 파라미터 없음
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener(string eventType, Action handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 1개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T>(string eventType, Action<T> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 2개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T, U>(string eventType, Action<T, U> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 3개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T, U, V>(string eventType, Action<T, U, V> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }

        /// <summary>
        ///  이벤트 삭제 파라미터 5개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void RemoveEventListener<T, U, V, W>(string eventType, Action<T, U, V, W> handler)
        {
            m_eventController.RemoveEventListener(eventType, handler);
        }
        #endregion

        #region 이벤트 발생
        /// <summary>
        ///  이벤트 발생 파라미터 없음
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent(string eventType)
        {
            m_eventController.TriggerEvent(eventType);
        }

        /// <summary>
        ///  이벤트 발생 파라미터 1개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T>(string eventType, T arg1)
        {
            m_eventController.TriggerEvent(eventType, arg1);
        }

        /// <summary>
        ///  이벤트 발생 파라미터 2개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T, U>(string eventType, T arg1, U arg2)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2);
        }

        /// <summary>
        ///  이벤트 발생 파라미터 3개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T, U, V>(string eventType, T arg1, U arg2, V arg3)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2, arg3);
        }

        /// <summary>
        ///  이벤트 발생 파라미터 4개
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="handler"></param>
        static public void TriggerEvent<T, U, V, W>(string eventType, T arg1, U arg2, V arg3, W arg4)
        {
            m_eventController.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
        }

        #endregion
    }
}