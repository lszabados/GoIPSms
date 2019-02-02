using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPSmsSendEndEventArgs : EventArgs
    {
        public GoIPSmsSendEndEventArgs(string sendId)
        {
            SendId = sendId;
        }

        public string SendId { get; }
    }
}
