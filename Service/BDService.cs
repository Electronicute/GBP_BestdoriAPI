using com.pareo.maruyamaAya.Code.Entity;
using com.pareo.maruyamaAya.Code.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace com.pareo.maruyamaAya.Code.Service
{
    class BDService
    {
        /// <summary>
        /// 登录BestDori，并保存Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns>返回值有以下几种，-1为密码错误，0为登录成功，1为数据库写入有问题，2为重复注册</returns>
        public int BD_Login(string name,string password,string bindQQ)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("username", name);
            parameters.Add("password", password);
            // 请求地址
            string url = "https://bestdori.com/api/user/login";
            // 跳转入口
            string refer = "https://bestdori.com/profile/account?g=%2F";
            string result = "";
            HttpWebResponse response = Tools.HttpPost(url, parameters, refer);
            Stream stream = response.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            JObject BD_result = JObject.Parse(result);

            if (BD_result["result"].ToString().Equals("True"))
            {
                //BD 登录Cookie
                string BD_Cookie = response.Cookies["token"].Value;
                
                string sql = "insert into BD_Token(BD_Token,BD_Account) values('" + BD_Cookie + "','" + name + "')";
                try
                {
                    //写Cookie到数据库中
                    if (SqlHelper.ExecuteNonQuery(sql) > 0)
                    {
                        try
                        {
                            //预写账号信息到BD用户表
                            sql = "insert into BD_User(username,password,bindQQ) values('" + name + "','" + password + "','" + bindQQ + "')";
                            SqlHelper.ExecuteNonQuery(sql);
                            return 0;
                        }
                        catch (SqlException)
                        {
                            return 2;
                        }
                    }
                    else
                    {
                        //写入Token表失败
                        return 1;
                    }
                }
                catch(SqlException ex)
                {
                    return 2;
                }
            }
            else
            {
                //账号登录失败
                return -1;
            }
        }
        /// <summary>
        /// 保存BD用户数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool BD_SaveUser(BD_Token token)
        {
            //先根据绑定的QQ取到用户名
            string sql = "select username from BD_User where bindQQ='" + token.User.BindQQ + "'";
            token.User.Username = SqlHelper.ExecuteScalar(sql).ToString();
            //通过用户名取详细信息
            string url = "https://bestdori.com/api/user?username=" + token.User.Username;
            string refer = "https://bestdori.com/community/user/" + token.User.Username;
            //发送请求
            string result = Tools.CreateGetHttpResponse(url, refer, "token=" + token.Token);
            JObject JResult = JObject.Parse(result);

            if (JResult["result"].ToString().Equals("True"))
            {
                token.User.FollowingCount = int.Parse(JResult["followingCount"].ToString());
                token.User.FollowedByCount = int.Parse(JResult["followedByCount"].ToString());
                token.User.Nickname = JResult["nickname"].ToString();
                token.User.SelfIntro = JResult["selfIntro"].ToString();
                token.User.SocialMedia = JResult["socialMedia"].ToString();
                token.User.FavCharacters = JResult["favCharacters"].ToString();
                token.User.FavCards = JResult["favCards"].ToString();
                token.User.FavBands = JResult["favBands"].ToString();
                token.User.FavSongs = JResult["favSongs"].ToString();
                token.User.FavCostumes = JResult["favCostumes"].ToString();
            }
            sql = "update BD_User set followingcount={0},followedbycount={1},nickname='{2}',selfIntro='{3}',socialMedia='{4}',favCharacters='{5}',favCards='{6}',favBands='{7}',favSongs='{8}',favCostumes='{9}' where username = '{10}'";
            sql = String.Format(sql, token.User.FollowingCount, token.User.FollowedByCount, token.User.Nickname, token.User.SelfIntro, token.User.SocialMedia, token.User.FavCharacters, token.User.FavCards, token.User.FavBands, token.User.FavSongs, token.User.FavCostumes, token.User.Username);
            if (SqlHelper.ExecuteNonQuery(sql) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取Bestdori的账号数据（从缓存取）
        /// </summary>
        /// <param name="QQID"></param>
        public BD_User BD_GetUser_FromCache(String QQID)
        {
            BD_User user = new BD_User();
            string sql = "select * from BD_User where bindQQ='" + QQID + "'";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            //检查行数，如果为0直接返回空
            if (dt.Rows.Count == 0)
            {
                return null;
            }

            for(int i = 0; i < dt.Rows.Count; i++)
            {
                user.Username = dt.Rows[i]["username"].ToString();
                user.Password = dt.Rows[i]["password"].ToString();
                user.FollowingCount = int.Parse(dt.Rows[i]["followingCount"].ToString());
                user.FollowedByCount = int.Parse(dt.Rows[i]["followedByCount"].ToString());
                user.Nickname = dt.Rows[i]["nickname"].ToString();
                user.SelfIntro = dt.Rows[i]["selfIntro"].ToString();
                user.SocialMedia = dt.Rows[i]["socialMedia"].ToString();
                user.FavCharacters = dt.Rows[i]["favCharacters"].ToString();
                user.FavCards = dt.Rows[i]["favCards"].ToString();
                user.FavBands = dt.Rows[i]["favBands"].ToString();
                user.FavSongs = dt.Rows[i]["favSongs"].ToString();
                user.FavCostumes = dt.Rows[i]["favCostumes"].ToString();
                user.BindQQ = QQID;
            }
            return user;
        }

        /// <summary>
        /// 更新信息
        /// </summary>
        /// <param name="user"></param>
        /// <returns>-1密码错误，0更新成功，1为更新失败</returns>
        public int BD_UpdData(BD_User user)
        {
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("username", user.Username);
            parameters.Add("password", user.Password);
            // 请求地址
            string url = "https://bestdori.com/api/user/login";
            // 跳转入口
            string refer = "https://bestdori.com/profile/account?g=%2F";
            string result = "";
            HttpWebResponse response = Tools.HttpPost(url, parameters, refer);
            Stream stream = response.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            JObject BD_result = JObject.Parse(result);
            //判断是否登录成功
            if (BD_result["result"].ToString().Equals("True"))
            {
                //BD 登录Cookie
                string BD_Cookie = response.Cookies["token"].Value;
                string sql = String.Format("update BD_Token set BD_Token='{0}' where BD_Account='{1}'", BD_Cookie, user.Username);
                if (SqlHelper.ExecuteNonQuery(sql) > 0)
                {
                    BD_SaveUser(new BD_Token(BD_Cookie, user));
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                //账号登录失败
                return -1;
            }
        }

        /// <summary>
        /// 更新BD密码
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool BD_UpdPassword(BD_User user)
        {
            string sql = String.Format("update BD_User set password = '{0}' where bindQQ='{1}'",user.Password,user.BindQQ);
            return SqlHelper.ExecuteNonQuery(sql) > 0 ? true : false;
        }

        /// <summary>
        /// 通过绑定的QQ号来获取BD令牌（从缓存获取）
        /// </summary>
        /// <param name="bindQQ"></param>
        /// <returns></returns>
        public BD_Token BD_GetToken_FromCache(string bindQQ)
        {
            string sql = "select a.BD_Token,b.username from BD_Token a inner join BD_User b on a.BD_Account=b.username where b.bindQQ='" + bindQQ + "'";
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            string BD_Token = dt.Rows[0]["BD_Token"].ToString();
            BD_User user = new BD_User();
            user.BindQQ = bindQQ;
            user.Username= dt.Rows[0]["username"].ToString();
            return new BD_Token(BD_Token,user);
        }

        /// <summary>
        /// 同步游戏账号数据（公开数据）
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public string GBP_SyncUser(BD_Token cookie)
        {
            try
            {
                //请求位置
                string url = "https://bestdori.com/api/sync/account";
                string refer = "https://bestdori.com/leaderboard/participate";
                //请求到的数据
                JObject result = JObject.Parse(Tools.CreateGetHttpResponse(url, refer, "token=" + cookie.Token));
                //定义一个全局，后面方便取数据
                JArray account_list = null;
                int succ = 0, fail = 0, upd = 0;
                if (result["result"].ToString().Equals("True"))
                {
                    //账号列表
                    account_list = JArray.Parse(result["accounts"].ToString());
                    //遍历数据
                    for (int i = 0; i < account_list.Count; i++)
                    {
                        //解析账号数据
                        JObject account_ = JObject.Parse(account_list[i].ToString());
                        //创建一个用户对象
                        GBP_User user = new GBP_User();
                        //创建一个伺服器对象
                        GBP_Server server = new GBP_Server();
                        //伺服器对象赋值
                        server.Server = int.Parse(account_["server"].ToString());
                        user.Server = server;
                        //获取区服详细数据
                        url = "https://bestdori.com/api/sync/account?server=" + server.Server;
                        JObject temp_serverdata = JObject.Parse(JObject.Parse(Tools.CreateGetHttpResponse(url, refer, "token=" + cookie.Token))["account"].ToString());
                        //加好友的id
                        user.Userid = temp_serverdata["uid"].ToString();
                        //玩家等级
                        if (temp_serverdata["rank"] == null)
                        {
                            user.Userrank = 0;
                        }
                        else
                        {
                            user.Userrank = int.Parse(temp_serverdata["rank"].ToString());
                        }
                        //完成的歌曲数量（ex+sp）
                        if (temp_serverdata["clearCount"]==null) 
                        {
                            user.Complete = 0;
                        }
                        else
                        {
                            user.Complete = int.Parse(temp_serverdata["clearCount"].ToString());
                        }
                        //FC的数量(ex+sp)
                        if (temp_serverdata["fullComboCount"]==null) 
                        {
                            user.Fullcombo = 0;
                        }
                        else
                        {
                            user.Fullcombo = int.Parse(temp_serverdata["fullComboCount"].ToString());
                        }
                        //玩家最高分
                        if (temp_serverdata["hsr"]==null) 
                        {
                            user.Highscore = 0;
                        }
                        else
                        {
                            user.Highscore = int.Parse(temp_serverdata["hsr"].ToString());
                        }

                        string sql = "select * from GBP_User where userid='" + user.Userid + "'";
                        if (SqlHelper.ExecuteScalar(sql) == null)
                        {
                            sql = String.Format("insert into GBP_User(userid,userrank,server,complete,fullcombo,highscore,account) values('{0}',{1},{2},{3},{4},{5},'{6}')", user.Userid, user.Userrank, user.Server.Server, user.Complete, user.Fullcombo, user.Highscore,cookie.User.Username);
                            if (SqlHelper.ExecuteNonQuery(sql) > 0)
                            {
                                succ++;
                            }
                            else
                            {
                                fail++;
                            }
                        }
                        else
                        {
                            sql = String.Format("update GBP_User set userrank={0},complete={1},fullcombo={2},highscore={3} where userid='{4}' and server={5} and account='{6}'", user.Userrank, user.Complete, user.Fullcombo, user.Highscore, user.Userid, user.Server.Server, cookie.User.Username);
                            if (SqlHelper.ExecuteNonQuery(sql) > 0)
                            {
                                upd++;
                            }
                            else
                            {
                                fail++;
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
                return String.Format("同步数据成功，您在Bestdori上面绑定了{0}个游戏数据，已新增了{1}个档案，更新了{2}个档案，失败了{3}个档案！", account_list.Count, succ, upd, fail);
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }
        /// <summary>
        /// 获取游戏账号数据（公开数据）
        /// </summary>
        /// <param name="cookie"></param>
        public string GBP_GetUser_FromCache(string bindQQ,string ServerName)
        {
            string sql = String.Format("select * from GBP_User a inner join GBP_Server b on a.server=b.server inner join BD_User c on a.account=c.username where c.bindQQ='{0}' and b.servername='{1}'",bindQQ,ServerName);
            DataTable dt = SqlHelper.ExecuteDataTable(sql);
            
            return String.Format("{0},欢迎使用丸山彩查询功能，您在BanGDream {1} 上的等级为{2},已完成乐曲数{3},全连击乐曲数{4},最高分:{5}", dt.Rows[0]["nickname"].ToString(), dt.Rows[0]["servername"].ToString(), dt.Rows[0]["userrank"].ToString(), dt.Rows[0]["complete"].ToString(), dt.Rows[0]["fullcombo"].ToString(), dt.Rows[0]["highscore"].ToString());
        }

        /// <summary>
        /// 获取BD上，别人的用户数据
        /// </summary>
        /// <param name="bdAccount"></param>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        public string GBP_GetUser_FromBD(string bdAccount,string ServerName)
        {
            string sql = String.Format("select server from GBP_Server where servername='{0}'",ServerName);
            string url = "https://bestdori.com/api/user/sync?username=" + bdAccount;
            string refer = "https://bestdori.com/leaderboard/participate";

            JObject result = JObject.Parse(Tools.CreateGetHttpResponse(url, refer, null));
            int selServer = int.Parse(SqlHelper.ExecuteScalar(sql).ToString());

            string sresult = "";
            if (result["result"].ToString().Equals("True"))
            {
                string nickname = JObject.Parse(Tools.CreateGetHttpResponse("https://bestdori.com/api/user?username=" + bdAccount, refer, null))["nickname"].ToString();
                JArray result_accounts = JArray.Parse(result["accounts"].ToString());
                for(int i = 0; i < result_accounts.Count; i++)
                {
                    JObject temp = JObject.Parse(result_accounts[i].ToString());
                    //去匹配你要的服务器
                    if (temp["server"].ToString().Equals(selServer+""))
                    {

                        sresult = String.Format("欢迎使用丸山彩查询功能，{0}在BanGDream {1} 上的等级为{2},已完成乐曲数{3},全连击乐曲数{4},最高分:{5}", nickname, ServerName, temp["rank"].ToString(), temp["clearCount"].ToString(), temp["fullComboCount"].ToString(), temp["hsr"].ToString());
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                sresult = "没有找到数据！请使用绑定Bestdori账号的形式查询！";
            }
            return sresult;
        }

        /// <summary>
        /// 游戏数据同步
        /// </summary>
        /// <param name="bindQQ"></param>
        /// <param name="ServerName"></param>
        /// <returns></returns>
        public string GBP_Sync(string bindQQ,string ServerName)
        {
            string url = "https://bestdori.com/api/sync";
            string refer = "https://bestdori.com/leaderboard/participate";

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = String.Format("select server from GBP_Server where servername='{0}'",ServerName);
            parameters.Add("type", "update");
            parameters.Add("server", 3);

            string result = Tools.HttpPost2(url, parameters, refer, "token=" + BD_GetToken_FromCache(bindQQ).Token);
            if (JObject.Parse(result)["result"].ToString().Equals("True"))
            {
                //result = "更新成功！";
                JObject JResult = JObject.Parse(Tools.CreateGetHttpResponse(url,refer, "token=" + BD_GetToken_FromCache(bindQQ).Token));
                if (JResult["result"].ToString().Equals("True"))
                {
                    
                    JObject syncRequest = JObject.Parse(JResult["syncRequest"].ToString());
                    //提交完同步操作，会进入一个同步中的情况，需要轮询该字段是否完毕
                    while (syncRequest["status"].ToString().Equals("IN_QUEUE"))
                    {
                        JResult = JObject.Parse(Tools.CreateGetHttpResponse(url, refer, "token=" + BD_GetToken_FromCache(bindQQ).Token));
                        syncRequest = JObject.Parse(JResult["syncRequest"].ToString());
                    }
                    // 查询中的状态变了之后会进入此判断
                    if (syncRequest["status"].ToString().Equals("COMPLETED"))
                    {
                        result = "同步成功！";
                    }
                    else
                    {
                        result = "同步失败，有可能是bestdori查询用小号被封禁！";
                    }
                }
                else
                {
                    result = "查询失败！";
                }
            }
            else
            {
                result = "提交失败！";
            }
            return result;
        }
    }
}
