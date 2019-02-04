using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    

    public class GoIpHangupPacket : GoIpPacket
    {
        public GoIpHangupPacket(string data) : base(data, "HANGUP")
        {

        }


        public string num { get; protected set; }
        public string cause { get; protected set; }


        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);
            num = FindStringValue("num", dlist);
            cause = FindStringValue("cause", dlist);
        }

    }
}
