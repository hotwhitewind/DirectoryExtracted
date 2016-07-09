using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace DirectoryExtracted_v2._1
{
    public static class Utility
    {
        private const string StrGet = "GET";
        private const string StrPost = "POST";
        private const string StrLocation = "Location";
        private const string StrAcceptAll = "*/*";
        private const string StrSetCookie = "Set-Cookie";
        private const string StrContentTypeTextHtml = "text/html; charset=utf-8";
        private const string StrContentTypeUrlEncoded = "application/x-www-form-urlencoded";
        private const string StrContentTypeMultipart = "multipart/form-data";
        private const string StrUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1) ; .NET CLR 2.0.50727; .NET CLR 3.0.04506.30; .NET CLR 1.1.4322; .NET CLR 3.5.20404)";
        private const string StrUseridPassMiss = "UserId/Password is missing !";
        private const string StrUseridPassWrong = "UserId/Password is wrong !";
        private const string StrApplicationError = "Application error! Try later..";

        public static string UserAgent
        { get { return StrUserAgent; } }

        public static string Get
        { get { return StrGet; } }

        public static string Post
        { get { return StrPost; } }

        public static string Location
        { get { return StrLocation; } }

        public static string SetCookie
        { get { return StrSetCookie; } }

        public static string ContentTypeUrlEncoded
        { get { return StrContentTypeUrlEncoded; } }

        public static string ContentTypeMultipart
        { get { return StrContentTypeMultipart; } }

        public static string ContentTypeTextHtml
        { get { return StrContentTypeTextHtml; } }

        public static string AcceptAll
        { get { return StrAcceptAll; } }

        public static string UseridPassMiss
        { get { return StrUseridPassMiss; } }

        public static string UseridPassWrong
        { get { return StrUseridPassWrong; } }

        public static string ApplicationError
        { get { return StrApplicationError; } }

        public static string GetRegExParsedValue(string strPattern, string strSearch)
        {
            var reg = new Regex(strPattern, RegexOptions.Multiline);
            var mat = reg.Match(strSearch);
            if (mat.Success)
            {
                return mat.Groups["RetVal"].Value;
            }
            return string.Empty;
        }

        public static string GetRegExParsedValue2(string strPattern, string strSearch)
        {
            var reg = new Regex(strPattern, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            var mat = reg.Match(strSearch);
            return mat.Success ? mat.Groups["RetVal"].Value : string.Empty;
        }

        public static MatchCollection GetRegExMatchCollection(string strPattern, string strSearch)
        {
            var reg = new Regex(strPattern, RegexOptions.Multiline);
            var mc = reg.Matches(strSearch);
            return mc;
        }

        public static MatchCollection GetRegExMatchCollection2(string strPattern, string strSearch)
        {
            var reg = new Regex(strPattern, RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
            var mc = reg.Matches(strSearch);
            return mc;
        }

        public static ArrayList GetRegExMatchCollectionRetVal(string strPattern, string strSearch)
        {
            var reg = new Regex(strPattern, RegexOptions.Multiline);
            var mc = reg.Matches(strSearch);
            var al = new ArrayList();
            foreach (Match m in mc)
            {
                al.Add(m.Groups["RetVal"].Value);
            }
            return al;
        }

        public static string GetInputControlsNameAndValueInPage(string strPage)
        {
            const string strRegExPatten = "<\\s*input.*?name\\s*=\\s*\"(?<Name>.*?)\".*?value\\s*=\\s*\"(?<Value>.*?)\".*?>";
            var reg = new Regex(strRegExPatten, RegexOptions.Multiline);
            var mc = reg.Matches(strPage);
            var strTemp = mc.Cast<Match>().Aggregate(string.Empty, (current, m) => current + m.Groups["Name"].Value + "=" + m.Groups["Value"].Value + "&");
            var n = strTemp.Length;
            strTemp = strTemp.Remove(n - 1);
            return strTemp;
        }

        public static CookieCollection GetDomainCorrectedCC(CookieCollection cc, string strDesiredDoamin)
        {
            var ccTemp = new CookieCollection();
            foreach (Cookie c in cc)
            {
                c.Domain = strDesiredDoamin;
                ccTemp.Add(c);
            }
            return ccTemp;
        }

        public static string GetJavaScriptTime()
        {
            var st = new DateTime(1970, 1, 1);
            var t = (DateTime.Now - st);
            var retval = (Int64)(t.TotalMilliseconds + 0.5);
            retval = retval + 1000000;
            return retval.ToString(CultureInfo.InvariantCulture);
        }

        public static string GetJavascriptRandomValue()
        {
            var ranDouble = new Random();
            var strd1 = ranDouble.NextDouble().ToString(CultureInfo.InvariantCulture);
            var strd1Len = strd1.Length;
            var ranLong = new Random();
            var strd2 = ranLong.Next() + ranLong.Next().ToString(CultureInfo.InvariantCulture);
            const int intDefLength = 18;
            var strCutFromstrd2 = string.Empty;
            if (strd1Len < intDefLength)
            {
                strCutFromstrd2 = strd2.Substring(0, intDefLength - strd1Len);
            }
            var strRes = strd1 + strCutFromstrd2;
            return strRes;
        }


        public static CookieCollection GetAllCookiesFromHeader(string strHeader, string strHost)
        {
            var cc = new CookieCollection();
            if (strHeader == string.Empty) return cc;
            var al = ConvertCookieHeaderToArrayList(strHeader);
            cc = ConvertCookieArraysToCookieCollection(al, strHost);
            return cc;
        }

        private static ArrayList ConvertCookieHeaderToArrayList(string strCookHeader)
        {
            strCookHeader = strCookHeader.Replace("\r", "");
            strCookHeader = strCookHeader.Replace("\n", "");
            var strCookTemp = strCookHeader.Split(',');
            var al = new ArrayList();
            var i = 0;
            var n = strCookTemp.Length;
            while (i < n)
            {
                if (strCookTemp[i].IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    al.Add(strCookTemp[i] + "," + strCookTemp[i + 1]);
                    i = i + 1;
                }
                else
                {
                    al.Add(strCookTemp[i]);
                }
                i = i + 1;
            }
            return al;
        }

        private static CookieCollection ConvertCookieArraysToCookieCollection(ArrayList al, string strHost)
        {
            var cc = new CookieCollection();

            var alcount = al.Count;
            for (var i = 0; i < alcount; i++)
            {
                var strEachCook = al[i].ToString();
                var strEachCookParts = strEachCook.Split(';');
                var intEachCookPartsCount = strEachCookParts.Length;
                var cookTemp = new Cookie();

                for (var j = 0; j < intEachCookPartsCount; j++)
                {
                    if (j == 0)
                    {
                        string strCNameAndCValue = strEachCookParts[j];
                        if (strCNameAndCValue != string.Empty)
                        {
                            int firstEqual = strCNameAndCValue.IndexOf("=", StringComparison.Ordinal);
                            string firstName = strCNameAndCValue.Substring(0, firstEqual);
                            string allValue = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1));
                            cookTemp.Name = firstName;
                            cookTemp.Value = allValue;
                        }
                        continue;
                    }
                    string strPNameAndPValue;
                    string[] nameValuePairTemp;
                    if (strEachCookParts[j].IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        strPNameAndPValue = strEachCookParts[j];
                        if (strPNameAndPValue != string.Empty)
                        {
                            nameValuePairTemp = strPNameAndPValue.Split('=');
                            cookTemp.Path = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : "/";
                        }
                        continue;
                    }

                    if (strEachCookParts[j].IndexOf("domain", StringComparison.OrdinalIgnoreCase) < 0) continue;
                    strPNameAndPValue = strEachCookParts[j];
                    if (strPNameAndPValue != string.Empty)
                    {
                        nameValuePairTemp = strPNameAndPValue.Split('=');

                        cookTemp.Domain = nameValuePairTemp[1] != string.Empty ? nameValuePairTemp[1] : strHost;
                    }
                }

                if (cookTemp.Path == string.Empty)
                {
                    cookTemp.Path = "/";
                }
                if (cookTemp.Domain == string.Empty)
                {
                    cookTemp.Domain = strHost;
                }
                cc.Add(cookTemp);
            }
            return cc;
        }
    }
}
