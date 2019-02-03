using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPRemainPacket : GoIPPacketBase
    {
        public GoIPRemainPacket(string data) : base(data)
        {

        }

        public int receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }
        public string gsm_remain_time { get; protected set; }
        

        public override void ExtractData(string data)
        {
            // split into data row
            var dlist = data.Split(new char[] { ';' });

            receiveid = FindIntValue("REMAIN", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
            gsm_remain_time = FindStringValue("gsm_remain_time", dlist);
        }

    }
}
