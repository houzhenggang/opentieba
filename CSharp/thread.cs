using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opentieba
{
    /// <summary>
    /// 看吧页中的帖子，含有基本信息
    /// </summary>
    public class basethread
    {
        public readonly long tid;
        public readonly String title;
        public readonly long reply_num;
        public readonly long last_time;
        public readonly bool is_top;
        public readonly bool is_good;
        public readonly user author;
        public readonly String kw;
        public basethread(long tid, String title, long reply_num, long last_time, bool is_top,
            bool is_good, user author, String kw)
        {
            this.tid = tid;
            this.title = title;
            this.reply_num = reply_num;
            this.last_time = last_time;
            this.is_top = is_top;
            this.is_good = is_good;
            this.author = author;
            this.kw = kw;
        }
    }
    /// <summary>
    /// 看帖出错时拖出
    /// </summary>
    public class ThreadNotFindField : TiebaField
    {
        public readonly long tid;
        public ThreadNotFindField(long tid, int errcode, String errmsg)
            : base(new EntryResult(),
                errcode, errmsg)
        {
            this.tid = tid;
        }
    }
    /// <summary>
    /// 帖子对象
    /// </summary>
    public class TieThread
    {
        public readonly long tid;
        public readonly String title;
        public readonly long reply_num;
        public readonly user author;
        public readonly long maxPage;
        public readonly bar kw;
        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="tid">帖子的ID</param>
        public TieThread(long tid)
        {
            JObject th = JSON.parse(_stbapi.sendTieba("/c/f/pb/page", "kz=" + tid + "&pn=1", ""));
            this.tid = tid;
            if (th["error_code"].Value<int>() != 0)
            {
                throw new ThreadNotFindField(tid, th["error_code"].Value<int>(), th["error_msg"].Value<String>());
            }
            maxPage = th["page"]["total_page"].Value<long>();
            title = th["thread"]["title"].Value<String>();
            reply_num = th["thread"]["reply_num"].Value<long>();
            author = new userInBar(th["thread"]["author"]["id"].Value<long>(),
                th["thread"]["author"]["name"].Value<String>(), th["thread"]["author"]["is_like"].Value<bool>(),
                th["thread"]["author"]["level_id"].Value<int>(), th["thread"]["author"]["portrait"].Value<String>());
            maxPage = th["page"]["total_page"].Value<long>();
            kw = new bar(th["forum"]["name"].Value<String>());
        }
        public List<basePost> listpost(long page)
        {
            JObject jo = JSON.parse(_stbapi.sendTieba("/c/f/pb/page", "kz=" + tid + "&pn=" + page, ""));
            JEnumerable<JToken> posts = jo["post_list"].Children();
            List<basePost> lbp = new List<basePost>();
            foreach (JToken pt in posts)
            {
                List<postContent> pc = new List<postContent>();
                JEnumerable<JToken> jecon = pt["content"].Children();
                foreach (JToken conjt in jecon)
                {
                    pc.Add(postContent.byJtoken(conjt));
                }
                lbp.Add(new basePost(new userInBar(pt["author"]["id"].Value<long>(), pt["author"]["name"].Value<String>(),
                    pt["author"]["is_like"].Value<bool>(), pt["author"]["level_id"].Value<int>(), pt["author"]["portrait"].Value<String>())
                    , pc.ToArray(), pt["floor"].Value<long>(), pt["id"].Value<long>(), pt["sub_post_number"].Value<long>(),
                    pt["time"].Value<long>(), pt["title"].Value<String>()));
            }
            return lbp;
        }
    }
    public class postContent
    {
        /// <summary>
        /// 引用JS里的一段代码：
        /// <code>
        ///                         function showcon(con,contarr){
        ///                            for (var i = 0; i &lt;; contarr.length; i++) {
        ///                                (function (a) {
        ///                                    if (a.type == 0) {
        ///                                        var pp = $("&lt;p&gt;正在加载.....&lt;/p&gt;");
        ///                                       _.js.encodeHtml(a.text, function (q) {
        ///                                            pp.html(q);
        ///                                        });
        ///                                        con.append(pp);
        ///                                    } else if (a.type == 1) {
        ///                                        con.append($("&lt;a style='color: #9c3f38;'&gt;" + enchtml(a.text) + "&lt;/a&gt;").click(function () {
        ///                                            //todo something open link
        ///                                            if (a.link.match(/^http:\/\/tieba.baidu.com\/p\/\d+$/) != null) {
        ///                                                _.contureDo("是否打开帖子" + enchtml(a.link.substr(25)) + "？", function () {
        ///                                                    seeThread(a.link.substr(25), kw, fid);
        ///                                                });
        ///                                            } else if (a.link.match(/^\//)) {
        ///                                                var win = window.open("http://tieba.baidu.com" + a.link, "贴吧链接");
        ///                                                win.setInterval(function () {
        ///                                                    win.document.title = "贴吧链接"
        ///                                                }, 500);
        ///                                            } else {
        ///                                                var win = window.open(a.link, "贴吧链接");
        ///                                                win.setInterval(function () {
        ///                                                    win.document.title = "贴吧链接"
        ///                                                }, 500);
        ///                                            }
        ///                                        })).append($("&lt;br&gt;"));
        ///                                   } else if (a.type == 2) {
        ///                                       con.append($(faceele(a.text)).click(function () {
        ///                                           console.info(a);
        ///                                       }));
        ///                                   } else if (a.type == 3) {
        ///                                       var sizearr = a.bsize.split(",", 2);
        ///                                       var img = $("&lt;img src='' width='" + sizearr[0] + "' height='" + sizearr[1] + "'&gt;");
        ///                                       con.append(img).append($("&lt;br&gt;"));
        ///                                       outdomainpic(img[0], a.src);
        ///                                       var picloading = $("&lt;div style='margin: 15px; background-color: rgba(0, 0, 0, 0.26);'&gt;此处的图像正在加载&lt;/div&gt;");
        ///                                       img.before(picloading);
        ///                                       img.load(function () {
        ///                                           picloading.animate({height: "0px", opacity: 0}, 300);
        ///                                       });
        ///                                       img.mouseenter(function () {
        ///                                           img.stop(false, false, false);
        ///                                           img.transit({opacity: 0.9,scale:1.6}, 250);
        ///                                       }).mouseleave(function () {
        ///                                               img.stop(false, false, false);
        ///                                               img.transit({opacity: 1,scale:1}, 250);
        ///                                           });
        ///                                       setTimeout(function () {
        ///                                           picloading.animate({height: "0px", opacity: 0}, 300);
        ///                                       }, 5000);
        ///                                   } else if (a.type == 4) {
        ///                                       con.append($("&lt;a style='color: #689269;'&gt;" + enchtml(a.text) + "&lt;/a&gt;").click(function () {
        ///                                           // todo...
        ///                                       })).append($("&lt;br&gt;"));
        ///                                       _.ReUpadtaEffect();
        ///                                   } else if (a.type == 5) {
        ///                                       con.append($("&lt;iframe sandbox='allow-same-origin allow-forms allow-scripts' style='width: 100%; height: 600px;' src='" + a.text + "'&gt;&lt;/iframe&gt;"));
        ///                                   } else {
        ///                                       con.append($("&lt;div style='color: #1e6c37;'&gt;[未知类型：" + a.type + "]“" + enchtml(JSON.stringify(a)) + "”&lt;/div&gt;")).append("&lt;br&gt;");
        ///                                   }
        ///                               })(contarr[i]);
        ///                           }
        ///                       }
        /// </code>
        /// </summary>
        public readonly int type;
        public readonly String text;
        public readonly String link = "";
        public readonly String c = "";
        public readonly int[] bsize = { 0, 0 };
        public readonly String src = "";
        public readonly long uid = 0;
        public postContent(int type, String text, String link, String c, int[] bsize, String src, long uid)
        {
            this.type = type;
            this.text = text;
            this.link = link;
            this.c = c;
            this.bsize = bsize;
            this.src = src;
            this.uid = uid;
        }
        public static postContent byJtoken(JToken jy)
        {
            int type;
            String text = "[?]";
            String link = "";
            String c = "";
            int[] bsize = { 0, 0 };
            String src = "";
            long uid = 0;
            type = jy["type"].Value<int>();
            if (jy["text"] != null)
            {
                text = jy["text"].Value<String>();
            }
            if (jy["link"] != null)
            {
                link = jy["link"].Value<String>();
            }
            if (jy["c"] != null)
            {
                c = jy["c"].Value<String>();
            }
            if (jy["bsize"] != null)
            {
                bsize = (int[])from String p in jy["bsize"].Value<String>().Split(",".ToCharArray())
                               select Int32.Parse(p);
            }
            return new postContent(type, text, link, c, bsize, src, uid);
        }
    }
    public class basePost
    {
        public readonly userInBar author;
        public readonly postContent[] content;
        public readonly long floor;
        public readonly long id;
        public readonly long sub_post_number;
        public readonly long time;
        public readonly String title;
        public basePost(userInBar author, postContent[] content, long floor, long id, long supn, long time, String title)
        {
            this.author = author;
            this.content = content;
            this.floor = floor;
            this.id = id;
            this.sub_post_number = supn;
            this.time = time;
            this.title = title;
        }
    }
}
