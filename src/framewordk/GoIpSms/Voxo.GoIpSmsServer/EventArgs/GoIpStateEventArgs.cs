using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpStateEventArgs : EventArgs
    {
        public GoIpStateEventArgs(string message, GoIpStatePacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpStatePacket Packet { get; }
        public string Host { get; }
        public int Port { get; }

    }
}
