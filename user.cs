using Newtonsoft.Json.Linq;
using System;

namespace Opentieba
{
    public class user
    {
        protected long userid;
        protected string username;
        public long getUid()
        {
            return this.userid;
        }
        public string getUsername()
        {
            return this.username;
        }
        public user(long userid, string username)
        {
            this.userid = userid;
            this.username = username;
        }
        public user(string username)
        {
            this.username = username;
            string j=_.sendHttp("http://tieba.baidu.com/i/sys/user_json?un=" + _.encodeURIComponent(username)
                + "&ie=utf-8", "GET", "", "");
            JObject json = JSON.parse(j);
            try
            {
                this.userid = json["creator"]["id"].Value<long>();
            }
            catch (Exception e)
            {
                this.userid = 0;
            }
        }
        /// <summary>
        /// 此函数仅用作BDUSS类的“BDUSS(string bduss)”构造器调用。
        /// </summary>
        /// <param name="bduss">bduss</param>
        protected user(string bduss,bool tru)
        {
            //TODO: 使用“看机器猫吧”的方式返回USERID和UNAME
        }
    }
}
