using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSms
{
    public class GoIpCellListEvenetArgs : EventArgs
    {
        public GoIpCellListEvenetArgs(string message, string cellList, string host, int port)
        {
            Message = message;
            celllist = cellList;
            Host = host;
            Port = port;
        }

        public string Message { get; }
        public string celllist { get; }
        public string Host { get; }
        public int Port { get; }
    }
}

