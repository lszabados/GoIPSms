using System;
using System.Collections.Generic;
using System.Text;

namespace Voxo.GoIpSmsServer
{
    public class GoIPPacketBase
    {

        public GoIPPacketBase(string data)
        {
            ExtractData(data);
        }

        public virtual void ExtractData(string data)
        {

        }

        /// <summary>
        /// Find row in data rows, and return value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns>return string data</returns>
        protected string FindStringValue(string name, string[] data)
        {
            string ret = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i].StartsWith(name))
                {
                    ret = data[i].Substring(name.Length+1);
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Find row in data rows, and convert value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns>return true if conversion success otherwise false</returns>
        protected bool FindTryIntValue(string name, string[] data, out int value)
        {
            string val = FindStringValue(name, data);

            return int.TryParse(val, out value);   
        }

        /// <summary>
        /// Find row in data rows, and convert value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns>return true if conversion success otherwise false</returns>
        protected bool FindTryLongValue(string name, string[] data, out long value)
        {
            string val = FindStringValue(name, data);

            return long.TryParse(val, out value);
        }

        protected long FindLongValue(string name, string[] data)
        {
            long ret = 0;

            FindTryLongValue(name, data, out ret);

            return ret;
        }

        protected int FindIntValue(string name, string[] data)
        {
            int ret = 0;

            FindTryIntValue(name, data, out ret);

            return ret;
        }

    }

    
}
