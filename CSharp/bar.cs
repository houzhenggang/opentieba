﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Opentieba
{
    /// <summary>
    /// 精品分类项
    /// </summary>
    public class goodclassflyItem
    {
        /// <summary>
        /// 吧
        /// </summary>
        public readonly String kw;
        /// <summary>
        /// 类型名称
        /// </summary>
        public readonly String name;
        /// <summary>
        /// 类型序数
        /// </summary>
        public readonly Int16 id;
        /// <summary>
        /// [内部程序调用]构造goodclassflyItem
        /// </summary>
        /// <param name="kw"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public goodclassflyItem(String kw, String name, Int16 id)
        {
            this.kw = kw;
            this.name = name;
            this.id = id;
        }
        /// <summary>
        /// 列出此精品分类下的帖子
        /// </summary>
        /// <returns>帖子组</returns>
        public List<basethread> listThread()
        {
            return null;
        }
    }
    /// <summary>
    /// 看吧失败后拖出
    /// </summary>
    public class SeeBarField : TiebaField
    {
        /// <summary>
        /// 吧名称
        /// </summary>
        public readonly String kw;
        /// <summary>
        /// [内部调用]
        /// </summary>
        /// <param name="kw"></param>
        /// <param name="errcode"></param>
        /// <param name="errmsg"></param>
        public SeeBarField(String kw, int errcode, String errmsg)
            : base(new EntryResult(), errcode, errmsg)
        {
            this.kw = kw;
        }
    }
    /// <summary>
    /// 一个列出的吧
    /// </summary>
    public class baseBar : kwf
    {
        public readonly int lid;
        /// <summary>
        /// [内部调用]
        /// </summary>
        /// <param name="kw"></param>
        /// <param name="fid"></param>
        public baseBar(String kw, long fid, int lid)
        {
            this.kw = kw;
            this.fid = fid;
            this.lid = lid;
        }
    }
    public class kwf
    {
        /// <summary>
        /// 吧名称
        /// </summary>
        public String kw;
        /// <summary>
        /// 贴吧fid
        /// </summary>
        public long fid;
        public kwf()
        {
            kw = "";
            fid = 0;
        }
        public kwf(long fid, String kw)
        {
            this.fid = fid;
            this.kw = kw;
        }
    }
    /// <summary>
    /// 表示一个吧。
    /// </summary>
    public class bar : kwf
    {
        /// <summary>
        /// 此吧大吧主的user数组
        /// </summary>
        public readonly List<user> Managers = new List<user>();
        public readonly JObject barinfo;
        public readonly bool isSigned = false;
        public readonly long signRank = 0;
        public readonly long maxPage;
        public readonly List<goodclassflyItem> gdclasses = new List<goodclassflyItem>();
        /// <summary>
        /// 根据吧名称构建一个“吧对象”
        /// </summary>
        /// <param name="kw">吧名</param>
        public bar(String kw)
        {
            this.kw = kw;
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=1", "");
            barinfo = JSON.parse(jon);
            if (barinfo["error_code"].Value<int>() != 0)
            {
                throw new SeeBarField(kw, barinfo["error_code"].Value<int>(), barinfo["error_msg"].Value<String>());
            }
            maxPage = barinfo["page"]["total_page"].Value<long>();
            JEnumerable<JToken> fromgr = barinfo["forum"]["managers"].Children();
            foreach (JToken jt in fromgr)
            {
                Managers.Add(new user(jt["id"].Value<long>(), jt["name"].Value<String>()));
            }
            JEnumerable<JToken> frgc = barinfo["forum"]["good_classify"].Children();
            foreach (JToken jt in frgc)
            {
                gdclasses.Add(new goodclassflyItem(kw, jt["class_name"].Value<String>(),
                    jt["class_id"].Value<Int16>()));
            }
            fid = barinfo["forum"]["id"].Value<long>();
        }
        public bar(BDUSS b, String kw)
        {
            this.kw = kw;
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=1", b.bduss);
            barinfo = JSON.parse(jon);
            if (barinfo["error_code"].Value<int>() != 0)
            {
                throw new SeeBarField(kw, barinfo["error_code"].Value<int>(), barinfo["error_msg"].Value<String>());
            }
            maxPage = barinfo["page"]["total_page"].Value<long>();
            JEnumerable<JToken> fromgr = barinfo["forum"]["managers"].Children();
            foreach (JToken jt in fromgr)
            {
                Managers.Add(new user(jt["id"].Value<long>(), jt["name"].Value<String>()));
            }
            JEnumerable<JToken> frgc = barinfo["forum"]["good_classify"].Children();
            foreach (JToken jt in frgc)
            {
                gdclasses.Add(new goodclassflyItem(kw, jt["class_name"].Value<String>(),
                    jt["class_id"].Value<Int16>()));
            }
            fid = barinfo["forum"]["id"].Value<long>();
            this.sb = b;
        }
        public BDUSS sb;
        /// <summary>
        /// 列出该吧所有主题
        /// </summary>
        /// <param name="page">页数</param>
        /// <returns>一个basethread组成的List组</returns>
        public MaxPageAndListResult<basethread> listThreads(long page)
        {
            String bduss = "";
            if (sb != null)
            {
                bduss = sb.bduss;
            }
            String jon = _stbapi.sendTieba("/c/f/frs/page", "kw=" + _.encodeURIComponent(kw) +
                "&is_good=0&pn=" + page, bduss);
            JObject jot = JSON.parse(jon);
            JEnumerable<JToken> threadlist = jot["thread_list"].Children();
            List<basethread> lt = new List<basethread>();
            foreach (JToken jt in threadlist)
            {
                List<userWithPic> likerList = new List<userWithPic>();
                foreach (JToken t in jt["zan"]["liker_list"].Children())
                {
                    likerList.Add(new userWithPic(t["id"].Value<long>(), t["name"].Value<String>(), t["portrait"].Value<String>()));
                }
                bool isb = false;
                if (jt["zan"]["is_liked"] != null)
                {
                    isb = jt["zan"]["is_liked"].Value<int>() > 0;
                }
                lt.Add(new basethread(jt["tid"].Value<long>(), jt["title"].Value<String>(),
                    jt["reply_num"].Value<long>(), jt["last_time_int"].Value<long>(), (jt["is_top"] == null ? false : jt["is_top"].Value<int>() == 1),
                    (jt["is_good"] == null ? false : jt["is_good"].Value<int>() == 1), new userWithPic(jt["author"]["id"].Value<int>(),
                        jt["author"]["name"].Value<String>(), jt["author"]["portrait"].Value<String>()), kw, jt["zan"]["num"].Value<long>(),
                        likerList, isb, jt["first_post_id"].Value<long>()));
            }
            return new MaxPageAndListResult<basethread>(lt, jot["page"]["total_page"].Value<long>());
        }
    }
}
