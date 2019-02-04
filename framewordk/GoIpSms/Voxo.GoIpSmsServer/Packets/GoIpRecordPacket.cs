using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public enum GoIpRecordDirection : int { INCOMING = 1, OUTGOING} 

    public class GoIpRecordPacket : GoIpPacket
    {
        public GoIpRecordPacket(string data) : base(data, "RECORD")
        {

        }

        
        public GoIpRecordDirection direction { get; protected set; }
        public string send_num { get; protected set; }

        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);

            direction = (GoIpRecordDirection)Enum.Parse(typeof(GoIpRecordDirection), FindStringValue("dir", dlist));
            send_num = FindStringValue("num", dlist);
        }
    }
}
