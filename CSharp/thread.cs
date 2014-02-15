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
            author = new user(th["thread"]["author"]["id"].Value<long>(),
                th["thread"]["author"]["name"].Value<String>());
            maxPage = th["page"]["total_page"].Value<long>();
            kw = new bar(th["forum"]["name"].Value<String>());
        }
    }
}
