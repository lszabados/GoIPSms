﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpRemainPacket : GoIpPacket
    {
        public GoIpRemainPacket(string data) : base(data,"REMAIN")
        {

        }

       
        public string gsm_remain_time { get; protected set; }
        

        public override void ExtractData(string[] dlist)
        {
            base.ExtractData(dlist);
            gsm_remain_time = FindStringValue("gsm_remain_time", dlist);
        }

    }
}
