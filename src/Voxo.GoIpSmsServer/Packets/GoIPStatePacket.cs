using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPStatePacket : GoIPPacketBase
    {
        public GoIPStatePacket(string data) : base(data)
        {

        }

        public string receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }
        public string gsm_remain_state { get; protected set; }

        public override void ExtractData(string data)
        {
            // split into data row
            var dlist = data.Split(new char[] { ';' });

            receiveid = FindStringValue("STATE", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
            gsm_remain_state = FindStringValue("gsm_remain_state", dlist);
        }
    }
}
