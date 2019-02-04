using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpHangupEventArgs : EventArgs
    {
        public GoIpHangupEventArgs(string message, GoIpHangupPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpHangupPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}
