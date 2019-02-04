using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpRemainEventArgs : EventArgs
    {
        public GoIpRemainEventArgs(string message, GoIpRemainPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpRemainPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
