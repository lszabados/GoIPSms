using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpPacket : GoIpPacketBase
    {
        public GoIpPacket(string data) : base(data)
        {


        }

        public int receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }

        public override void ExtractData(string[] dlist)
        {
            receiveid = FindIntValue("CELLS", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
        }
    }
}
