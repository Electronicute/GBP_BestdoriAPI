using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.pareo.maruyamaAya.Code.Entity
{
    /// <summary>
    /// BestDori 用户与令牌绑定表
    /// </summary>
    class BD_Token
    {
        /// <summary>
        /// Bestdori用户
        /// </summary>
        private BD_User user;
        /// <summary>
        /// Bestdori登录令牌
        /// </summary>
        private string token;

        public string Token { get => token; set => token = value; }
        public BD_User User { get => user; set => user = value; }

        public BD_Token() { }
        public BD_Token(string token,BD_User user)
        {
            this.token = token;
            this.user = user;
        }
    }
}
