using System;

namespace Voxo.GoIpSmsServer
{
    public class GoIPDeliveryReportEventArgs : EventArgs
    {
        public GoIPDeliveryReportEventArgs(string message, GoIPSmsDeliveryReportPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIPSmsDeliveryReportPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}