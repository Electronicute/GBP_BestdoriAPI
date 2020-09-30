using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace com.pareo.maruyamaAya.Code.Utils
{
    /**
     工具类
         */
    class Tools
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);
        //获取ini文件路径
        private static string inifilepath = System.IO.Directory.GetCurrentDirectory() + @"\profile.ini";

        //获取配置项
        public static string GetValue(string node,string key)
        {
            StringBuilder s = new StringBuilder(1024);
            GetPrivateProfileString(node, key, "", s, 1024, inifilepath);
            return s.ToString();
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="code_type"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }

        //读入文件
        public static byte[] readFile(String filename)
        {
            byte[] bytes = new byte[0];
            try
            {
                using (FileStream fsRead = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    bytes = new byte[fsRead.Length];
                    fsRead.Read(bytes, 0, bytes.Length);
                    return bytes;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 发送http Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CreateGetHttpResponse(string url,string refer,string cookie)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            
            if(GetValue("Proxy", "enable").Equals("1"))
            {
                // proxy设置
                WebProxy proxyObject = new WebProxy(GetValue("Proxy", "host"), int.Parse(GetValue("Proxy", "port")));
                // 设置属性
                request.Proxy = proxyObject;
            }

            request.Method = "GET";
            request.ContentType = "application/json;charset=UTF-8";
            // 三目运算符
            request.Referer = refer!=null?refer:"";
            request.Headers.Add("Cookie", cookie != null ? cookie : "");
            
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36";
            return GetResponseString((HttpWebResponse)request.GetResponse());
        }


        /// <summary>
        /// 从HttpWebResponse对象中提取响应的数据转换为字符串
        /// </summary>
        /// <param name="webresponse"></param>
        /// <returns></returns>
        public static string GetResponseString(HttpWebResponse webresponse)
        {
            using (Stream s = webresponse.GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8); return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Post方式请求接口，返回HttpWebResponse对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static HttpWebResponse HttpPost(string url, IDictionary<string, object> parameters,string refer)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
           
            if (GetValue("Proxy", "enable").Equals("1"))
            {
                // proxy设置
                WebProxy proxyObject = new WebProxy(GetValue("Proxy", "host"), int.Parse(GetValue("Proxy", "port")));
                // 设置属性
                request.Proxy = proxyObject;
            }

            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.Referer = refer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36";
            request.CookieContainer = new CookieContainer();

            // 参数的部分，使用Body {"key1":"value1","key2":"value2"} 方式传输
            StringBuilder builder = new StringBuilder();
            int i = 0;
            builder.Append("{");
            foreach (var item in parameters)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                builder.AppendFormat("\"{0}\":\"{1}\"", item.Key, item.Value);
                i++;
            }
            builder.Append("}");
            // 参数部分组装结束
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            request.ContentLength = data.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            
            return resp;
        }

        /// <summary>
        /// Post方式请求接口
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string HttpPost2(string url, IDictionary<string, object> parameters, string refer,string cookie)
        {
            string result = string.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            
            if (GetValue("Proxy", "enable").Equals("1"))
            {
                // proxy设置
                WebProxy proxyObject = new WebProxy(GetValue("Proxy", "host"), int.Parse(GetValue("Proxy", "port")));
                // 设置属性
                request.Proxy = proxyObject;
            }

            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.Referer = refer;
            //request.CookieContainer = new CookieContainer();

            request.Headers.Add("Cookie", cookie != null ? cookie : "");

            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36";
            

            StringBuilder builder = new StringBuilder();
            int i = 0;
            builder.Append("{");
            foreach (var item in parameters)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }
                if(item.Key.Equals("server"))
                {
                    builder.AppendFormat("\"{0}\":{1}", item.Key, item.Value);
                }
                else
                {
                    builder.AppendFormat("\"{0}\":\"{1}\"", item.Key, item.Value);
                }
                
                i++;
            }
            builder.Append("}");
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            request.ContentLength = data.Length;
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}
