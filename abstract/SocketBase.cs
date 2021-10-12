using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Networking
{
    public abstract class SocketBase : IDisposable
    {
        Socket m_socket;

        public SocketBase(byte ip0, byte ip1, byte ip2, byte ip3, ushort port, SocketType socket, ProtocolType protocol, ushort bufferSize, ushort backLog)
        {
            IPEndPoint ipe = new IPEndPoint(new IPAddress(new byte[4] { ip0, ip1, ip2, ip3 }), 8888);
            m_socket = new Socket(ipe.AddressFamily, socket, protocol);

            m_socket.SendBufferSize = bufferSize;
            m_socket.ReceiveBufferSize = bufferSize;

            m_socket.Bind(ipe);
            m_socket.Listen(backLog);
        }

        protected abstract void Dispose();
        void IDisposable.Dispose() { Dispose(); }

        public abstract class UDP : SocketBase
        {
            public UDP(byte ip0, byte ip1, byte ip2, byte ip3, ushort port, ushort bufferSize, ushort backLog)
                : base(ip0, ip1, ip2, ip3, port, SocketType.Dgram, ProtocolType.Udp, bufferSize, backLog) { }
            protected override void Dispose() { m_socket.Close(0); }
        }
    }
}