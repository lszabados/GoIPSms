using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPStateEventArgs : EventArgs
    {
        public GoIPStateEventArgs(string message, GoIPStatePacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIPStatePacket Packet { get; }
        public string Host { get; }
        public int Port { get; }

    }
}
