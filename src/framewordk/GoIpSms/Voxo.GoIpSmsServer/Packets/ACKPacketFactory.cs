using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    /// <summary>
    /// GoIp device messages generator
    /// </summary>
    public static class ACKPacketFactory
    {
        public static string ACK_MESSAGE(long req, int status)
        {
            return string.Format(@"req:{0};status:{1};", req.ToString(), status.ToString());
        }

        public static string BULK_SMS_REQUEST(string sendid, string content)
        {
            string cont = content;
            if (cont.Length > 3000)
            {
                cont = cont.Substring(0, 3000);
            }


            return  string.Format(@"MSG {0} {1} {2}", sendid, cont.Length.ToString(), cont);
        }

        public static string SEND(string rstring, string sendid)
        {
            return string.Format(@"{0} {1}\n", rstring, sendid);
        }

        public static string REQUEST(string rstring, string sendid, string password)
        {
            return string.Format(@"{0} {1} {2}\n", rstring, sendid, password);
        }

        public static string SETREQUEST(string rstring, string sendid, string param)
        {
            return string.Format(@"{0} {1} {2}\n", rstring, sendid, param);
        }

        public static string REQUEST(string rstring, string sendid, string password, string arg0)
        {
            return string.Format(@"{0} {1} {2} {3}\n", rstring, sendid, password, arg0);
        }

        public static string REQUEST(string rstring, string sendid, string password, string arg0, string arg1)
        {
            return string.Format(@"{0} {1} {2} {3} {4}\n", rstring, sendid, password, arg0, arg1);
        }

        public static string REQUEST(string rstring, string sendid, string password, string arg0, string arg1, string arg2)
        {
            return string.Format(@"{0} {1} {2} {3} {4} {5}\n", rstring, sendid, password, arg0, arg1, arg2);
        }

        public static string REQUEST(string rstring, string sendid, string password, string arg0, string arg1, string arg2, string arg3)
        {
            return string.Format(@"{0} {1} {2} {3} {4} {5} {6}\n", rstring, sendid, password, arg0, arg1, arg2, arg3);
        }

        public static string AUTHENTICATION_REQUEST(string sendid, string password)
        {
            return string.Format(@"PASSWORD {0} {1}\n", sendid, password);
        }

        public static string SUBMIT_NUMBER_REQUEST(string sendid, int telid, string telephoneNumber)
        {
            return string.Format(@"SEND {0} {1} {2}", sendid, telid.ToString(), telephoneNumber);
        }

        public static string END_REQUEST(string sendid)
        {
            return string.Format(@"DONE {0}\n", sendid);
        }

        public static string ACK(string CODE, string receiveid, string errorMsg = "")
        {
            if (string.IsNullOrEmpty(errorMsg))
            {
                return string.Format(@"{1} {0} OK\n", receiveid, CODE);
            }
            else
            {
                return string.Format(@"{2} {0} ERROR {1}\n", receiveid, errorMsg, CODE);
            }
        }
    }
}
