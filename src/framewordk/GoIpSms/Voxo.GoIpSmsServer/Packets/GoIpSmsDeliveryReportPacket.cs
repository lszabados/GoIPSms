using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIpSmsDeliveryReportPacket : GoIpPacket
    {
        public GoIpSmsDeliveryReportPacket(string data) : base(data)
        {

        }

       
        public string sms_no { get; protected set; }
        public string deliver_state { get; protected set; }
        public string send_num { get; protected set; }

        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);
            sms_no = FindStringValue("sms_no", dlist);
            deliver_state = FindStringValue("state", dlist);
            send_num = FindStringValue("num", dlist);
        }

    }
}
