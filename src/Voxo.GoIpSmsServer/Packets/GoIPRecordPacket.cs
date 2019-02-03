using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public enum GoIPRecordDirection : int { INCOMING = 1, OUTGOING} 

    public class GoIPRecordPacket : GoIPPacket
    {
        public GoIPRecordPacket(string data) : base(data)
        {

        }

        
        public GoIPRecordDirection direction { get; protected set; }
        public string send_num { get; protected set; }

        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);

            direction = (GoIPRecordDirection)Enum.Parse(typeof(GoIPRecordDirection), FindStringValue("dir", dlist));
            send_num = FindStringValue("num", dlist);
        }
    }
}
