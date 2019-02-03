using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPMessageEventArgs : EventArgs
    {
        public GoIPMessageEventArgs(string message, GoIPMessagePacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIPMessagePacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
