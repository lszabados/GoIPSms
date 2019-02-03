using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    class GoIPCellListPacket : GoIPPacket
    {
        public GoIPCellListPacket(string data) : base(data)
        {

        }
 
        public string celllist { get; protected set; }


        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);
            celllist = FindStringValue("lists", dlist);
        }

    }
}
