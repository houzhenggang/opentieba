using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
namespace Opentieba
{
    /// <summary>
    /// 登录结果
    /// </summary>
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
    /// <summary>
    /// 列出消息数目结果
    /// </summary>
    public class messageNumResult : TiebaResult
    {
#pragma warning disable 1591
        public readonly long atme, bookmark, count, fans, pletter, replyme;
#pragma warning restore 1591
        public messageNumResult(long am, long bm, long ct, long fa, long pl, long re)
        {
            atme = am;
            bookmark = bm;
            count = ct;
            fans = fa;
            pletter = pl;
            replyme = re;
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
        public readonly kwf kw;
        public addThreadResult(int cd, String ms, long kz, kwf kw, bool needVcd, String vcd, String vcmd5)
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
    public class MaxPageAndListResult<arrtype> : TiebaResult
    {
        public readonly List<arrtype> list;
        public readonly long maxPage;
        public MaxPageAndListResult(List<arrtype> lst, long maxPage)
        {
            list = lst;
            this.maxPage = maxPage;
        }
    }
    public class msgPost
    {
        public readonly String contentStr;
        public readonly String fname;
        public readonly bool is_floor;
        public readonly long post_id;
        public readonly userWithPic replyer;
        public readonly long thread_id;
        public readonly long time;
        public readonly String title;
        public msgPost(String content, String kw, bool isfloor, long pid, long tme, String tit, long tid, userWithPic uwp)
        {
            contentStr = content;
            fname = kw;
            is_floor = isfloor;
            post_id = pid;
            replyer = uwp;
            thread_id = tid;
            time = tme;
            title = tit;
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
                catch (NullReferenceException)
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

        public String encgetCookie()
        {
            return "BDUSS=" + _.encodeURIComponent(this.bduss) + "; TIEBA_USERTYPE=737e8e03175d6b15cc177324; TIEBAUID=c5f54a97e163720d29161d75; BAIDUID=00F2392CACC8FD95E6E4E415D301804E:FG=1;";
        }

        /// <summary>
        /// 使用给定BDUSS初始化一个BDUSS（通常来自ICID马甲）
        /// </summary>
        /// <param name="bduss">BDUSS</param>
        public BDUSS(String bduss)
            : base(bduss, true)
        {
            this.bduss = bduss;
        }
        /// <summary>
        /// 使用给定BDUSS和用户名初始化一个BDUSS（通常来自ICID马甲）
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="bduss">BDUSS</param>
        public BDUSS(String name, String bduss):base(name)
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
        public addThreadResult addThread(kwf kw, String content, String title, String vcode, String vcodemd5)
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
                    if (res["info"]["need_vcode"].Value<int>()>0)
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
                catch (NullReferenceException)
                {
                    throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                        0, kw, false, "", ""), res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                }
                catch (ArgumentNullException)
                {
                    throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                        0, kw, false, "", ""), res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                }
                catch (InvalidOperationException)
                {
                    throw new AddThreadField(new addThreadResult(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                        0, kw, false, "", ""), res["error_code"].Value<int>(), res["error_msg"].Value<String>());
                }
                catch (ArgumentException)
                {
                    throw new addPostField(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                            "", "");
                }
                catch (InvalidCastException)
                {
                    throw new addPostField(res["error_code"].Value<int>(), res["error_msg"].Value<String>(),
                                                "", "");
                }
            }
            return new addThreadResult(0, "", res["tid"].Value<long>(), kw, false, "", "");
        }
        public long checkSign(String kw)
        {
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=1", bduss);
            JObject barinfo = JSON.parse(jon);
            if (barinfo["error_code"].Value<int>() != 0)
            {
                throw new SeeBarField(kw, barinfo["error_code"].Value<int>(), barinfo["error_msg"].Value<String>());
            }
            if (barinfo["forum"]["sign_in_info"]["user_info"]["is_sign_in"].Value<int>()>0)
            {
                return barinfo["forum"]["sign_in_info"]["user_info"]["user_sign_rank"].Value<long>();
            }
            else
            {
                return 0;
            }
        }
        public void addPost(String content, tkwtid thread, long toFloor, TiePost FloorPost, String vcodemd5, String vcode)
        {
            long tofloorpid = 0;
            if (FloorPost != null)
            {
                tofloorpid = FloorPost.id;
            }
            String tbs = getTbs();
            JToken resjt = JSON.parse(_stbapi.sendTieba("/c/c/post/add", "content=" + _.encodeURIComponent(content) + "&floor_num=" +
                toFloor + "&kw=" + _.encodeURIComponent(thread.kw.kw) + "&tid=" + thread.tid + "&vcode=" + _.encodeURIComponent(vcode) + "&vcode_md5=" + vcodemd5 +
                "&tbs=" + tbs + "&quote_id=" + tofloorpid + "&fid=" + thread.kw.fid, bduss));
            if (resjt["error_code"].Value<int>() != 0)
            {
                try
                {
                    if (resjt["info"]["need_vcode"].Value<int>()>0)
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
                catch (NullReferenceException)
                {
                    throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            "", "");
                }
                catch (ArgumentNullException)
                {
                    throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            "", "");
                }
                catch (InvalidOperationException)
                {
                    throw new addPostField(resjt["error_code"].Value<int>(), resjt["error_msg"].Value<String>(),
                            "", "");
                }
                catch (ArgumentException)
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
            //WebClient wclient = new WebClient();
            //wclient.Headers.Add("Cookie", encgetCookie());
            //String json;
            //try
            //{
            //    json = wclient.DownloadString("http://tieba.baidu.com/dc/common/tbs");
            //}
            //catch (WebException e)
            //{
            //    throw;
            //}
            //JObject jt = JSON.parse(json);
            //try
            //{
            //    if (!jt["is_login"].Value<int>()>0) { throw new NullReferenceException(); }
            //    return jt["tbs"].Value<String>();
            //}
            //catch (NullReferenceException)
            //{
            //    throw new TiebaField(new EntryResult(), -1, "BDUSS不正确");
            //}
            //! 这样子有问题
            JObject jt = JSON.parse(_stbapi.sendTieba("/c/u/user/profile", "uid=" + userid, bduss));
            try
            {
                return jt["anti"]["tbs"].Value<String>();
            }
            catch (NullReferenceException)
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
                lb.Add(new baseBar(tok["name"].Value<String>(), tok["forum_id"].Value<long>(), tok["level_id"].Value<int>()));
            }
            return new ListBarResult(lb.ToArray(), bjx["page"]["total_page"].Value<int>());
        }
        public messageNumResult msgNum() {
            JObject jb = JSON.parse(_stbapi.sendTieba("/c/s/msg", "", bduss));
            if (jb["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jb["error_code"].Value<int>(), jb["error_msg"].Value<String>());
            }
            JToken jo = JSON.parse(_stbapi.sendTieba("/c/s/msg", "", bduss))["message"];
            return new messageNumResult(jo["atme"].Value<long>(), jo["bookmark"].Value<long>(), jo["count"].Value<long>(),
                jo["fans"].Value<long>(), jo["pletter"].Value<long>(), jo["replyme"].Value<long>());
        }
        public MaxPageAndListResult<msgPost> listMsg(String type, long page)
        {
            switch (type)
            {
                case "reply":
                    {
                        JObject rjo = JSON.parse(_stbapi.sendTieba("/c/u/feed/replyme", "pn=" + page + "&uid=" + getUid(), bduss));
                        if (rjo["error_code"].Value<int>() != 0)
                        {
                            throw new TiebaField(new EntryResult(), rjo["error_code"].Value<int>(), rjo["error_msg"].Value<String>());
                        }
                        List<msgPost> tp = new List<msgPost>();
                        JEnumerable<JToken> jejt = rjo["reply_list"].Children();
                        foreach (JToken t in jejt)
                        {
                            tp.Add(new msgPost(t["content"].Value<String>(), t["fname"].Value<String>(), t["is_floor"].Value<int>()>0,
                                t["post_id"].Value<long>(), t["time"].Value<long>(), t["title"].Value<String>(), t["thread_id"].Value<long>(),
                                new userWithPic(t["replyer"]["id"].Value<long>(), t["replyer"]["name"].Value<String>(), t["replyer"]["portrait"].Value<String>())));
                        }
                        return new MaxPageAndListResult<msgPost>(tp, rjo["page"]["has_more"].Value<long>());
                    }
                case "atme":
                    {
                        JObject rjo = JSON.parse(_stbapi.sendTieba("/c/u/feed/atme", "pn=" + page + "&uid=" + getUid(), bduss));
                        if (rjo["error_code"].Value<int>() != 0)
                        {
                            throw new TiebaField(new EntryResult(), rjo["error_code"].Value<int>(), rjo["error_msg"].Value<String>());
                        }
                        List<msgPost> tp = new List<msgPost>();
                        JEnumerable<JToken> jejt = rjo["at_list"].Children();
                        foreach (JToken t in jejt)
                        {
                            tp.Add(new msgPost(t["content"].Value<String>(),t["fname"].Value<String>(),t["is_floor"].Value<int>()>0,
                                t["post_id"].Value<long>(),t["time"].Value<long>(),t["title"].Value<String>(),t["thread_id"].Value<long>(),
                                new userWithPic(t["replyer"]["id"].Value<long>(),t["replyer"]["name"].Value<String>(),t["replyer"]["portrait"].Value<String>())));
                        }
                        return new MaxPageAndListResult<msgPost>(tp, rjo["page"]["has_more"].Value<long>());
                    }
                default:
                    throw new ArgumentException("type参数不正确，此值只能为reply或atme");
            }
        }
        public void delPost(long pid, long tid, kwf kw)
        {
            String tbs = getTbs();
            JObject jors = JSON.parse(_stbapi.sendTieba("/c/c/bawu/delpost", "z=" + tid + "&word=" + _.encodeURIComponent(kw.kw) + "&pid=" + pid + "&tbs=" + tbs,
                bduss));
            if (jors["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jors["error_code"].Value<int>(), jors["error_msg"].Value<String>());
            }
        }
        public void delThread(long tid, kwf kw)
        {
            String tbs = getTbs();
            JObject jors = JSON.parse(_stbapi.sendTieba("/c/c/bawu/delthread", "z=" + tid + "&word=" + _.encodeURIComponent(kw.kw) + "&tbs=" + tbs,
                bduss));
            if (jors["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jors["error_code"].Value<int>(), jors["error_msg"].Value<String>());
            }
        }
        public void sign(kwf kw)
        {
            String tbs = getTbs();
            JObject jors = JSON.parse(_stbapi.sendTieba("/c/c/forum/sign", "kw=" + _.encodeURIComponent(kw.kw) + "&tbs=" + tbs, bduss));
            if (jors["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jors["error_code"].Value<int>(), jors["error_msg"].Value<String>());
            }
        }
        public void like(kwf kw)
        {
            JObject jors = JSON.parse(_stbapi.sendTieba("/c/c/forum/like", "kw=" + _.encodeURIComponent(kw.kw) + "&tbs=" + getTbs(),bduss));
            if (jors["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jors["error_code"].Value<int>(), jors["error_msg"].Value<String>());
            }
        }
        public void unlike(kwf kw)
        {
            JObject jors = JSON.parse(_stbapi.sendTieba("/c/c/forum/unlike", "kw=" + _.encodeURIComponent(kw.kw) + "&tbs=" + getTbs(), bduss));
            if (jors["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jors["error_code"].Value<int>(), jors["error_msg"].Value<String>());
            }
        }
        public void zam(String za, TiePost tp)
        {
            JObject jors = JSON.parse(_stbapi.sendTieba("/c/c/zan/like", "action=" + _.encodeURIComponent(za) + "&post_id=" + tp.id + "&thread_id=" + tp.tid, this.bduss));
            if (jors["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), jors["error_code"].Value<int>(), jors["error_msg"].Value<String>());
            }
        }
        /// <summary>
        /// "#(pic," + picid + "," + img.width + "," + img.height + ")");
        /// </summary>
        /// <param name="image">图片byte</param>
        /// <returns>picid</returns>
        public String updataImage(byte[] image)
        {
            WebClient c = new WebClient();
            String bound = "--------opentieba" + new Random().Next(Int32.MaxValue) + new Random().Next(Int32.MaxValue) + new Random().Next(Int32.MaxValue);
            c.Headers.Add("Content-Type", "multipart/form-data; boundary="+bound);
            List<byte> data = new List<byte>();
            String cstart = @"--" + bound + @"
Content-Disposition: form-data; name=""BDUSS""

" + bduss + @"
--" + bound + @"
Content-Disposition: form-data; name=""_client_type""

1
--" + bound + @"
Content-Disposition: form-data; name=""_phone_imei""

05-00-54-20-06-00-01-00-04-00-9C-35-01-00-26-28-02-00-24-14-09-00-32-53
--" + bound + @"
Content-Disposition: form-data; name=""_client_version""

wp1.0beta
--" + bound + @"
Content-Disposition: form-data; name=""_net_type""

3
--" + bound + @"
Content-Disposition: form-data; name=""pic"";filename=""file""

";
            byte[] cstardata = Encoding.UTF8.GetBytes(cstart);
            data.AddRange(cstardata);
            data.AddRange(image);
            String cend = @"
--" + bound;
            byte[] cenddata = Encoding.UTF8.GetBytes(cend);
            data.AddRange(cenddata);
            byte[] upd = data.ToArray();
            byte[] fhbyte = c.UploadData("http://c.tieba.baidu.com/c/c/img/upload", upd);
            JObject json = JSON.parse(Encoding.ASCII.GetString(fhbyte));
            if (json["error_code"].Value<int>() != 0)
            {
                throw new TiebaField(new EntryResult(), json["error_code"].Value<int>(), json["error_msg"].Value<String>());
            }
            return json["info"]["pic_id"].Value<String>();
        }
        public String updataImage(System.IO.FileStream f)
        {
            byte[] b = new byte[f.Length];
            f.Read(b, 0, (int)f.Length);
            f.Close();
            return updataImage(b);
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
