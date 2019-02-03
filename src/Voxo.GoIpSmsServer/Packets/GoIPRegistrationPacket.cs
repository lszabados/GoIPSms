using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    /// <summary>
    /// GoIP device Registration packet
    /// </summary>
    public class GoIPRegistrationPacket : GoIPPacketBase
    {

        public GoIPRegistrationPacket(string data):base(data)
        {

        }

        public long req { get; protected set; }
        public string authid { get; protected set; }
        public string password { get; set; }
        public string gsm_num { get; protected set; }
        public string gsm_signal { get; protected set; }
        public string gsm_status { get; protected set; }
        public string voip_status { get; protected set; }
        public string imei { get; protected set; }
        public string imsi { get; protected set; }
        public string iccid { get; protected set; }
        public string voip_state { get; protected set; }
        public string pro { get; protected set; }
        public string remain_time { get; protected set; }
        public string idle { get; protected set; }
        public string disable_status { get; protected set; }
        public string SMS_LOGIN { get; protected set; }
        public string CELLINFO { get; protected set; }
        public string CELL_ID { get; protected set; }
        public string CGATT { get; protected set; }

        public override void ExtractData(string[] dlist)
        {
            gsm_signal = FindStringValue("signal", dlist);
            gsm_status = FindStringValue("gsm_status", dlist);
            voip_status = FindStringValue("voip_status", dlist);
            voip_state = FindStringValue("voip_state", dlist);
            req = FindLongValue("req", dlist);
            authid = FindStringValue("id", dlist);
            password = FindStringValue("pass", dlist);
            gsm_num = FindStringValue("num", dlist);
            iccid = FindStringValue("iccid", dlist);
            imei = FindStringValue("imei", dlist);
            remain_time = FindStringValue("remain_time", dlist);
            pro = FindStringValue("pro", dlist);
            idle = FindStringValue("idle", dlist);
            disable_status = FindStringValue("disable_status", dlist);
            SMS_LOGIN = FindStringValue("SMS_LOGIN", dlist);
            CELLINFO = FindStringValue("CELLINFO", dlist);
            CELL_ID = FindStringValue("CELL ID", dlist);
            CGATT = FindStringValue("CGATT", dlist);
        }
    }
}
