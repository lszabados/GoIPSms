using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPMessagePacket : GoIPPacketBase
    {
        public GoIPMessagePacket(string data) : base(data)
        {

        }

        public string ReceiveId { get; protected set; }
        public string Id { get; protected set; }
        public string Password { get; set; }
        public string Srcnum { get; protected set; }
        public string Message { get; protected set; }

        public override void ExtractData(string data)
        {
            // split into data row
            var dlist = data.Split(new char[] { ';' });

            ReceiveId = FindStringValue("RECEIVE", dlist);
            Id = FindStringValue("id", dlist);
            Password = FindStringValue("password", dlist);
            Srcnum = FindStringValue("srcnum", dlist);
            Message = FindStringValue("msg", dlist);
        }
    }
}
