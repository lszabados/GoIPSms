using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpExpiryEventArgs : EventArgs
    {
        public GoIpExpiryEventArgs(string message, GoIpExpiryPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpExpiryPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
