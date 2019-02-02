using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public static class ACKPacketFactory
    {
        public static string ACK_MESSAGE(long req, int status)
        {
            return string.Format(@"req:{0};status:{1};", req.ToString(), status.ToString());
        }

        public static string BULK_SMS_REQUEST(int sendid, string content)
        {
            // max length !!
            string cont = content.Substring(0, 3000);

            return  string.Format(@"MSG {0} {1} {2}\n", sendid.ToString(), cont.Length.ToString(), cont);
        }

        public static string AUTHENTICATION_REQUEST(int sendid, string password)
        {
            return string.Format(@"PASSWORD {0} {1}\n", sendid.ToString(), password);
        }

        public static string SUBMIT_NUMBER_REQUEST(int sendid, int telid, string telephoneNumber)
        {
            return string.Format(@"SEND {0} {1} {2}", sendid.ToString(), telid.ToString(), telephoneNumber);
        }

        public static string END_REQUEST(int sendid)
        {
            return string.Format(@"DONE {0}\n", sendid.ToString());
        }

        public static string RECEIVE_SMS_ACK(string receiveid, string errorMsg = "")
        {
            if (string.IsNullOrEmpty(errorMsg))
            {
                return string.Format(@"RECEIVE {0} OK\n", receiveid);
            }
            else
            {
                return string.Format(@"RECEIVE {0} ERROR {1}\n", receiveid, errorMsg);
            }
        }



    }
}
