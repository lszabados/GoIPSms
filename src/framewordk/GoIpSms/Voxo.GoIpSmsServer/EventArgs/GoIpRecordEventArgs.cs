using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpRecordEventArgs : EventArgs
    {
        public GoIpRecordEventArgs(string message, GoIpRecordPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpRecordPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
