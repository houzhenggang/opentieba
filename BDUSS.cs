using Newtonsoft.Json.Linq;
using System;
namespace Opentieba
{
    public class LoginResult : TiebaResult
    {
        /// <summary>
        /// 是否需要验证码
        /// </summary>
        public readonly bool needVcode = false;
        /// <summary>
        /// 验证码标识符，传给BDUSS构造函数。
        /// </summary>
        public readonly string vcode_md5 = "";
        /// <summary>
        /// 验证码图片地址
        /// </summary>
        public readonly string vcode_pic_url = "";
        /// <summary>
        /// 错误代码
        /// </summary>
        public readonly int error_code = 0;
        /// <summary>
        /// 错误描述
        /// </summary>
        public readonly string error_msg = "";
        /// <summary>
        /// BDUSS
        /// </summary>
        public readonly string bduss = "";
        /// <summary>
        /// 用户UID
        /// </summary>
        public readonly int uid = 0;
        /// <summary>
        /// 请勿私自调用。
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <param name="c">c</param>
        /// <param name="d">d</param>
        /// <param name="e">e</param>
        /// <param name="f">f</param>
        /// <param name="g">g</param>
        public LoginResult(bool a, string b, string c, int d, string e, string f, int g)
        {
            needVcode = a; vcode_md5 = b; vcode_pic_url = c;
            error_code = d; error_msg = e;
            bduss = f; uid = g;
        }
    }
    public class LoginField : TiebaField
    {
        public readonly LoginResult tbres;
        public LoginField(LoginResult tr, int errcode, string errmsg)
            : base(tr, errcode, errmsg)
        {
            tbres = tr;
        }
    }
    /// <summary>
    /// 指定一个已登录的BDUSS类，可以使用此类发帖等。
    /// </summary>
    public class BDUSS : user
    {
        public readonly string bduss;
        /// <summary>
        /// 该函数将使用给定用户名和密码登录。
        /// </summary>
        /// <param name="uname">用户名</param>
        /// <param name="upass">密码</param>
        public BDUSS(string uname,string upass,string codemd5,string code):base(uname)
        {
            string j=_stbapi.sendTieba("/c/s/login", "un=" + _.encodeURIComponent(uname) + "&passwd=" +
                _.encodeURIComponent(_.base64enc(upass))
                + "&vcode_md5=" + codemd5 + "&vcode=" + _.encodeURIComponent(code),"");
            JObject login = JSON.parse(j);
            if (login["error_code"].Value<int>() != 0)
            {
                try
                {
                    if (login["anti"]["need_vcode"].Value<int>() == 1)
                    {
                        throw new LoginField(new LoginResult(true,login["anti"]["vcode_md5"].Value<string>(),
                            login["anti"]["vcode_pic_url"].Value<string>(),login["error_code"].Value<int>(),
                            login["error_msg"].Value<string>(), "", 0), login["error_code"].Value<int>(),
                            login["error_msg"].Value<string>());
                    }
                    else
                    {
                        throw new LoginField(new LoginResult(false, "", "", login["error_code"].Value<int>(),
                            login["error_msg"].Value<string>(), "", 0), login["error_code"].Value<int>(),
                            login["error_msg"].Value<string>());
                    }
                }
                catch (NullReferenceException e)
                {
                    throw new LoginField(new LoginResult(false, "", "", login["error_code"].Value<int>(),
                            login["error_msg"].Value<string>(), "", 0), login["error_code"].Value<int>(),
                            login["error_msg"].Value<string>());
                }
            }
            else
            {
                this.bduss = login["user"]["BDUSS"].Value<string>();
                this.userid = login["user"]["id"].Value<long>();
                this.username = uname;
            }
        }
        /// <summary>
        /// 使用给定BDUSS和用户名初始化一个BDUSS（通常来自ICID马甲）
        /// </summary>
        /// <param name="bduss">BDUSS</param>
        public BDUSS(string bduss):base(bduss,true)
        {
        }
        public BDUSS(string bduss, string name)
            : base(name)
        {
            this.bduss = bduss;
        }
    }
}
