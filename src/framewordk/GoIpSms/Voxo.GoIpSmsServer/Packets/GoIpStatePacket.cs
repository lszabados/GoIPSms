using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpStatePacket : GoIpPacket
    {
        public GoIpStatePacket(string data) : base(data)
        {

        }

        public string gsm_remain_state { get; protected set; }

        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);
            // split into data row
            gsm_remain_state = FindStringValue("gsm_remain_state", dlist);
        }
    }
}
