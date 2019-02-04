using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    class GoIpCellListPacket : GoIpPacket
    {
        public GoIpCellListPacket(string data) : base(data, "CELLS")
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
