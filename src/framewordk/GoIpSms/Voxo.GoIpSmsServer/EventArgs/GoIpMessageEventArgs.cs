using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpMessageEventArgs : EventArgs
    {
        public GoIpMessageEventArgs(string message, GoIpMessagePacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpMessagePacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
