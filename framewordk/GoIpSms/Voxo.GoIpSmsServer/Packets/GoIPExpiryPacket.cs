using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpExpiryPacket : GoIpPacket
    {
        public GoIpExpiryPacket(string data) : base(data, "EXPIRY")
        {

        }

        public string exp { get; protected set; }
 
        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);
            exp = FindStringValue("exp", dlist);
        }

    }
}
