using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Opentieba
{
    public class goodclassflyItem
    {
        public readonly string kw;
        public readonly string name;
        public readonly short id;
        
        public goodclassflyItem(string kw, string name, short id)
        {
            this.kw = kw;
            this.name = name;
            this.id = id;
        }
        public List<basethread> listThread()
        {
            return null;
        }
    }
    /// <summary>
    /// 表示一个吧。
    /// </summary>
    public class bar
    {
        /// <summary>
        /// 吧名称
        /// </summary>
        public readonly string kw;
        /// <summary>
        /// 贴吧fid
        /// </summary>
        public readonly string fid;
        /// <summary>
        /// 此吧大吧主的user数组
        /// </summary>
        public readonly List<user> Managers=new List<user>();
        protected readonly JObject barinfo;
        public readonly List<goodclassflyItem> gdclasses=new List<goodclassflyItem>();
        /// <summary>
        /// 根据吧名称构建一个“吧对象”
        /// </summary>
        /// <param name="kw">吧名</param>
        public bar(string kw)
        {
            this.kw = kw;
            string jon=_stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=1", "");
            barinfo = JSON.parse(jon);
            JEnumerable<JToken> fromgr = barinfo["forum"]["managers"].Children();
            foreach (JToken jt in fromgr)
            {
                Managers.Add(new user(jt["id"].Value<long>(),jt["name"].Value<string>()));
            }
            JEnumerable<JToken> frgc=barinfo["forum"]["good_classify"].Children();
            foreach (JToken jt in frgc)
            {
                gdclasses.Add(new goodclassflyItem(kw, jt["class_name"].Value<string>(),
                    jt["class_id"].Value<short>()));
            }
        }
        public List<basethread> listThreads(long page)
        {
            string jon = _stbapi.sendTieba("http://c.tieba.baidu.com/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn="+page, "");
            JEnumerable<JToken> threadlist = JSON.parse(jon)["thread_list"].Children();
            List<basethread> lt = new List<basethread>();
            foreach (JToken jt in threadlist)
            {
                lt.Add(new basethread(jt["tid"].Value<long>(),jt["title"].Value<string>(),
                    jt["reply_num"].Value<long>(),jt["last_time_int"].Value<long>(),(jt["is_top"].Value<short>()==1?true:false),
                    (jt["is_good"].Value<short>() == 1 ? true : false),new user(jt["author"]["name"].Value<string>()),kw));
            }
            return lt;
        }
    }
}
