using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.pareo.maruyamaAya.Code.Entity
{
    /// <summary>
    /// GBP服务器映射
    /// </summary>
    class GBP_Server
    {
        /// <summary>
        /// 服务器编号
        /// </summary>
        private int server;
        /// <summary>
        /// 服务器名称
        /// </summary>
        private string servername;

        public int Server { get => server; set => server = value; }
        public string Servername { get => servername; set => servername = value; }

        public GBP_Server() { }
        public GBP_Server(int server,string servername)
        {
            this.server = server;
            this.servername = servername;
        }
    }
}
