using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opentieba
{
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
            bool is_good,user author,String kw)
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
}
