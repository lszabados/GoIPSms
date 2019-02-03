using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPRegisterEventArgs : EventArgs
    {
        public GoIPRegisterEventArgs(string message, GoIPRegistrationPacket packet, string host, int port, int status)
        {
            Packet = packet;
            Host = host;
            Port = port;
            Status = status;
            Message = message;
        }

        public string Message { get; }
        public GoIPRegistrationPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
        public int Status { get; }
    }
}
