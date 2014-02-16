using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
        public readonly String vcode_md5 = "";
        /// <summary>
        /// 验证码图片地址
        /// </summary>
        public readonly String vcode_pic_url = "";
        /// <summary>
        /// 错误代码
        /// </summary>
        public readonly int error_code = 0;
        /// <summary>
        /// 错误描述
        /// </summary>
        public readonly String error_msg = "";
        /// <summary>
        /// BDUSS
        /// </summary>
        public readonly String bduss = "";
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
        public LoginResult(bool a, String b, String c, int d, String e, String f, int g)
        {
            needVcode = a; vcode_md5 = b; vcode_pic_url = c;
            error_code = d; error_msg = e;
            bduss = f; uid = g;
        }
    }
    public class addThreadResult : TiebaResult
    {
        /// <summary>
        /// 是否需要验证码
        /// </summary>
        public readonly bool needVcode = false;
        /// <summary>
        /// 验证码标识符，传给addThread函数。
        /// </summary>
        public readonly String vcode_md5 = "";
        /// <summary>
        /// 验证码图片地址
        /// </summary>
        public readonly String vcode_pic_url = "";
        /// <summary>
        /// 错误代码
        /// </summary>
        public readonly int error_code;
        /// <summary>
        /// 错误描述
        /// </summary>
        public readonly String error_msg;
        /// <summary>
        /// 新发帖号
        /// </summary>
        public readonly long kz;
        /// <summary>
        /// 贴吧
        /// </summary>
        public readonly bar kw;
        public addThreadResult(int cd, String ms, long kz, bar kw, bool needVcd, String vcd, String vcmd5)
        {
            error_code = cd;
            error_msg = ms;
            this.kz = kz;
            this.kw = kw;
            needVcode = needVcd;
            vcode_pic_url = vcd;
            vcode_md5 = vcmd5;
        }
    }
    public class LoginField : TiebaField
    {
        public readonly LoginResult tbres;
        public LoginField(LoginResult tr, int errcode, String errmsg)
            : base(tr, errcode, errmsg)
        {
            tbres = tr;
        }
    }
    public class AddThreadField : TiebaField
    {
        public readonly addThreadResult tbres;
        public AddThreadField(addThreadResult tr, int errcode, String errmsg)
            : base(tr, errcode, errmsg)
        {
            tbres = tr;
        }
    }
    public class addPostField : TiebaField
    {
        public readonly String vcodeurl, vcodemd5;
        public addPostField(int code, String msg, String vcodeurl, String vcodemd5)
            : base(new EntryResult(), code, msg)
        {
            this.vcodeurl = vcodeurl;
            this.vcodemd5 = vcodemd5;
        }
    }
    /// <summary>
    /// 指定一个已登录的BDUSS类，可以使用此类发帖等。
    /// </summary>
    public class BDUSS : user
    {
        public readonly String bduss;
        /// <summary>
        /// 该函数将使用给定用户名和密码登录。
        /// </summary>
        /// <param name="uname">用户名</param>
        /// <param name="upass">密码</param>
        public BDUSS(String uname, String upass, String codemd5, String code)
            : base(uname)
        {
            String j = _stbapi.sendTieba("/c/s/login", "un=" + _.encodeURIComponent(uname) + "&passwd=" +
                _.encodeURIComponent(_.base64enc(upass))
                + "&vcode_md5=" + codemd5 + "&vcode=" + _.encodeURIComponent(code), "");
            JObject login = JSON.parse(j);
            if (login["error_code"].Value<int>() != 0)
            {
                try
                {
                    if (login["anti"]["need_vcode"].Value<int>() == 1)
                    {
                        throw new LoginField(new LoginResult(true, login["anti"]["vcode_md5"].Value<String>(),
                            login["anti"]["vcode_pic_url"].Value<String>(), login["error_code"].Value<int>(),
                            login["error_msg"].Value<String>(), "", 0), login["error_code"].Value<int>(),
                            login["error_msg"].Value<String>());
                    }
                    else
                    {
                        throw new LoginField(new LoginResult(false, "", "", login["error_code"].Value<int>(),
                            login["error_msg"].Value<String>(), "", 0), login["error_code"].Value<int>(),
                            login["error_msg"].Value<String>());
                    }
                }
                catch (NullReferenceException e)
                {
                    throw new LoginField(new LoginResult(false, "", "", login["error_code"].Value<int>(),
                            login["error_msg"].Value<String>(), "", 0), login["error_code"].Value<int>(),
                            login["error_msg"].Value<String>());
                }
            }
            else
            {
                this.bduss = login["user"]["BDUSS"].Value<String>();
                this.userid = login["user"]["id"].Value<long>();
                this.username = uname;
            }
        }
        /// <summary>
        /// 使用给定BDUSS和用户名初始化一个BDUSS（通常来自ICID马甲）
        /// </summary>
        /// <param name="bduss">BDUSS</param>
        public BDUSS(String bduss)
            : base(bduss, true)
        {
        }
        public BDUSS(String bduss, String name)
            : base(name)
        {
            this.bduss = bduss;
        }
        /// <summary>
        /// 发帖
        /// </summary>
        /// <param name="kw">吧对象</param>
        /// <param name="content">内容</param>
        /// <param name="title">标题</param>
        /// <param name="vcode">验证码</param>
        /// <param name="vcodemd5">验证码标识</param>
        /// <returns></returns>
        public addThreadResult addThread(bar kw, String content, String title, String vcode, String vcodemd5)
        {
            String tbs = getTbs();
            JObject res = JSON.parse(_stbapi.sendTieba("/c/c/thread/add", "content=" + _.encodeURIComponent(content) + "&" +
                "kw=" + _.encodeURIComponent(kw.kw) + "&vcode=" + _.encodeURIComponent(vcode) + "&vcode_md5=" +
                _.encodeURIComponent(vcodemd5) + "&tbs=" + _.encodeURIComponent(tbs) + "&fid=" +
                kw.fid + "&title=" + _.encodeURIComponent(title), bduss));
            if (res["error_code"].Value<int>() != 0)
            {
                try
                {
                    if (res["info"]["need_vcode"].Value<bool>())
                    {
                        throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                            0, kw, true, res["info"]["vcode_pic_url"].Value<String>(), res["info"]["vcode_md5"].Value<String>())
                            , res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                }
                catch (NullReferenceException e)
                {
                    throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                        0, kw, false, "", ""), res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                }
                catch (ArgumentNullException e)
                {
                    throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                        0, kw, false, "", ""), res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                }
                catch (InvalidOperationException e)
                {
                    throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                        0, kw, false, "", ""), res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                }
            }
            return new addThreadResult(0, "", res["tid"].Value<long>(), kw, false, "", "");
        }
        public void addPost(String content, TieThread thread, long toFloor, TiePost FloorPost, String vcodemd5, String vcode)
        {
            long tofloorpid = 0;
            if (FloorPost != null)
            {
                tofloorpid = FloorPost.id;
            }
            String tbs = getTbs();
            JToken resjt = JSON.parse(_stbapi.sendTieba("/c/c/post/add", "content=" + _.encodeURIComponent(content) + "&floor_num=" +
                toFloor + "&kw=" + thread.kw.kw + "&tid=" + thread.tid + "&vcode=" + _.encodeURIComponent(vcode) + "&vcode_md5=" + vcodemd5 +
                "&tbs=" + tbs + "&quote_id=" + tofloorpid + "&fid=" + thread.kw.fid, bduss));
            if (resjt["error_code"].Value<int>() != 0)
            {
                try
                {
                    if (resjt["info"]["need_vcode"].Value<bool>())
                    {
                        /*throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                            0, kw, true, res["info"]["vcode_pic_url"].Value<String>(), res["info"]["vcode_md5"].Value<String>())
                            , res["error_code"].Value<int>(), res["error_msg"].Value<String>());*/
                        throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            resjt["info"]["vcode_pic_url"].Value<String>(), resjt["info"]["vcode_md5"].Value<String>());
                    }
                    else
                    {
                        throw new NullReferenceException();
                    }
                }
                catch (NullReferenceException e)
                {
                    throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            "", "");
                }
                catch (ArgumentNullException e)
                {
                    throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            "", "");
                }
                catch (InvalidOperationException e)
                {
                    throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            "", "");
                }
            }
        }
        /// <summary>
        /// 获得TBS
        /// </summary>
        /// <returns>tbs字符串</returns>
        public String getTbs()
        {
            JObject jt = JSON.parse(_stbapi.sendTieba("/c/u/user/profile", "uid=" + userid, bduss));
            try
            {
                return jt["anti"]["tbs"].Value<String>();
            }
            catch (NullReferenceException e)
            {
                throw new TiebaField(new EntryResult(), -1, "BDUSS不正确");
            }
        }
        public ListBarResult listLike(int page)
        {
            JObject bjx = JSON.parse(_stbapi.sendTieba("/c/f/forum/favolike", "pn=" + page, bduss));
            if (bjx["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), bjx["error_code"].Value<int>(), bjx["error_msg"].Value<String>());
            }
            JEnumerable<JToken> jet = bjx["forum_list"].Children();
            List<baseBar> lb = new List<baseBar>();
            foreach (JToken tok in jet)
            {
                lb.Add(new baseBar(tok["name"].Value<String>(),tok["forum_id"].Value<long>()));
            }
            return new ListBarResult(lb.ToArray(), bjx["page"]["total_page"].Value<int>());
        }
    }
    public class ListBarResult : TiebaResult
    {
        public readonly baseBar[] likebars;
        public readonly int maxPage;
        public ListBarResult(baseBar[] lb, int pg)
        {
            likebars = lb;
            maxPage = pg;
        }
    }
}
