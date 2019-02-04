using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpSendMessageEventArgs : EventArgs
    {
        public GoIpSendMessageEventArgs(string phoneNumber, string sendID, string status)
        {
            PhoneNumber = phoneNumber;
            SendId = sendID;
            Status = status;
        }

        public string PhoneNumber { get; }
        public string SendId { get; }
        public string Status { get; }
    }
}
