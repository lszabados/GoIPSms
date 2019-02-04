using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpRegisterEventArgs : EventArgs
    {
        public GoIpRegisterEventArgs(string message, GoIpRegistrationPacket packet, string host, int port, int status)
        {
            Packet = packet;
            Host = host;
            Port = port;
            Status = status;
            Message = message;
        }

        public string Message { get; }
        public GoIpRegistrationPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
        public int Status { get; }
    }
}
