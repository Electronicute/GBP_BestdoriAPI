using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.pareo.maruyamaAya.Code.Entity
{
    class BD_User
    {
        /// <summary>
        /// BD用户名
        /// </summary>
        private string username;
        /// <summary>
        /// BD密码
        /// </summary>
        private string password;
        /// <summary>
        /// BD用户名
        /// </summary>
        private string nickname;
        /// <summary>
        /// 绑定的QQ号码
        /// </summary>
        private string bindQQ;
        /// <summary>
        /// 喜欢的乐队
        /// </summary>
        private string favBands;
        /// <summary>
        /// 喜欢的卡
        /// </summary>
        private string favCards;
        /// <summary>
        /// 喜欢的角色
        /// </summary>
        private string favCharacters;
        /// <summary>
        /// 喜欢的服装
        /// </summary>
        private string favCostumes;
        /// <summary>
        /// 喜欢的歌曲
        /// </summary>
        private string favSongs;
        /// <summary>
        /// 关注我的
        /// </summary>
        private int followedByCount;
        /// <summary>
        /// 我关注的
        /// </summary>
        private int followingCount;
        /// <summary>
        /// 自我介绍
        /// </summary>
        private string selfIntro;
        /// <summary>
        /// 社交网络
        /// </summary>
        private string socialMedia;

        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public string Nickname { get => nickname; set => nickname = value; }
        public string BindQQ { get => bindQQ; set => bindQQ = value; }
        public string FavBands { get => favBands; set => favBands = value; }
        public string FavCards { get => favCards; set => favCards = value; }
        public string FavCharacters { get => favCharacters; set => favCharacters = value; }
        public string FavCostumes { get => favCostumes; set => favCostumes = value; }
        public string FavSongs { get => favSongs; set => favSongs = value; }
        public int FollowedByCount { get => followedByCount; set => followedByCount = value; }
        public int FollowingCount { get => followingCount; set => followingCount = value; }
        public string SelfIntro { get => selfIntro; set => selfIntro = value; }
        public string SocialMedia { get => socialMedia; set => socialMedia = value; }
    }
}
