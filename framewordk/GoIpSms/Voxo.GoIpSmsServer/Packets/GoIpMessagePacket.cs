using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    /// <summary>
    /// GoIp device SMS message packet
    /// </summary>
    public class GoIpMessagePacket : GoIpPacket
    {
        public GoIpMessagePacket(string data) : base(data, "RECEIVE")
        {

        }

        
        public string Srcnum { get; protected set; }
        public string Message { get; protected set; }

        public override void ExtractData(string[] dlist)
        {
            // split into data row
            base.ExtractData(dlist);
            Srcnum = FindStringValue("srcnum", dlist);
            Message = FindStringValue("msg", dlist);
        }
    }
}
