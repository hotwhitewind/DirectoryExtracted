using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace DirectoryExtracted_v2._1
{
    public partial class MainForm : Form
    {
        private string _saveDirPath;
        private readonly List<string> _findList;
        private readonly TextWriter _writer;
        private string _pathforkvart;
        private readonly string _currentDate;
        private string _login;
        private string _password;
        private readonly Hashtable _objectHash;
        private readonly Webber _webber;
        private readonly Stopwatch _watch;
        private List<Task> _downloadList;
        private readonly TextWriter _logWriter;

        private bool IsKvartDownl
        {
            get { return checkBoxKvart.Checked; }
        }

        private bool IsPlanDownl
        {
            get { return checkBoxPlanning.Checked; }
        }

        private bool IsFasadDownl
        {
            get { return checkBoxFasad.Checked; }
        }

        private bool IsOtdelDownl
        {
            get { return checkBoxOtdelka.Checked; }
        }
        private bool IsAllDownl
        {
            get { return checkBoxAll.Checked; }
        }


        public MainForm()
        {
            InitializeComponent();
            int limit = 2;
            if (rbLow.Checked)
                limit = 2;
            if (rbMedium.Checked)
                limit = 8;
            if (rbMax.Checked)
                limit = 20;
            _webber = new Webber(limit);
            _watch = new Stopwatch();
            _objectHash = new Hashtable();
            _currentDate = DateTime.Now.ToShortDateString();
            _saveDirPath = "";
            //откроем xml файл и считаем его в массив
            _findList = new List<string>();
            var doc = new XmlDocument();
            _writer = new StreamWriter("result.txt");
            _logWriter = new StreamWriter("exlog.txt");
            if (!File.Exists("table.xml"))
            {
                var files = new FileStream("table.xml", FileMode.Create, FileAccess.Write);
                files.Close();
                MessageBox.Show(@"Файл table.xml создан");
            }

            try
            {
                doc.Load("table.xml");
                //файл существует
                var fileinfo = doc.SelectNodes("//fileinfo");
                if (fileinfo != null)
                {
                    foreach (var filepath in from XmlNode filename in fileinfo
                                             select filename.Attributes
                                                 into xmlAttributeCollection
                                                 where xmlAttributeCollection != null
                                                 select xmlAttributeCollection["filepath"].InnerText)
                    {
                        _findList.Add(filepath);
                    }
                }
                MessageBox.Show(@"Список поиска заполнен!");
            }
            catch (XmlException)
            {
                MessageBox.Show(@"Ваш XML файл пуст и будет заполнен заново!");
            }
            TextReader reader = null;
            try
            {
                reader = new StreamReader("pass.txt");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Не могу найти файл с именем пользователя и паролем!!!!");
                return;
            }
            var temppass = reader.ReadLine();
            if (temppass == null)
            {
                MessageBox.Show(@"Не могу найти строки пользователь:пароль!");
                return;
            }
            _login = temppass.Split(':')[0];
            _password = temppass.Split(':')[1];
            reader.Close();
            var document = _webber.WebAutorisation(_login, _password);
            if (document == null)
            {
                MessageBox.Show(@"Не могу подключиться к серверу!");
                return;
            }
            var list1 = document.DocumentNode.SelectNodes("//select[@id='landing_developer']").FirstOrDefault();
            var id = "";

            if (list1 == null) return;
            foreach (var li in list1.ChildNodes)
            {
                if (li.Name == "option")
                {
                    id = li.GetAttributeValue("value", "");
                }
                else if (li.Name == "#text" && !li.InnerText.Contains("\n") && !li.InnerText.Contains("неважно"))
                {
                    var name = li.InnerText;
                    name = name.Replace("&quot;", "\"");
                    name = Regex.Replace(name, "[<>\"*:/?\\|]", "");

                    _objectHash.Add(name, id);
                    listBoxDir.Items.Add(name);
                }
            }
            _webber.EvDownloadComplete += WebberOnEvDownloadComplete;
        }

        private void WebberOnEvDownloadComplete(string fileName)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                listBoxDownloadStat.Items.Add(fileName);
                listBoxDownloadStat.SelectedIndex = listBoxDownloadStat.Items.Count - 1;
                listBoxDownloadStat.SelectedIndex = -1;
            });
        }

        private void buttonSetSaveDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _saveDirPath = textBoxSaveDirPath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private async void buttonLoad_Click(object sender, EventArgs e)
        {
            if (!File.Exists("pass.txt"))
            {
                MessageBox.Show(@"Не могу найти файл pass.txt, содержащий строку 'login:password'");
                return;
            }
            TextReader reader = new StreamReader("pass.txt");
            var temppass = reader.ReadLine();
            if (temppass == null)
            {
                MessageBox.Show(@"Не могу найти строку пользователь:пароль");
                return;
            }
            _login = temppass.Split(':')[0];
            _password = temppass.Split(':')[1];

            
            var sub = new List<string>();

            for (var i = 0; i < listBoxDir.Items.Count; i++)
            {
                if (listBoxDir.SelectedIndices.Contains(i))
                {
                    sub.Add(listBoxDir.Items[i].ToString());
                }
            }

            reader.Close();

            if (_saveDirPath == "")
            {
                MessageBox.Show(@"Установите папку для сохранения файлов!");
                return;
            }
            if (sub.Count == 0)
            {
                MessageBox.Show(@"Выберите застройщика!");
                return;
            }
            _webber.SetSaveDir(_saveDirPath, _findList);
            textBoxSaveDirPath.Enabled = false;
            buttonLoad.Enabled = false;
            buttonSetSaveDir.Enabled = false;
            checkBoxKvart.Enabled = false;
            checkBoxPlanning.Enabled = false;
            checkBoxAll.Enabled = false;
            checkBoxOtdelka.Enabled = false;
            checkBoxFasad.Enabled = false;
            var doc = _webber.WebAutorisation(_login, _password);
            var listOfException = new List<string>();
            _webber.InitQueue();
            _downloadList = new List<Task>();

            if (doc != null)
            {
                _watch.Start();
                try
                {
                    var mainTask = Task.Run(() => LoadAll(sub));
                    await mainTask;
                }
                catch (AggregateException ex)
                {
                    listOfException.AddRange(ex.InnerExceptions.Select(er => er.Message));
                }
                catch (Exception ex)
                {
                    listOfException.Add(ex.Message);
                }
                if (!_webber.NewFileQueue.IsEmpty)
                {
                    var branchesXml = _webber.NewFileQueue.Select(i => new XElement("fileinfo",
                        new XAttribute("filepath", i)));
                    var bodyXml = new XElement("Files", branchesXml);
                    bodyXml.Save("table.xml");

                    foreach (var fileName in _webber.NewFileQueue)
                    {
                        await _writer.WriteLineAsync(fileName);
                        _findList.Add(fileName);
                    }
                }
            }
            else
            {
                MessageBox.Show(@"Не могу подключиться к серверу!!");
            }
            _watch.Stop();

            textBoxSaveDirPath.Enabled = true;
            buttonLoad.Enabled = true;
            buttonSetSaveDir.Enabled = true;
            checkBoxKvart.Enabled = true;
            checkBoxPlanning.Enabled = true;
            checkBoxAll.Enabled = true;
            checkBoxOtdelka.Enabled = true;
            checkBoxFasad.Enabled = true;

            var elapsedtime = "";
            if (_watch.ElapsedMilliseconds > 86400000 || _watch.ElapsedMilliseconds < 1000)
                elapsedtime = string.Format("{0} мс", _watch.ElapsedMilliseconds);
            else
            {
                if (_watch.ElapsedMilliseconds > 3600000)
                {
                    var hour = _watch.ElapsedMilliseconds/3600000;
                    var minute = (_watch.ElapsedMilliseconds - (hour*3600000))/60000;
                    var second = (_watch.ElapsedMilliseconds - (hour*3600000) - (minute*60000))/1000;
                    elapsedtime = string.Format("{0} ч. {1} мин. {2} сек.", hour, minute, second);
                }
                else
                {
                    if (_watch.ElapsedMilliseconds > 60000)
                    {
                        var minute = _watch.ElapsedMilliseconds/60000;
                        var second = (_watch.ElapsedMilliseconds - (minute*60000))/1000;
                        elapsedtime = string.Format("{0} мин. {1} сек.", minute, second);
                    }
                    else
                    {
                        if (_watch.ElapsedMilliseconds > 1000)
                        {
                            var second = _watch.ElapsedMilliseconds/1000;
                            elapsedtime = string.Format("{0} сек.", second);
                        }
                    }
                }
            }

            var resstr = string.Format(
                    "Загрузка завершена. Получено квартирограмм - {0}.\n Получено новых файлов - {1}\n Ошибок загрузки - {2}\n Время работы - {3}",
                    _webber.DownloadKvartCount, _webber.DownloadFileCount, _webber.ErrdownloadCount, elapsedtime);
            MessageBox.Show(resstr);
            await _logWriter.WriteLineAsync(resstr + "\n");

            if (listOfException.Count > 0)// || _webber.ErrList.Count > 0)
            {
                MessageBox.Show(@"Во время загрузки произошли ошибки!");

                //await _logWriter.WriteLineAsync(@"Во время загрузки произошли ошибки!");
                foreach (var err in listOfException)
                {
                    await _logWriter.WriteLineAsync(err);
                    await _logWriter.WriteLineAsync("\n---------------------------------------------\n");
                }
                foreach (var err in _webber.ErrList)
                {
                    await _logWriter.WriteLineAsync(err);
                    await _logWriter.WriteLineAsync("\n---------------------------------------------\n");
                }
            }

            if (_webber.ErrWebResponse.Count > 0)
            {
                MessageBox.Show(@"Во время загрузки произошли ошибки доступа к серверу!");
                //await _logWriter.WriteLineAsync(@"Во время загрузки произошли ошибки доступа к серверу!");
                foreach (var err in _webber.ErrWebResponse)
                {
                    await _logWriter.WriteLineAsync(err);
                    await _logWriter.WriteLineAsync("\n---------------------------------------------\n");
                }
            }

            //bool isHibernate = Application.SetSuspendState(PowerState.Hibernate, true, false);
        }

        private async Task LoadAll(IEnumerable<string> sub)
        {
            var fileAcQuery = new List<Task<Webber.SearchRet>>();
            foreach (var subDir1 in sub)
            {
                //в цикле проверим всех застройщиков
                //вот здесь можно применять ассинхронность... наверно
                //надо проверять на недопустимые знаки
                var subDir = Regex.Replace(subDir1, "[<>\"*:/?\\|]", "");
                var tempsaveDirPath = _saveDirPath + "\\" + subDir;
                if (!Directory.Exists(tempsaveDirPath))
                    Directory.CreateDirectory(tempsaveDirPath); //папка для объекта
                _pathforkvart = tempsaveDirPath + "\\" + "квартирограммы_" + _currentDate;
                if (!Directory.Exists(_pathforkvart))
                    Directory.CreateDirectory(_pathforkvart);
                //получим список объектов
                var dir = subDir;
                fileAcQuery.Add(_webber.WebSearch((string) _objectHash[dir], tempsaveDirPath, _pathforkvart));
            }
            while (fileAcQuery.Count > 0)
            {
                var first = await Task.WhenAny(fileAcQuery);
                fileAcQuery.Remove(first);
                await first;
                var srh = first.Result;
                //каждую задачу можно отправить на дальнейшую обработку, т.е. надо 
                if (srh == null)
                {
                    continue;
                }
                var temp1 = srh.SubDirectory.Keys.Where(c => c.ToLower().Contains("квартирогр"));
                var allProcessTask = new List<Task>();

                if (!temp1.Any())
                {
                    //есть несколько объектов
                    //получим список подпапок для каждого подобъекта
                    var objDir = srh.SubDirectory;
                    var getSubDirQuery = new List<Task<Webber.GetDirRet>>();

                    foreach (var objName in objDir.Keys)
                    {
                        //теперь надо завести асинхронность для получения папок объекта, прежде чем перейти к ProcessAll, она кстати тоже должна быть ассинхронная
                        var tempsaveDirPath2 = srh.DevSaveDirPath + "\\" + objName;
                        if (IsPlanDownl && !Directory.Exists(tempsaveDirPath2))
                            Directory.CreateDirectory(tempsaveDirPath2);
                        var name = objName;
                        getSubDirQuery.Add(_webber.GetSubDir(name, srh));
                    }

                    while (getSubDirQuery.Count > 0)
                    {
                        var firstSubDir = await Task.WhenAny(getSubDirQuery);
                        getSubDirQuery.Remove(firstSubDir);
                        await firstSubDir;
                        var subDirRet = firstSubDir.Result;
                        if (subDirRet == null)
                        {
                            continue;
                        }
                        //получим список папок объекта, можно начать разбор
                        subDirRet.DevSaveDirPath += "\\" + subDirRet.ObjName;
                        allProcessTask.Add(ProcessAll(subDirRet));
                    }
                }
                else
                {
                    var newSubDirRet = new Webber.GetDirRet
                    {
                        SubDirectory = srh.SubDirectory,
                        ObjName = "",
                        DevSaveDirPath = srh.DevSaveDirPath,
                        KvartSaveDirPath = srh.KvartSaveDirPath
                    };
                    allProcessTask.Add(ProcessOne(newSubDirRet));
                }
                await Task.WhenAll(allProcessTask);
            }
        }

        private async Task GetAllKvart(Dictionary<string, string> dic, string saveDir)
        {
            var arr = new ArrayList();
            foreach (var item in dic)
                arr.Add(item);
            var index = 0;

            while (index < arr.Count)
            {
                var key = ((KeyValuePair<string, string>) arr[index]).Key;
                var refd = ((KeyValuePair<string, string>) arr[index]).Value;
                index++;
                var res = await _webber.LoadSubRef(refd, key, "_");
                if (res == null) continue;

                if (res.SubDictionary.Count == 0)
                {
                    var key1 = key;
                    _downloadList.Add(_webber.DownloadFile(res.Document, saveDir + "_" + key1));
                }
                else
                {
                    var key1 = key;
                    _downloadList.Add(_webber.DownloadFile(res.Document, saveDir + "_" + key1));

                    //еще есть каталоги и возможно файлы
                    foreach (var dir in res.SubDictionary)
                    {
                        arr.Add(new KeyValuePair<string, string>(dir.Key, dir.Value));
                    }
                }
            }
        }

        private async Task GetAllPlan(Dictionary<string, string> dic, string saveDir)
        {
            var arr = new ArrayList();
            foreach (var item in dic)
                arr.Add(item);
            var index = 0;

            while (index < arr.Count)
            {
                var key = ((KeyValuePair<string, string>)arr[index]).Key;
                var refd = ((KeyValuePair<string, string>)arr[index]).Value;
                index++;
                var res = await _webber.LoadSubRef(refd, key, "\\");
                if (res == null) continue;

                if (res.SubDictionary.Count == 0)
                {
                    if (key.ToLower().Contains("архив")) continue;
                    if (!Directory.Exists(saveDir + "\\" + key))
                        Directory.CreateDirectory(saveDir + "\\" + key);
                    var key1 = key;
                    _downloadList.Add(_webber.DownloadFile(res.Document, saveDir + "\\" + key1));
                }
                else
                {
                    var key1 = key;
                    if (!Directory.Exists(saveDir + "\\" + key1))
                        Directory.CreateDirectory(saveDir + "\\" + key1);

                    _downloadList.Add(_webber.DownloadFile(res.Document, saveDir + "\\" + key1));

                    //еще есть каталоги и возможно файлы
                    foreach (var dir in res.SubDictionary)
                    {
                        arr.Add(new KeyValuePair<string, string>(dir.Key, dir.Value));
                    }
                } 
            }
        }

        private async Task ProcessOne(Webber.GetDirRet subDirRet)
        {
            var kvartkeys = subDirRet.SubDirectory.Keys.Where(c => c.ToLower().Contains("квартирогр")).ToList();
            if (kvartkeys.Any() && IsKvartDownl)
            {
                //нашли сразу, достаем все файлы, отдаем на обработку квартир
                var refd = subDirRet.SubDirectory[kvartkeys.First()];

                //здесь у нас должна быть ассинхронная загрузка 
                var res = await _webber.LoadSubRef(refd, "", "");
                if (res != null && res.SubDictionary.Count == 0)
                {
                    _downloadList.Add(res.SubDictionary.Count == 0
                    ? _webber.DownloadFile(res.Document, subDirRet.KvartSaveDirPath + "\\" + subDirRet.ObjName)
                    : GetAllKvart(res.SubDictionary, subDirRet.KvartSaveDirPath + "\\" + subDirRet.ObjName));
                }
            }

            if (IsAllDownl)
            {
                foreach (var subdir in subDirRet.SubDirectory.Keys)
                {
                    var subdirval = subdir;
                    if (!subdirval.ToLower().Contains("квартирогр"))
                    {
                        subdirval = Regex.Replace(subdirval, "[<>\"*:/?\\|]", "");
                        if (!Directory.Exists(subDirRet.MainSaveDirPath))
                            Directory.CreateDirectory(subDirRet.MainSaveDirPath);
                        var tempsavePlan = subDirRet.MainSaveDirPath + "\\" + subdirval;
                        if (!Directory.Exists(tempsavePlan))
                            Directory.CreateDirectory(tempsavePlan);
                        var refd = subDirRet.SubDirectory[subdir];
                        var res = await _webber.LoadSubRef(refd, "", "");
                        if (res != null)
                        {
                            _downloadList.Add(_webber.DownloadFile(res.Document, tempsavePlan));
                            if (res.SubDictionary.Count != 0)
                                _downloadList.Add(GetAllPlan(res.SubDictionary, tempsavePlan));
                            //_downloadList.Add(res.SubDictionary.Count == 0
                            //    ? _webber.DownloadFile(res.Document, tempsavePlan)
                            //    : GetAllPlan(res.SubDictionary, tempsavePlan));
                        }
                    }
                }
            }
            else
            {

                var kartkeys =
                    subDirRet.SubDirectory.Keys.Where(
                        c =>
                            c.ToLower().Contains("карта") || c.ToLower().Contains("ген") ||
                            c.ToLower().Contains("место")).ToList();
                if (kartkeys.Any() && IsPlanDownl)
                {
                    //нашли сразу, достаем все файлы
                    if (!Directory.Exists(subDirRet.DevSaveDirPath))
                        Directory.CreateDirectory(subDirRet.DevSaveDirPath);
                    var tempsavePlan = subDirRet.DevSaveDirPath + "\\Карта";
                    if (!Directory.Exists(tempsavePlan))
                        Directory.CreateDirectory(tempsavePlan);
                    var refd = subDirRet.SubDirectory[kartkeys.First()];
                    var res = await _webber.LoadSubRef(refd, "", "");
                    if (res != null)
                    {
                        _downloadList.Add(_webber.DownloadFile(res.Document, tempsavePlan));
                        if (res.SubDictionary.Count != 0)
                            _downloadList.Add(GetAllPlan(res.SubDictionary, tempsavePlan));

                        //_downloadList.Add(res.SubDictionary.Count == 0
                        //    ? _webber.DownloadFile(res.Document, tempsavePlan)
                        //    : GetAllPlan(res.SubDictionary, tempsavePlan));
                    }
                }
                var plankeys = subDirRet.SubDirectory.Keys.Where(c => c.ToLower().Contains("планиров")).ToList();
                if (plankeys.Any() && IsPlanDownl)
                {
                    //нашли сразу, достаем все файлы
                    if (!Directory.Exists(subDirRet.DevSaveDirPath))
                        Directory.CreateDirectory(subDirRet.DevSaveDirPath);
                    var tempsavePlan = subDirRet.DevSaveDirPath + "\\Планировка";
                    if (!Directory.Exists(tempsavePlan))
                        Directory.CreateDirectory(tempsavePlan);
                    var refd = subDirRet.SubDirectory[plankeys.First()];
                    var res = await _webber.LoadSubRef(refd, "", "");
                    if (res != null)
                    {
                        _downloadList.Add(_webber.DownloadFile(res.Document, tempsavePlan));
                        if (res.SubDictionary.Count != 0)
                            _downloadList.Add(GetAllPlan(res.SubDictionary, tempsavePlan));

                        //_downloadList.Add(res.SubDictionary.Count == 0
                        //    ? _webber.DownloadFile(res.Document, tempsavePlan)
                        //    : GetAllPlan(res.SubDictionary, tempsavePlan));
                    }
                }
                var fasadkeys = subDirRet.SubDirectory.Keys.Where(c => c.ToLower().Contains("фасад")).ToList();
                if (fasadkeys.Any() && IsFasadDownl)
                {
                    //нашли сразу, достаем все файлы
                    if (!Directory.Exists(subDirRet.DevSaveDirPath))
                        Directory.CreateDirectory(subDirRet.DevSaveDirPath);
                    var tempsavePlan = subDirRet.DevSaveDirPath + "\\Фасады_и_виды";
                    if (!Directory.Exists(tempsavePlan))
                        Directory.CreateDirectory(tempsavePlan);
                    var refd = subDirRet.SubDirectory[fasadkeys.First()];
                    var res = await _webber.LoadSubRef(refd, "", "");
                    if (res != null)
                    {
                        _downloadList.Add(_webber.DownloadFile(res.Document, tempsavePlan));
                        if (res.SubDictionary.Count != 0)
                            _downloadList.Add(GetAllPlan(res.SubDictionary, tempsavePlan));

                        //_downloadList.Add(res.SubDictionary.Count == 0
                        //    ? _webber.DownloadFile(res.Document, tempsavePlan)
                        //    : GetAllPlan(res.SubDictionary, tempsavePlan));
                    }
                }

                var otdelkakeys = subDirRet.SubDirectory.Keys.Where(c => c.ToLower().Contains("отделка")).ToList();
                if (otdelkakeys.Any() && IsOtdelDownl)
                {
                    //нашли сразу, достаем все файлы
                    if (!Directory.Exists(subDirRet.DevSaveDirPath))
                        Directory.CreateDirectory(subDirRet.DevSaveDirPath);
                    var tempsavePlan = subDirRet.DevSaveDirPath + "\\Отделка";
                    if (!Directory.Exists(tempsavePlan))
                        Directory.CreateDirectory(tempsavePlan);
                    var refd = subDirRet.SubDirectory[otdelkakeys.First()];
                    var res = await _webber.LoadSubRef(refd, "", "");
                    if (res != null)
                    {
                        _downloadList.Add(_webber.DownloadFile(res.Document, tempsavePlan));
                        if (res.SubDictionary.Count != 0)
                            _downloadList.Add(GetAllPlan(res.SubDictionary, tempsavePlan));

                        //_downloadList.Add(res.SubDictionary.Count == 0
                        //    ? _webber.DownloadFile(res.Document, tempsavePlan)
                        //    : GetAllPlan(res.SubDictionary, tempsavePlan));
                    }
                }
            }
            await Task.WhenAll(_downloadList);
        }

        private async Task ProcessAll(Webber.GetDirRet subDirRet)
        {
            if (subDirRet.SubDirectory.Keys.Any(c => c.ToLower().Contains("кварт")))
            {
                subDirRet.MainSaveDirPath = subDirRet.DevSaveDirPath;

                //значит сразу попали и можно обработать
                await ProcessOne(subDirRet);
            }
            //else
            //{
               //начинаем шерстить директории
                var subDirTask = new List<Task>();

                var arr = new ArrayList();
                foreach (var item in subDirRet.SubDirectory)
                    arr.Add(item);
                var index = 0;

                while (index < arr.Count)
                {
                    var key = ((KeyValuePair<string, string>)arr[index]).Key;
                    var refd = ((KeyValuePair<string, string>)arr[index]).Value;
                    index++;
                    var res = await _webber.LoadSubRef(refd, key, "\\");
                    if (res == null) continue;
                    if (res.SubDictionary.Keys.Any(c => c.ToLower().Contains("кварт")))
                    {
                        //если есть директория с кварт, то отдаем на обработку, иначе, добавляем в список директорий
                        var newDirRet = new Webber.GetDirRet
                        {
                            MainSaveDirPath = subDirRet.DevSaveDirPath,
                            DevSaveDirPath = subDirRet.DevSaveDirPath + "\\" + key,
                            KvartSaveDirPath = subDirRet.KvartSaveDirPath,
                            ObjName = subDirRet.ObjName + "_" + key,
                            SubDirectory = res.SubDictionary
                        };
                        subDirTask.Add(ProcessOne(newDirRet));
                    }
                    else
                    {
                        //еще есть каталоги
                        foreach (var dir in res.SubDictionary)
                        {
                            arr.Add(new KeyValuePair<string, string>(dir.Key, dir.Value));
                        }
                    }
                }
                await Task.WhenAll(subDirTask);
            //}
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _writer.Close();
            _logWriter.Close();
        }

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAll.Checked)
            {
                checkBoxFasad.Checked = true;
                checkBoxFasad.Enabled = false;
                checkBoxKvart.Checked = true;
                checkBoxKvart.Enabled = false;
                checkBoxOtdelka.Checked = true;
                checkBoxOtdelka.Enabled = false;
                checkBoxPlanning.Checked = true;
                checkBoxPlanning.Enabled = false;
            }
            else
            {
                checkBoxFasad.Checked = false;
                checkBoxFasad.Enabled = true;
                checkBoxKvart.Checked = false;
                checkBoxKvart.Enabled = true;
                checkBoxOtdelka.Checked = false;
                checkBoxOtdelka.Enabled = true;
                checkBoxPlanning.Checked = false;
                checkBoxPlanning.Enabled = true;
            }
        }

        private void rbLow_CheckedChanged(object sender, EventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 2;

        }

        private void rbMedium_CheckedChanged(object sender, EventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 8;
        }

        private void rbMax_CheckedChanged(object sender, EventArgs e)
        {
            ServicePointManager.DefaultConnectionLimit = 20;
        }
    }
}
