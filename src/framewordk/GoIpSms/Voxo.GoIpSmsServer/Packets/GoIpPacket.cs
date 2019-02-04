using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpPacket : GoIpPacketBase
    {
        private readonly string packetId;

        public GoIpPacket(string data, string PacketId) : base(data)
        {
            packetId = PacketId;
        }

        public int receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }

        public override void ExtractData(string[] dlist)
        {
            receiveid = FindIntValue(packetId, dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
        }
    }
}
