using System;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public abstract class SocketBase : IDisposable
    {
        Socket m_socket;
        AsyncProvider<byte[]> m_sendBuffer, m_receiveBuffer;

        public EndPoint LocalEndPoint { get => m_socket.LocalEndPoint; }
        public int ReceiveSize { get => m_socket.ReceiveBufferSize; }
        public int SendSize { get => m_socket.SendBufferSize; }

        public SocketBase(byte ip0, byte ip1, byte ip2, byte ip3, ushort port, SocketType socket, ProtocolType protocol, int receiveBuffer, int sendBuffer)
        {
            IPEndPoint ipe = new IPEndPoint(new IPAddress(new byte[4] { ip0, ip1, ip2, ip3 }), port);
            m_socket = new Socket(ipe.AddressFamily, socket, protocol);

            m_socket.ReceiveBufferSize = receiveBuffer;
            m_receiveBuffer = new AsyncProvider<byte[]>(() => new byte[receiveBuffer]);
            m_socket.SendBufferSize = sendBuffer;
            m_sendBuffer = new AsyncProvider<byte[]>(() => new byte[sendBuffer]);

            m_socket.Bind(ipe);
        }

        protected abstract void Dispose();
        void IDisposable.Dispose() { Dispose(); }

        public abstract class UDP : SocketBase
        {
            

            public UDP(byte ip0, byte ip1, byte ip2, byte ip3, ushort port, int receiveBuffer, int sendBuffer)
                : base(ip0, ip1, ip2, ip3, port, SocketType.Dgram, ProtocolType.Udp, receiveBuffer, sendBuffer) 
            {
                byte[] buffer = m_receiveBuffer.Pop();
                SocketError err;
                m_socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out err, OnReceiveCallback, buffer);
            }
            protected override void Dispose() { m_socket.Close(0); }

            public void Send(EndPoint target, byte[] data, int offset, int size)
            {
                byte[] buffer = m_sendBuffer.Pop();
                Array.Copy(data, offset, buffer, 0, size);
                m_socket.BeginSendTo(buffer, 0, size, SocketFlags.None, target, OnSendCallback, buffer);
            }
            /// <summary>
            /// Is called when receiving data
            /// </summary>
            /// <param name="data">Received data, WARNING: don't resize</param>
            /// <param name="size">Amount of received data, any additional bytes are random trush and should be ignored</param>
            protected abstract void OnReceiveAsync(byte[] data, int size);

            protected virtual void OnReceiveCallback(IAsyncResult result)
            {
                SocketError errEnd;
                int size = m_socket.EndReceive(result, out errEnd);

                byte[] buffer = m_receiveBuffer.Pop();
                SocketError errBeg;
                m_socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, out errBeg, OnReceiveCallback, buffer);

                buffer = result.AsyncState as byte[];
                int bufferSize = buffer.Length;
                OnReceiveAsync(buffer, size);
                if(bufferSize != buffer.Length)
                {
                    throw new Exception("Buffer Lenght changed");
                }
                m_receiveBuffer.Push(buffer);
            }
            protected virtual void OnSendCallback(IAsyncResult result)
            {
                SocketError err;
                m_socket.EndSend(result, out err);
                m_sendBuffer.Push(result.AsyncState as byte[]);
            }
        }
    }
}