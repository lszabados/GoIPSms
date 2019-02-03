using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPSmsDeliveryReportPacket : GoIPPacketBase
    {
        public GoIPSmsDeliveryReportPacket(string data) : base(data)
        {

        }

        public int receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }
        public string sms_no { get; protected set; }
        public string deliver_state { get; protected set; }
        public string send_num { get; protected set; }

        public override void ExtractData(string data)
        {
            // split into data row
            var dlist = data.Split(new char[] { ';' });

            receiveid = FindIntValue("DELIVER", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
            sms_no = FindStringValue("sms_no", dlist);
            deliver_state = FindStringValue("state", dlist);
            send_num = FindStringValue("num", dlist);
        }

    }
}
