using Newtonsoft.Json.Linq;
using System;

namespace Opentieba
{
    /// <summary>
    /// 表示一个用户
    /// </summary>
    public class user
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        protected long userid;
        /// <summary>
        /// 用户名
        /// </summary>
        protected String username;
        /// <summary>
        /// 获得UID
        /// </summary>
        /// <returns>UID</returns>
        public long getUid()
        {
            return this.userid;
        }
        /// <summary>
        /// 获得此对象表示的用户的用户名
        /// </summary>
        /// <returns>用户名</returns>
        public String getUsername()
        {
            return this.username;
        }
        /// <summary>
        /// 构造一个user，省略查询uid的步骤
        /// </summary>
        /// <param name="userid">客户端UID</param>
        /// <param name="username">用户名</param>
        public user(long userid, String username)
        {
            this.userid = userid;
            this.username = username;
        }
        public user(long userid)
        {
            this.userid = userid;
            String j = _stbapi.sendTieba("/c/u/user/profile", "uid=" + userid, "");
            JObject json = JSON.parse(j);
            try
            {
                this.username = json["user"]["name"].Value<String>();
            }
            catch (Exception)
            {
                this.username = "";
            }
        }
        /// <summary>
        /// 构造一个user
        /// </summary>
        /// <param name="username">用户名</param>
        public user(String username)
        {
            this.username = username;
            String j = _.sendHttp("http://tieba.baidu.com/i/sys/user_json?un=" + _.encodeURIComponent(username)
                + "&ie=utf-8", "", "");
            JObject json = JSON.parse(j);
            try
            {
                this.userid = json["creator"]["id"].Value<long>();
            }
            catch (Exception)
            {
                this.userid = 0;
            }
        }
        /// <summary>
        /// [内部调用]BDUSS类的“BDUSS(String bduss)”构造器调用。
        /// </summary>
        /// <param name="bduss">bduss</param>
        /// <param name="tru">区分函数</param>
        protected user(String bduss,bool tru)
        {
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent("机器猫") +
                "&is_good=0&pn=1", bduss);
            JToken barinfo = JSON.parse(jon);
            if (barinfo["error_code"].Value<int>() != 0)
            {
                throw new SeeBarField("机器猫", barinfo["error_code"].Value<int>(), barinfo["error_msg"].Value<String>());
            }
            try
            {
                userid = barinfo["user"]["id"].Value<long>();
                username = barinfo["user"]["name"].Value<String>();
            }
            catch (NullReferenceException)
            {
                throw new LoginField(new LoginResult(false,"","",-1,"","",-1), -1, "BDUSS不正确");
            }
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
