using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    class GoIPCellListPacket : GoIPPacketBase
    {
        public GoIPCellListPacket(string data) : base(data)
        {

        }

        public int receiveid { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }
        public string celllist { get; protected set; }


        public override void ExtractData(string data)
        {
            // split into data row
            var dlist = data.Split(new char[] { ';' });

            receiveid = FindIntValue("CELLS", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("password", dlist);
            celllist = FindStringValue("lists", dlist);
        }

    }
}
