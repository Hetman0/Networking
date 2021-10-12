using System;
using System.Collections.Generic;

namespace Networking
{
    public class AsyncProvider<T>
    {
        Func<T> m_ctor;
        Stack<T> m_stack = new Stack<T>();
        object m_lock = new object();

        public AsyncProvider(Func<T> ctor)
        {
            m_ctor = ctor;
        }

        public T Pop()
        {
            lock(m_lock)
            {
                if(m_stack.Count > 0)
                {
                    return m_stack.Pop();
                }
            }
            return m_ctor();
        }
        public void Push(T value)
        {
            lock (m_lock)
            {
                m_stack.Push(value);
            }
        }
        public void Clear()
        {
            lock(m_lock)
            {
                m_stack.Clear();
            }
        }
    }
}
