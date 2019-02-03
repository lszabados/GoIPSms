using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    /// <summary>
    /// GoIP device SMS message packet
    /// </summary>
    public class GoIPMessagePacket : GoIPPacket
    {
        public GoIPMessagePacket(string data) : base(data)
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
