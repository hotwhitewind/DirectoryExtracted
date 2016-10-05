using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DirectoryExtracted_v2._1
{
    public class Webber
    {
        //класс для реализации запросов
        private CookieContainer _cook;
        private const string MainRef = "http://www.3355136.ru";//"http://dealers.spbrealty.ru/";
        private string _saveDirPath;
        private List<string> _findList;

        public ConcurrentQueue<string> NewFileQueue { get; private set; }
 
        public List<string> ErrWebResponse { get; private set; }

        public List<string> ErrList { get; private set; }

        public int DownloadFileCount { get; private set; }

        public int DownloadKvartCount { get; private set; }

        public int ErrdownloadCount { get; private set; }

        public delegate void DownloadCompete(string fileName);
        public event DownloadCompete EvDownloadComplete;

        protected virtual void OnEvDownloadComplete(string filename)
        {
            var handler = EvDownloadComplete;
            if (handler != null)
            {
                handler(filename);
            }
        }

        public class SearchRet
        {
            public Dictionary<string, string> SubDirectory;
            public string RefString;
            public string DevSaveDirPath;
            public string KvartSaveDirPath;
        }

        public class GetDirRet
        {
            public string MainSaveDirPath;
            public string ObjName;
            public string DevSaveDirPath;
            public string KvartSaveDirPath;
            public Dictionary<string, string> SubDirectory;
        }

        public class GetRefRet
        {
            public HtmlDocument Document;
            public Dictionary<string, string> SubDictionary;
        }

        public Webber(int limit)
        {
            ServicePointManager.DefaultConnectionLimit = limit;
            ErrWebResponse = new List<string>();    
            ErrList = new List<string>();
        }

        public void InitQueue()
        {
            NewFileQueue = new ConcurrentQueue<string>();
            ErrWebResponse.Clear();
            ErrList.Clear();
        }

        public HtmlDocument WebAutorisation(string user, string password)
        {
            var webReq = (HttpWebRequest) WebRequest.Create(MainRef);
            webReq.Method = Utility.Get;
            webReq.KeepAlive = true;
            webReq.AllowAutoRedirect = true;
            webReq.CookieContainer = new CookieContainer();
            webReq.UseDefaultCredentials = true;
            webReq.UserAgent = Utility.UserAgent;
            try
            {
                var webRes = (HttpWebResponse) webReq.GetResponse();
                _cook = webReq.CookieContainer;
                webRes.Close();
                webReq.Abort();

                var webReqLog = (HttpWebRequest) WebRequest.Create(MainRef + "/auth");
                webReqLog.Method = Utility.Post;
                webReqLog.SendChunked = false;
                webReqLog.KeepAlive = true;
                webReqLog.AllowAutoRedirect = true;
                webReqLog.CookieContainer = _cook;
                webReqLog.UserAgent = Utility.UserAgent;
                webReqLog.ContentType = Utility.ContentTypeUrlEncoded;
                webReqLog.ServicePoint.Expect100Continue = false;
                var passString = "LoginForm[username]=" + user + "&LoginForm[password]=" + password +
                                 "&LoginForm[rememberMe]=0&yt0=Войти";
                var byteArr = Encoding.UTF8.GetBytes(passString);
                webReqLog.ContentLength = byteArr.Length;
                webReqLog.GetRequestStream().Write(byteArr, 0, byteArr.Length);
                var webResLog = (HttpWebResponse) webReqLog.GetResponse();
                var document = new HtmlDocument();
                using (var stream = webResLog.GetResponseStream())
                {
                    if (stream == null) return null;
                    var sr = new StreamReader(stream);
                    document.Load(sr);
                    sr.Close();
                    return document;
                }
            }
            catch (WebException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<GetDirRet> GetSubDir(string objName, SearchRet shr)
        {
            var retDic = new GetDirRet();

            string tempRef;
            var document = new HtmlDocument();
            try
            {
                var webReq = (HttpWebRequest) WebRequest.Create(shr.SubDirectory[objName]);
                webReq.Method = Utility.Get;
                webReq.KeepAlive = true;
                webReq.AllowAutoRedirect = false;
                webReq.CookieContainer = _cook;
                webReq.UseDefaultCredentials = true;
                webReq.UserAgent = Utility.UserAgent;
                webReq.Referer = shr.RefString;
                webReq.Timeout = 30000;
                retDic.DevSaveDirPath = shr.DevSaveDirPath;
                retDic.KvartSaveDirPath = shr.KvartSaveDirPath;
                retDic.ObjName = objName;
                retDic.SubDirectory = new Dictionary<string, string>();

                using (var webRes = await webReq.GetResponseAsync())
                {
                    using (var stream = webRes.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            var sr = new StreamReader(stream);
                            document.Load(sr);
                            var lineNum = document.DocumentNode.SelectSingleNode("//div[@id='docsContainer']");
                            var javas = document.DocumentNode.SelectNodes("//script");
                            var tempJava = javas.Where(c => c.Line > lineNum.Line).ToList();
                            if (tempJava.Any())
                            {
                                var tempJava1 = tempJava.First().InnerText;
                                tempRef = Regex.Match(tempJava1, @"'[\d\D]+?'").Value;
                                tempRef = tempRef.Replace("'", "");
                            }
                            else
                            {
                                sr.Close();
                                return null;
                            }
                            sr.Close();
                        }
                        else
                        {
                            ErrWebResponse.Add("Ошибка при загрузке объекта " + objName + "\n" + "stream == null");
                            return null;
                        }
                    }
                }
            }
            catch (WebException e)
            {
                ErrWebResponse.Add("Ошибка при загрузке объекта " + objName + "\n" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                ErrList.Add("Ошибка при загрузке объекта " + objName + "\n" + ex.Message);
                return null;
            }
            //найдем ссылку на папки
            if (tempRef == "")
            {
                ErrWebResponse.Add("Ошибка при загрузке объекта " + objName + "\n" + "tempRef == ''");
                return null;
            }

            try
            {
                var webReq1 = (HttpWebRequest) WebRequest.Create(MainRef + tempRef);
                webReq1.Method = Utility.Get;
                webReq1.KeepAlive = true;
                webReq1.AllowAutoRedirect = false;
                webReq1.CookieContainer = _cook;
                webReq1.UseDefaultCredentials = true;
                webReq1.UserAgent = Utility.UserAgent;
                webReq1.Referer = shr.RefString;
                webReq1.Timeout = 30000;
                using (var webRes = await webReq1.GetResponseAsync())
                {
                    using (var stream = webRes.GetResponseStream())
                    {
                        if (stream == null)
                        {
                            ErrWebResponse.Add("Ошибка при загрузке объекта " + objName + "\n" + "stream == null");
                            return null;
                        }
                        var sr = new StreamReader(stream);
                        document.Load(sr);

                        var allLi = document.DocumentNode.SelectNodes("//li[@class='directory collapsed']");
                        if (allLi == null)
                            return null;
                        foreach (var li in allLi)
                        {
                            if (!li.HasChildNodes || li.ChildNodes.Count <= 1 || li.ChildNodes[1].Name != "div")
                                continue;
                            var div = li.ChildNodes[1];
                            if (!div.HasChildNodes || div.ChildNodes.Count <= 1 || div.ChildNodes[1].Name != "h3")
                                continue;
                            var dirName = div.ChildNodes[1].InnerText;
                            var refTemp = div.GetAttributeValue("data-path", "");
                            var dirRef = MainRef + refTemp;
                            if (refTemp.Length > 0)
                                retDic.SubDirectory.Add(dirName, dirRef);
                        }
                        return retDic;
                    }
                }
            }
            catch (WebException e)
            {
                ErrWebResponse.Add("Ошибка при загрузке объекта " + objName + "\n" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                ErrList.Add("Ошибка при загрузке объекта " + objName + "\n" + ex.Message);
                return null;
            }
        }

        public async Task DownloadFile(HtmlDocument document, string subDirPath)
        {
            var arefalls = document.DocumentNode.SelectNodes("//div[@class='header']");
            if (arefalls == null) return;
            var fileName = "";
            foreach (var arefall in arefalls)
            {
                try
                {
                    var tempref = arefall.ChildNodes[1].ChildNodes[0].GetAttributeValue("href", "");
                    if (tempref == "") continue;
                    var downloadref = tempref;

                    fileName = arefall.ChildNodes[1].InnerText;
                    var isfindStr = false;
                    var xmlsavedirpath = subDirPath.Substring(_saveDirPath.Length,
                        subDirPath.Length - _saveDirPath.Length);
                    string outfilePath;
                    if (!subDirPath.ToLower().Contains("квартирог"))
                    {
                        //занесем путь в список
                        var findStr = _findList.Find(c => c == xmlsavedirpath + "\\" + fileName);
                        if (findStr != null)
                        {
                            isfindStr = true;
                        }
                        else
                        {
                            NewFileQueue.Enqueue(xmlsavedirpath + "\\" + fileName);
                        }
                        outfilePath = subDirPath + "\\" + fileName;
                    }
                    else
                    {
                        //когда квартирограмма, то надо сформировать имя файла
                        outfilePath = subDirPath + "_" + fileName;
                        //NewFileQueue.Enqueue(xmlsavedirpath + "_" + fileName);
                    }

                    if (isfindStr) return;
                    var webReq = (HttpWebRequest)WebRequest.Create(downloadref);
                    webReq.Method = Utility.Get;
                    webReq.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    webReq.KeepAlive = true;
                    webReq.AllowAutoRedirect = true;
                    webReq.CookieContainer = _cook;
                    webReq.UseDefaultCredentials = true;
                    webReq.UserAgent = Utility.UserAgent;
                    webReq.Proxy = null;
                    webReq.Referer = MainRef;
                    webReq.Timeout = 30000;

                    var webRes = await webReq.GetResponseAsync();
                    var webHttpRes = (HttpWebResponse)webRes;

                    if (webHttpRes.StatusDescription.ToLower().Contains("tempor"))
                    {
                        //файл временно перенесен, надо новый запрос
                        var temps = webRes.Headers["Location"];
                        var iso = Encoding.GetEncoding("iso-8859-1");
                        var utf8 = Encoding.UTF8;

                        var isoBytes = utf8.GetBytes(temps);
                        var utf8Bytes = Encoding.Convert(utf8, iso, isoBytes);
                        var templocation = utf8.GetString(utf8Bytes);
                        templocation = MainRef + templocation;
                        webReq = (HttpWebRequest)WebRequest.Create(templocation);
                        webReq.Method = Utility.Get;
                        webReq.Headers.Add("X-Requested-With", "XMLHttpRequest");
                        webReq.KeepAlive = true;
                        webReq.AllowAutoRedirect = true;
                        webReq.CookieContainer = _cook;
                        webReq.UseDefaultCredentials = true;
                        webReq.UserAgent = Utility.UserAgent;
                        webReq.Referer = MainRef;
                        webReq.Proxy = null;
                        webReq.Timeout = 30000;
                        webReq.ReadWriteTimeout = 40000;
                        webRes = await webReq.GetResponseAsync();
                    }

                    var fileStream = new FileStream(outfilePath, FileMode.Create, FileAccess.ReadWrite);

                    var responseStream = webRes.GetResponseStream();

                    if (responseStream != null)
                    {
                        await responseStream.CopyToAsync(fileStream);

                        fileStream.Close();
                        responseStream.Close();
                    }
                    OnEvDownloadComplete(outfilePath);
                    if (!outfilePath.ToLower().Contains("квартирог"))
                    {
                        DownloadFileCount += 1;
                    }
                    else
                    {
                        DownloadKvartCount += 1;
                    }
                }
                catch (WebException e)
                {
                    //скорее всего файл не найден
                    ErrWebResponse.Add("Ошибка при загрузке объекта " + fileName + "\n" + e.Message);
                    ErrdownloadCount += 1;
                    throw;
                }
                catch (Exception ex)
                {
                    ErrList.Add("Ошибка при загрузке объекта " + fileName + "\n" + ex.Message);
                    ErrdownloadCount += 1;
                    throw;
                }
            }
        }

        public async Task<GetRefRet> LoadSubRef(string subRef, string key, string padd)
        {
            try
            {
                var subDirRet = new GetRefRet();
                var webReq = (HttpWebRequest) WebRequest.Create(subRef);
                webReq.Method = Utility.Get;
                webReq.Headers.Add("X-Requested-With", "XMLHttpRequest");
                webReq.KeepAlive = true;
                webReq.AllowAutoRedirect = false;
                webReq.CookieContainer = _cook;
                webReq.UseDefaultCredentials = true;
                webReq.UserAgent = Utility.UserAgent;
                webReq.Referer = MainRef;
                webReq.Timeout = 30000;

                var document = new HtmlDocument();

                using (var webRes = await webReq.GetResponseAsync())
                {
                    using (var stream = webRes.GetResponseStream())
                    {
                        if (stream == null)
                        {
                            ErrWebResponse.Add("Ошибка при загрузке объекта " + key + "\n" + "stream == null");
                            return null;
                        }
                        var sr = new StreamReader(stream);
                        document.Load(sr);
                        subDirRet.Document = document;
                        subDirRet.SubDictionary = new Dictionary<string, string>();
                        var allLi = document.DocumentNode.SelectNodes("//li[@class='directory collapsed']");

                        if (allLi == null)
                            return subDirRet;
                        foreach (var li in allLi)
                        {
                            if (!li.HasChildNodes || li.ChildNodes.Count <= 1 || li.ChildNodes[1].Name != "div")
                                continue;
                            var div = li.ChildNodes[1];
                            if (!div.HasChildNodes || div.ChildNodes.Count <= 1 || div.ChildNodes[1].Name != "h3")
                                continue;
                            var dirName = div.ChildNodes[1].InnerText;
                            var refTemp = div.GetAttributeValue("data-path", "");
                            var dirRef = MainRef + refTemp;
                            if (key != "")
                                dirName = key + padd + dirName;
                            if (refTemp.Length > 0)
                                subDirRet.SubDictionary.Add(dirName, dirRef);
                        }
                        return subDirRet;
                    }
                }
            }
            catch (WebException e)
            {
                ErrWebResponse.Add("Ошибка при загрузке объекта " + key + "\n" + e.Message);
                return null;
            }
            catch (Exception ex)
            {
                ErrList.Add("Ошибка при загрузке объекта " + key + "\n" + ex.Message);
                return null;
            }
        }

        public async Task<SearchRet> WebSearch(string devId, string savePath, string pathForKvart)
        {
            //сделаем поисковый запрос
            var retSearch = new SearchRet();

            var webReqLog = (HttpWebRequest) WebRequest.Create(MainRef + "/buildings");
            webReqLog.Method = Utility.Post;
            webReqLog.SendChunked = false;
            webReqLog.KeepAlive = true;
            webReqLog.Referer = MainRef + "\\buildings";
            webReqLog.AllowAutoRedirect = true;
            webReqLog.CookieContainer = _cook;
            webReqLog.UserAgent = Utility.UserAgent;
            webReqLog.Accept = "*/*";
            const string boundary = "---------------------------7dffc53902e0";
            webReqLog.ContentType = Utility.ContentTypeMultipart + "; boundary=" + boundary;
            webReqLog.ServicePoint.Expect100Continue = false;
            var findString = "--" + boundary + "\r\n";
            findString += "Content-Disposition: form-data; name=\"NewSearchFormFull[developer]\"\r\n\r\n";
            findString += devId + "\r\n";
            findString += "--" + boundary + "\r\n";
            findString += "--" + boundary + "--\r\n";
            var byteArr = Encoding.UTF8.GetBytes(findString);
            webReqLog.ContentLength = byteArr.Length;
            webReqLog.GetRequestStream().Write(byteArr, 0, byteArr.Length);
            webReqLog.Timeout = 30000;

            var document = new HtmlDocument();
            bool isPaged = true;
            int pages = 1;

            using (var webResLog = await webReqLog.GetResponseAsync())
            {
                retSearch.RefString = webResLog.ResponseUri.ToString();
            }
            retSearch.SubDirectory = new Dictionary<string, string>();
            var referer = MainRef + "buildings";
            do
            {
                try
                {
                    webReqLog = (HttpWebRequest) WebRequest.Create(retSearch.RefString + $"&page={pages}");
                    webReqLog.Method = Utility.Get;
                    webReqLog.Headers.Add("X-Requested-With", "XMLHttpRequest");
                    webReqLog.SendChunked = false;
                    webReqLog.KeepAlive = true;
                    webReqLog.Referer = referer;
                    webReqLog.AllowAutoRedirect = true;
                    webReqLog.CookieContainer = _cook;
                    webReqLog.UserAgent = Utility.UserAgent;
                    webReqLog.Accept = "*/*";
                    webReqLog.ContentType = Utility.ContentTypeUrlEncoded;
                    webReqLog.ServicePoint.Expect100Continue = false;
                    webReqLog.Timeout = 30000;

                    using (var webResLog1 = await webReqLog.GetResponseAsync())
                    {
                        using (var stream = webResLog1.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                var sr = new StreamReader(stream);
                                document.Load(sr);
                            }
                            else
                            {
                                ErrWebResponse.Add("Ошибка при загрузке объекта " + devId + "\n" + "stream == null");
                                return null;
                            }

                            //найдем объекты 
                            var table_lines = document.DocumentNode.SelectNodes("//tr[@class = 'table-line']");
                            //var li = document.DocumentNode.SelectNodes("//li[@class = 'title']");
                            foreach (var tab in table_lines)
                            {
                                var a = tab.SelectSingleNode(".//td[@data-href]");//li1.ChildNodes.Where(c => c.Name == "a").ToList();
                                var title_td = tab.SelectSingleNode(".//td");
                                if (a == null) continue;

                                var title = title_td.InnerText;

                                if (title_td.HasChildNodes)
                                {
                                    title = title_td.FirstChild.InnerText;
                                }

                                title = Regex.Replace(title, "[<>\"*:/?\\|\n\t\r]", "");
                                var href = a.GetAttributeValue("data-href", "");
                                href = href.Substring(0, href.IndexOf("layout", StringComparison.InvariantCulture));
                                href += "materials";
                                retSearch.SubDirectory.Add(title, MainRef + href);
                                retSearch.DevSaveDirPath = savePath;
                                retSearch.KvartSaveDirPath = pathForKvart;
                            }
                            //теперь проверим на страницы
                            isPaged = false;
                            var lipage = document.DocumentNode.SelectNodes("//li[@class = 'next']");
                            if (lipage != null)
                            {
                                isPaged = true;
                                pages += 1;
                                referer = retSearch.RefString;
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    ErrWebResponse.Add("Ошибка при загрузке объекта " + devId + "\n" + e.Message);
                    return null;
                }
                catch (Exception ex)
                {
                    ErrList.Add("Ошибка при загрузке объекта " + devId + "\n" + ex.Message);
                    return null;
                }
            } while (isPaged);
            return retSearch;
        }

        public void SetSaveDir(string saveDirPath, List<string> findList)
        {
            _saveDirPath = saveDirPath;
            _findList = findList;
            ErrdownloadCount = 0;
            DownloadFileCount = 0;
            DownloadKvartCount = 0;
        }
    }
}
