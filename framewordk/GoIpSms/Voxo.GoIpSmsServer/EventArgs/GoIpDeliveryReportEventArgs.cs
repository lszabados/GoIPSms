using System;

namespace Voxo.GoIpSms
{
    public class GoIpDeliveryReportEventArgs : EventArgs
    {
        public GoIpDeliveryReportEventArgs(string message, GoIpSmsDeliveryReportPacket packet, string host, int port)
        {
            Message = message;
            Packet = packet;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public GoIpSmsDeliveryReportPacket Packet { get; }
        public string Host { get; }
        public int Port { get; }
    }
}