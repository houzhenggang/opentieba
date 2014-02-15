using Newtonsoft.Json.Linq;
using System;

namespace Opentieba
{
    public class user
    {
        protected long userid;
        protected String username;
        public long getUid()
        {
            return this.userid;
        }
        public String getUsername()
        {
            return this.username;
        }
        public user(long userid, String username)
        {
            this.userid = userid;
            this.username = username;
        }
        public user(String username)
        {
            this.username = username;
            String j=_.sendHttp("http://tieba.baidu.com/i/sys/user_json?un=" + _.encodeURIComponent(username)
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
        /// 此函数仅用作BDUSS类的“BDUSS(String bduss)”构造器调用。
        /// </summary>
        /// <param name="bduss">bduss</param>
        protected user(String bduss,bool tru)
        {
            //TODO: 使用“看机器猫吧”的方式返回USERID和UNAME
        }
    }
    public class userInBar : userWithPic
    {
        public readonly bool is_like;
        public readonly int level_id;
        public userInBar(long uid, String n, bool islike, int lid, String pic)
            : base(uid, n, pic)
        {
            is_like = islike;
            level_id = lid;
        }
    }
    public class userWithPic : user
    {
        public readonly String portrait;
        public userWithPic(long uid, String n, String pic)
            : base(uid, n)
        {
            portrait = pic;
        }
    }
}
