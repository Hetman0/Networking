using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public abstract partial class SocketBase
    {
        public abstract class UDP : SocketBase
        {
            HashSet<IAsyncResult> m_sendThreadHandler = new HashSet<IAsyncResult>();
            HashSet<IAsyncResult> m_receiveThreadHandler = new HashSet<IAsyncResult>();

            public UDP(byte ip0, byte ip1, byte ip2, byte ip3, ushort port, int receiveBuffer, int sendBuffer)
                : base(ip0, ip1, ip2, ip3, port, SocketType.Dgram, ProtocolType.Udp, receiveBuffer, sendBuffer)
            {
                BeginReceive();
            }
            protected override void Dispose() { m_socket.Close(0); }

            public void BeginSend(EndPoint target, byte[] data, int offset, int size)
            {
                byte[] buffer = m_sendBuffer.Pop();
                Array.Copy(data, offset, buffer, 0, size);
                IAsyncResult result = null;

                result = m_socket.BeginSendTo(buffer, 0, size, SocketFlags.None, target, OnSendCallback, buffer);
                lock (m_sendThreadHandler)
                { m_sendThreadHandler.Add(result); }
            }
            /*public*/
            void BeginReceive()
            {
                byte[] buffer = m_receiveBuffer.Pop();
                SocketError err = SocketError.SocketError;
                IAsyncResult result = null;

                result = m_socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out err, OnReceiveCallback, buffer);
                lock (m_receiveThreadHandler)
                { m_receiveThreadHandler.Add(result); }

                switch (err)
                {
                    case SocketError.IOPending: break;
                    default: throw new SocketException((int)err);
                }
            }
            /*public void CloseAllReceive()
            {
                lock (m_receiveThreadHandler)
                    foreach (var item in m_receiveThreadHandler)
                    {
                        item.AsyncWaitHandle.Close();
                    }
            }*/

            /// <summary>
            /// Is called when received data
            /// </summary>
            /// <param name="data">Received data, WARNING: don't resize</param>
            /// <param name="size">Amount of received data, any additional bytes are random trush and should be ignored</param>
            protected virtual void OnReceiveAsync(byte[] data, int size) { }
            /// <summary>
            /// Is called after send data
            /// </summary>
            /// <param name="data">Send data, WARNING: don't resize</param>
            /// <param name="size">Amount of send data, any additional bytes are random trush and should be ignored</param>
            protected virtual void OnSendAsync(byte[] data, int size) { }

            protected virtual void OnReceiveCallback(IAsyncResult result)
            {
                byte[] buffer = result.AsyncState as byte[];
                int size = 0;
                try
                {
                    size = m_socket.EndReceive(result, out SocketError err);
                    switch (err)
                    {
                        case SocketError.Success: break;
                        default: throw new SocketException((int)err);
                    }
                }
                finally
                {
                    lock (m_receiveThreadHandler) m_receiveThreadHandler.Remove(result);
                    BeginReceive();
                }

                int bufferSize = buffer.Length;
                OnReceiveAsync(buffer, size);
                if (bufferSize == buffer.Length)
                { m_receiveBuffer.Push(buffer); }
            }
            protected virtual void OnSendCallback(IAsyncResult result)
            {
                byte[] buffer = result.AsyncState as byte[];
                int size;
                try
                {
                    size = m_socket.EndSend(result, out SocketError err);
                    switch (err)
                    {
                        case SocketError.Success: break;
                        default: throw new SocketException((int)err);
                    }
                }
                finally
                { lock (m_sendThreadHandler) m_sendThreadHandler.Remove(result); }

                int bufferSize = buffer.Length;
                OnSendAsync(buffer, size);
                if (bufferSize == buffer.Length)
                { m_sendBuffer.Push(buffer); }
            }
        }
    }
}