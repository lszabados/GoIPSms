using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPRecordEventArgs : EventArgs
    {
        public GoIPRecordEventArgs(string message, GoIPRecordPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIPRecordPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
