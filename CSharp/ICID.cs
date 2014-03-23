using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Opentieba
{
    /// <summary>
    /// 表示一个ICID文件
    /// </summary>
    public class ICID : List<BDUSS>
    {
        /// <summary>
        /// 从文件流构造ICID
        /// </summary>
        /// <param name="fs"></param>
        public ICID(FileStream fs)
        {
            StreamReader sr = new StreamReader(fs);
            String p=sr.ReadToEnd();
            explainICID(p);
        }
        /// <summary>
        /// 从ICID格式字符串构造ICID
        /// </summary>
        /// <param name="icidStr">ICID字符串</param>
        public ICID(String icidStr)
        {
            explainICID(icidStr);
        }
        /// <summary>
        /// [内部调用]解析ICID
        /// </summary>
        /// <param name="icidStr"></param>
        protected void explainICID(String icidStr)
        {
            String[] lines = icidStr.Split("\n".ToCharArray());
            try
            {
                foreach (String line in lines)
                {
                    String linep = line;
                    if (line.EndsWith("\r"))
                    {
                        linep = line.Substring(0, line.Length - 1);
                    }
                    if (line.Length < 1)
                    {
                        continue;
                    }
                    if (line.StartsWith(";"))
                    {
                        continue;
                    }
                    //           111
                    // 0123456789012
                    // (a,)[BDUSS=b]
                    //           1111111
                    // 01234567890123456
                    // (abc,)[BDUSS=bbc]
                    if (line.StartsWith("("))
                    {
                        int usernameEnd = line.IndexOf(',', 1) - 1;
                        String username=line.Substring(1, usernameEnd);
                        int bdussStart = line.IndexOf("[BDUSS=", usernameEnd + 1) + 7;
                        String bduss = linep.Substring(bdussStart);
                        bduss = bduss.Substring(0, bduss.Length - 1);
                        this.Add(new BDUSS(username, bduss));
                    }
                    else
                    {
                        throw new FormatException();
                    }
                }
            }
            catch (Exception)
            {
                throw new FormatException("ICID 格式不正确。参见文档“The ICID standard”");
            }
        }
    }
}
