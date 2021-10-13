using System;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public abstract partial class SocketBase : IDisposable
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
    }
}