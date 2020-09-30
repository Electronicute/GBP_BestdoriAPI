using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.pareo.maruyamaAya.Code.Entity
{
    class GBP_User
    {
        /// <summary>
        /// 邦邦加好友的id
        /// </summary>
        private string userid;
        /// <summary>
        /// 用户等级
        /// </summary>
        private int userrank;
        /// <summary>
        /// 账号所在的服务器
        /// </summary>
        private GBP_Server server;
        /// <summary>
        /// 账号完成的乐曲数（EX+SP）
        /// </summary>
        private int complete;
        /// <summary>
        /// 账号全连的乐曲数（EX+SP)
        /// </summary>
        private int fullcombo;
        /// <summary>
        /// 账号最高分
        /// </summary>
        private int highscore;
        /// <summary>
        /// 账号绑定的BD账号
        /// </summary>
        private string account;

        public string Userid { get => userid; set => userid = value; }
        public GBP_Server Server { get => server; set => server = value; }
        public int Complete { get => complete; set => complete = value; }
        public int Fullcombo { get => fullcombo; set => fullcombo = value; }
        public int Highscore { get => highscore; set => highscore = value; }
        public int Userrank { get => userrank; set => userrank = value; }
        public string Account { get => account; set => account = value; }
    }
}
