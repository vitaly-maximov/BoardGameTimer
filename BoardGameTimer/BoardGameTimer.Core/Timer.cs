using System;
using System.Collections.Generic;
using System.Linq;

using BoardGameTimer.Model;

namespace BoardGameTimer.Core
{
    public class Timer
    {
        #region fields
                
        private readonly IReadOnlyList<UserModel> m_usersOrder;
        private readonly Dictionary<UserModel, TimeSpan> m_usersTime = new Dictionary<UserModel, TimeSpan>();

        private readonly Stack<TimeSpan> m_history = new Stack<TimeSpan>();

        private DateTime m_lastTime;
        private TimeSpan m_newTimeSpan;

        private int m_currentUserIndex;        

        #endregion

        public Timer(IReadOnlyList<UserModel> users)
        {
            m_usersOrder = users;
            Reset();
        }

        #region public methods

        public bool IsStarted { get; private set; }

        public UserModel GetCurrentUser()
        {
            return m_usersOrder[m_currentUserIndex];
        }

        public TimeSpan GetUserSpentTime(UserModel user)
        {
            return ReferenceEquals(m_usersOrder[m_currentUserIndex], user)
                ? m_usersTime[user] + m_newTimeSpan
                : m_usersTime[user];            
        }

        public void Reset()
        {
            m_newTimeSpan = TimeSpan.Zero;

            IsStarted = false;
            m_currentUserIndex = 0;

            foreach (UserModel user in m_usersOrder)
            {
                m_usersTime[user] = TimeSpan.Zero;
            }
        }

        public void Start()
        {
            if (!IsStarted)
            {
                m_lastTime = DateTime.Now;
                IsStarted = true;
            }
        }

        public void Stop()
        {
            if (IsStarted)
            {
                Update();
                IsStarted = false;
            }
        }

        public void Update()
        {
            if (IsStarted)
            {
                DateTime now = DateTime.Now;
                m_newTimeSpan += now - m_lastTime;
                m_lastTime = now;
            }            
        }

        public void Next()
        {
            Update();

            m_history.Push(m_newTimeSpan);
            m_usersTime[m_usersOrder[m_currentUserIndex]] += m_newTimeSpan;

            m_newTimeSpan = TimeSpan.Zero;
            m_currentUserIndex = (m_currentUserIndex + 1) % m_usersOrder.Count;
        }

        public void Previous()
        {
            if (m_history.Any())
            {
                TimeSpan time = m_history.Pop();
                m_newTimeSpan += time;

                m_currentUserIndex = (m_currentUserIndex + m_usersOrder.Count - 1) % m_usersOrder.Count;
                m_usersTime[m_usersOrder[m_currentUserIndex]] -= time;
            }          
        }

        #endregion
    }
}
