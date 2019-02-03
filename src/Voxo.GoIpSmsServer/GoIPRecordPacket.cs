using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public enum GoIPRecordDirection : int { INCOMING = 1, OUTGOING} 

    public class GoIPRecordPacket : GoIPPacketBase
    {
        public GoIPRecordPacket(string data) : base(data)
        {

        }

        public int receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }
        public GoIPRecordDirection direction { get; protected set; }
        public string send_num { get; protected set; }

        public override void ExtractData(string data)
        {
            // split into data row
            var dlist = data.Split(new char[] { ';' });

            receiveid = FindIntValue("RECORD", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
            direction = (GoIPRecordDirection)Enum.Parse(typeof(GoIPRecordDirection), FindStringValue("dir", dlist));
            send_num = FindStringValue("num", dlist);
        }
    }
}
