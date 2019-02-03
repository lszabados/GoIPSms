using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPRemainEventArgs : EventArgs
    {
        public GoIPRemainEventArgs(string message, GoIPRemainPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIPRemainPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
