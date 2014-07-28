using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.ServiceModel;
using System.Xml.Linq;
using System.Threading;

namespace LiveCricketNDTVfeed
{
    public partial class Form1 : Form
    {
        public static clsProxy Proxy;
        public static string XMLDir, ProxiesFilePath, LogDir, ParamLogFolder, ParamInputFolder, ParamOutputFolder;
        public static string HtmlDir;//, OverWriteDir;
        private clsWebPage web = new clsWebPage();
        private clsWriteFile wf = new clsWriteFile();
        private DateTime UKTime, TimeRefresh;
        public static clsDataFixtures dFixtures;
        public static clsDataScorecard dScorecard;
        private static TimeZoneInfo GMTStandardTime = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        private Queue<int> CompQ = new Queue<int>();
        public static int MaxConnection, RequestTime, ThreadQ = 0, AutoRefreshInterval;
        private bool AutoRefresh;

        public Form1()
        {
            InitializeComponent();
            InitializeAgentWrapper();
            mainStart();
            
        }

        private void mainStart()
        {
            string url, text;
            lInning.Text = "";
            TeamScore.Text = "";
            cbIn.Items.Clear();
            GetConfig();
            Proxy = new clsProxy();
            AddProxys();
            CreateDir();
            Createlv();
            url = "http://cricket.hosted.stats.com/apis/fixtures/fetch_fixtures_json.aspx?cal=calendar_new_liupre";
            text = getHtml(url, Proxy, "fixtures");
            dFixtures = new clsDataFixtures();
            dScorecard = new clsDataScorecard();
            if (AgentSemiautomatic)
            {
                AgentWaitHandle.Reset();
                //
                dFixtures.getFixtures(text);
                //
                AgentWaitHandle.Set();
            }
            else
            {
                //
                dFixtures.getFixtures(text);
                //
            }
            dScorecard.lvComp = lvTournament;
            addlvTournament();
            dScorecard.addMatches(dFixtures);
            TimeRefresh = DateTime.Now.AddMinutes(AutoRefreshInterval);
            if (dFixtures.Fixtures.Count == 0)
                TeamScore.Text = "NO MATCH PLAY!";
            timerMain.Enabled = true;
        }

        private void addlvTournament()
        {
            try
            {
                int no = -1;
                foreach (var df in dFixtures.Fixtures)
                {
                    no++;
                    lvTournament.Items.Add(df.league);
                    lvTournament.Items[no].SubItems.Add(df.tourname);
                    lvTournament.Items[no].SubItems.Add(df.seriesname);
                    lvTournament.Items[no].SubItems.Add(df.matchdate_gmt);
                    lvTournament.Items[no].SubItems.Add(df.matchdate_local);
                    lvTournament.Items[no].SubItems.Add(df.teama);
                    lvTournament.Items[no].SubItems.Add(df.teamb);
                    lvTournament.Items[no].SubItems.Add(df.venue);
                    lvTournament.Items[no].SubItems.Add(df.matchstatus);
                    if (df.Status == clsDataFixtures.eStatus.Sleeping)
                        lvTournament.Items[no].SubItems.Add(df.Status.ToString() + " " + df.TSleep.ToString() + " s");
                    else lvTournament.Items[no].SubItems.Add(df.Status.ToString());
                }
            }
            catch { }
        }

        private string getHtml(string URL, clsProxy Proxy, string Comp)
        {
            string text = "";
            try
            {
                string ProxyName = "";
                text = web.GetbyWebRequest(URL, Proxy, ref ProxyName);
                if (!string.IsNullOrEmpty(text))
                {
                    int bytes = System.Text.Encoding.Default.GetBytes(text).Length;
                    string dir = HtmlDir + @"\" + Comp.Replace(".", "") + ".txt";
                    using (StreamWriter sw = new StreamWriter(dir, false))
                    {
                        sw.WriteLine(text);
                    }
                    wf.WriteGeneralLog(LogDir, "XMLSource", "", "Request Webpage ok," + dir + ",(bytes:" + bytes.ToString() + ")", ProxyName);
                }
                else
                    wf.WriteGeneralLog(LogDir, "XMLSource", "", "Request Webpage failed,", ProxyName);
                wf.WriteLogProxys(Proxy, LogDir);
            }
            catch { }
            return (text);
        }

        private void AddProxys()
        {
            try
            {
                Proxy.ProxyList.Clear();
                lbProxy.Items.Clear();

                if (File.Exists(ProxiesFilePath))
                {
                    using (StreamReader sr = new StreamReader(ProxiesFilePath))
                    {
                        string sLine;
                        string sFirstChar;
                        while ((sLine = sr.ReadLine()) != null)
                        {
                            sLine = sLine.Trim();
                            if (sLine != "")
                            {
                                sFirstChar = sLine.Substring(0, 1);
                                if (sFirstChar != "[" && sFirstChar != "/" && sFirstChar != ";")
                                {
                                    Proxy.ProxyList.Add(new clsProxy.ProxyWithStatus(sLine));
                                    lbProxy.Items.Add(sLine);
                                }
                            }
                        }
                    }
                }
                tbProxies.Text = Proxy.ProxyList.Count.ToString();
            }
            catch { }
        }

        private void GetConfig()
        {
            try
            {
                NameValueCollection appSettings = ConfigurationSettings.AppSettings;
                string value;
                XMLDir = appSettings.Get("XMLStoreDir");
                //OverWriteDir = appSettings.Get("XMLOverWriteDir");
                ProxiesFilePath = appSettings.Get("ProxiesFilePath");
                value = appSettings.Get("MaxConnection");
                try
                {
                    MaxConnection = Convert.ToInt16(value);
                }
                catch
                {
                    MaxConnection = 10;
                }
                value = appSettings.Get("Request Time");
                try
                {
                    RequestTime = Convert.ToInt16(value);
                }
                catch
                {
                    RequestTime = 10;
                }
                //value = appSettings.Get("RequestProxy");
                //try
                //{
                //    ReqProxy = Convert.ToInt16(value);
                //}
                //catch
                //{
                //    ReqProxy = 10;
                //}
                //value = appSettings.Get("IOStatsFlush");
                //try
                //{
                //    IOStatsFlush = Convert.ToInt16(value);
                //}
                //catch
                //{
                //    IOStatsFlush = 60;
                //}
                LogDir = appSettings.Get("LogDir");
                //value = appSettings.Get("Log");
                //Log = (value != null && value == "true");
                //value = appSettings.Get("SaveHtml");
                //cbSaveHtml.Checked = (value != null && value == "true");
                value = appSettings.Get("AutoRefresh");
                AutoRefresh = (value != null && value == "true");
                //cbAutoRefresh.Checked = (value != null && value == "true");
                value = appSettings.Get("AutoRefreshInterval");
                try
                {
                    AutoRefreshInterval = Convert.ToInt16(value);
                }
                catch
                {
                    AutoRefreshInterval = 60;
                }

                value = appSettings.Get("LogFolder");
                ParamLogFolder = (value == null ? "" : value.Trim());
                if (ParamLogFolder == "")
                    ParamLogFolder = @".\Log";
                ParamLogFolder = Path.GetFullPath(ParamLogFolder);

                value = appSettings.Get("InputFolder");
                ParamInputFolder = (value == null ? "" : value.Trim());
                if (ParamInputFolder == "")
                    ParamInputFolder = @".\Input";
                ParamInputFolder = Path.GetFullPath(ParamInputFolder);

                value = appSettings.Get("OutputFolder");
                ParamOutputFolder = (value == null ? "" : value.Trim());
                if (ParamOutputFolder == "")
                    ParamOutputFolder = @".\Output";
                ParamOutputFolder = Path.GetFullPath(ParamOutputFolder);
            }
            catch { }
        }

        private void CreateDir()
        {
            try
            {
                UKTime = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime).Date;
                XMLDir += @"\" + UKTime.ToString("yyyyMMdd");
                Directory.CreateDirectory(XMLDir);
                LogDir += @"\" + UKTime.ToString("yyyyMMdd");
                Directory.CreateDirectory(LogDir);
                HtmlDir = LogDir + @"\" + "html";
                Directory.CreateDirectory(HtmlDir);
                Directory.CreateDirectory(ParamLogFolder);
                Directory.CreateDirectory(ParamInputFolder);
                Directory.CreateDirectory(ParamOutputFolder);
            }
            catch { }
        }

        private void Createlv()
        {
            try
            {
                lvTournament.Items.Clear();
                lvTournament.Clear();
                lvTournament.Columns.Add("League", 50, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Tournament", 160, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Name", 160, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Start Time GMT", 110, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Start Time Local", 110, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Team1", 90, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Team2", 90, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Location", 160, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Match Status", 80, HorizontalAlignment.Left);
                lvTournament.Columns.Add("Thread Status", 80, HorizontalAlignment.Left);
                lvTournament.View = View.Details;
            }
            catch (Exception) { }

            try
            {
                lvBat.Items.Clear();
                lvBat.Clear();
                lvBat.Columns.Add("Batsman", 120, HorizontalAlignment.Left);
                lvBat.Columns.Add("Runs", 40, HorizontalAlignment.Left);
                lvBat.Columns.Add("Balls", 40, HorizontalAlignment.Left);
                lvBat.Columns.Add("Fours", 40, HorizontalAlignment.Left);
                lvBat.Columns.Add("Sixes", 40, HorizontalAlignment.Left);
                lvBat.Columns.Add("Dots", 40, HorizontalAlignment.Left);
                lvBat.Columns.Add("Strikerate", 40, HorizontalAlignment.Left);
                lvBat.Columns.Add("Howout", 50, HorizontalAlignment.Left);
                lvBat.View = View.Details;
            }
            catch { }
            
            try
            {
                lvBow.Items.Clear();
                lvBow.Clear();
                lvBow.Columns.Add("Bowler", 120, HorizontalAlignment.Left);
                lvBow.Columns.Add("Overs", 40, HorizontalAlignment.Left);
                lvBow.Columns.Add("Maidens", 40, HorizontalAlignment.Left);
                lvBow.Columns.Add("Runs", 40, HorizontalAlignment.Left);
                lvBow.Columns.Add("Wickets", 40, HorizontalAlignment.Left);
                lvBow.Columns.Add("Economyrate", 40, HorizontalAlignment.Left);
                lvBow.Columns.Add("Noballs", 40, HorizontalAlignment.Left);
                lvBow.Columns.Add("Dots", 40, HorizontalAlignment.Left);
                lvBow.View = View.Details;
            }
            catch { }
            try
            {
                lvFall.Items.Clear();
                lvFall.Clear();
                lvFall.Columns.Add("Batsman", 120, HorizontalAlignment.Left);
                lvFall.Columns.Add("Score", 40, HorizontalAlignment.Left);
                lvFall.Columns.Add("Overs", 40, HorizontalAlignment.Left);
                lvFall.View = View.Details;
            }
            catch { }
        }

        private void GeneralStatus()
        {
            try
            {
                int goodProxy = 0;
                int unused = 0;
                int failed = 0;
                foreach (clsProxy.ProxyWithStatus ps in Proxy.ProxyList)
                {
                    if (ps.UseStatus == 1)
                        goodProxy++;
                    else if (ps.UseStatus == 0)
                        unused++;
                    else if (ps.UseStatus == -1)
                        failed++;
                }
                wf.WriteGeneralStatus(LogDir, "TotalRequests", Proxy.ProTotalRequests.ToString());
                wf.WriteGeneralStatus(LogDir, "FailedRequests", Proxy.ProFailedRequests.ToString());
                wf.WriteGeneralStatus(LogDir, "WorkedProxies", goodProxy.ToString());
                wf.WriteGeneralStatus(LogDir, "UnusedProxies", unused.ToString());
                wf.WriteGeneralStatus(LogDir, "FailedProxies", failed.ToString());
            }
            catch { }
        }

        bool checkDay = true;
        string DayStart, DayRestart;
        private void timerMain_Tick(object sender, EventArgs e)
        {
            DateTime UKTime = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime);
            Application.DoEvents();
            try
            {
                for (int no = 0; no < dFixtures.Fixtures.Count; no++)
                {
                    if (dFixtures.Fixtures[no].Status != clsDataFixtures.eStatus.Abort)// && (dFixtures.Fixtures[no].live == "1"))
                    {
                        if ((dFixtures.Fixtures[no].ReqThread) || (UKTime > dFixtures.Fixtures[no].NextTimeReq))
                        {
                            if (!CompQ.Contains(no))
                                CompQ.Enqueue(no);
                            if (MaxConnection > dFixtures.ThreadRun)
                            {
                                dFixtures.ThreadRun++;
                                try
                                {
                                    int noCom;
                                    noCom = CompQ.Dequeue();
                                    if ((dFixtures.Fixtures[no].Status == clsDataFixtures.eStatus.ThreadQ)&&(ThreadQ > 0))
                                        ThreadQ--;

                                    dFixtures.Fixtures[no].Status = clsDataFixtures.eStatus.Running;
                                    dFixtures.Fixtures[no].ReqThread = false;
                                    dFixtures.Fixtures[no].NextTimeReq = DateTime.MaxValue;

                                    Thread CricketThread = new Thread(() => dScorecard.mainCompetitons(noCom));
                                    CricketThread.Start();

                                    wf.WriteLogProxys(Proxy, LogDir);

                                }
                                catch { }
                            }
                            else
                            {
                                if (dFixtures.Fixtures[no].Status != clsDataFixtures.eStatus.ThreadQ)
                                {
                                    ThreadQ++;
                                    dFixtures.Fixtures[no].Status = clsDataFixtures.eStatus.ThreadQ;
                                    int column = 9;
                                    UpdatelvTournament(no, column, "Queue");
                                }
                            }
                        }
                    }
                    
                }
            }
            catch { }
            // Date Change
            try
            {
                if (checkDay)
                {
                    DayStart = UKTime.ToString("dd");
                    checkDay = false;
                }
                DayRestart = UKTime.ToString("dd");
                if ((DayRestart != DayStart) || ((TimeRefresh.AddMinutes(AutoRefreshInterval) < UKTime)))
                {
                    timerMain.Enabled = false;
                    GeneralStatus();
                    stop_Click(sender, e);
                    TimeRefresh = UKTime;
                    DayStart = DayRestart;
                    restart_Click(sender, e);
                }
            }
            catch { }

            //Flush - Restart
            try
            {
                if (TimeRefresh < DateTime.Now)
                {
                    GeneralStatus();
                    TimeRefresh = DateTime.Now.AddMinutes(AutoRefreshInterval);
                    restart_Click(sender, e);
                }
            }
            catch { }
            tbXML.Text = XMLDir;
            tbLog.Text = LogDir;
            slbUKTime.Text = "UK Time : " + UKTime.ToString("dd/MMM/yyyy HH:mm:ss");
        }

        

        private int noMatches = -1;
        private void scorecard_Click(object sender, EventArgs e)
        {
            int noMatch=-1;
            lInning.Visible = false;
            try
            {
                cbIn.Items.Clear();
                cbIn.Text = "Select Innings";
                lvBat.Items.Clear();
                lvBow.Items.Clear();
                lvFall.Items.Clear();
            }
            catch { }
            try
            {
                for (int i = 0; i < lvTournament.SelectedItems.Count; i++)
                {
                    int inx = lvTournament.FocusedItem.Index;
                    noMatch = inx;
                    noMatches = inx;
                }
            }
            catch { }
            string mId=null;
            try
            {
                mId = dScorecard.Match[noMatch].MatcheId;
            }
            catch { }

            try
            {
                for (int i = 0; i < dScorecard.Match.Count; i++)
                {
                    cbIn.Items.Clear();
                    foreach (var s in dScorecard.Match[noMatch].Inning)
                    {
                        cbIn.Items.Add("Innings " + s.Number);
                    }
                }
                TeamScore.Text = dFixtures.Fixtures[noMatch].teamscores;
            }
            catch { }
        }

        public string getPlayer(string id)
        {
            foreach (var item in dScorecard.Match[noMatches].Player)
            {
                if (id == item.PlayersNameId)
                    return (item.PlayersName);
            }
            return ("Player");
        }

        private void addlvBatFallBow(int noIn)
        {
            int no = -1;
            string player;
            try
            {
                no = -1;
                foreach (var Bat in dScorecard.Match[noMatches].Inning[noIn].Batsman)
                {
                    no++;
                    player = getPlayer(Bat.Batsman);
                    lvBat.Items.Add(player);
                    lvBat.Items[no].SubItems.Add(Bat.Runs);
                    lvBat.Items[no].SubItems.Add(Bat.Balls);
                    lvBat.Items[no].SubItems.Add(Bat.Fours);
                    lvBat.Items[no].SubItems.Add(Bat.Sixes);
                    lvBat.Items[no].SubItems.Add(Bat.Dots);
                    lvBat.Items[no].SubItems.Add(Bat.Strikerate);
                    lvBat.Items[no].SubItems.Add(Bat.Howout);
                }
            }
            catch { }
            try
            {
                no = -1;
                foreach (var Fall in dScorecard.Match[noMatches].Inning[noIn].FallofWicket)
                {
                    no++;
                    player = getPlayer(Fall.Batsman);
                    lvFall.Items.Add(player);
                    lvFall.Items[no].SubItems.Add(Fall.Score);
                    lvFall.Items[no].SubItems.Add(Fall.Overs);
                    
                }
            }
            catch { }
            try
            {
                no = -1;
                foreach (var Bow in dScorecard.Match[noMatches].Inning[noIn].Bowler)
                {
                    no++;
                    player = getPlayer(Bow.Bowler);
                    lvBow.Items.Add(player);
                    lvBow.Items[no].SubItems.Add(Bow.Overs);
                    lvBow.Items[no].SubItems.Add(Bow.Maidens);
                    lvBow.Items[no].SubItems.Add(Bow.Runs);
                    lvBow.Items[no].SubItems.Add(Bow.Wickets);
                    lvBow.Items[no].SubItems.Add(Bow.Economyrate);
                    lvBow.Items[no].SubItems.Add(Bow.Noballs);
                    lvBow.Items[no].SubItems.Add(Bow.Dots);
                }
            }
            catch { }
        }

        private void lvTournament_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            scorecard_Click(sender, e);
        }

        private void cbIn_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ind = cbIn.SelectedIndex;
            try
            {
                lvBow.Items.Clear();
                lvBat.Items.Clear();
                lvFall.Items.Clear();
                addlvBatFallBow(ind);
            }
            catch { }
            try
            {
                lInning.Visible = true;
                string team = dScorecard.Match[noMatches].Inning[ind].Battingteam;
                foreach (var item in dFixtures.Fixtures)
                {
                    if (team == item.teama_Id)
                        team = item.teama;
                    if (team == item.teamb_Id)
                        team = item.teamb;
                }
                lInning.Text = "Team : " + team + "  Total : " + dScorecard.Match[noMatches].Inning[ind].Total+
                    "  Wickets : " + dScorecard.Match[noMatches].Inning[ind].Wickets+
                    "  Overs : " + dScorecard.Match[noMatches].Inning[ind].Overs+
                    "  Runrate : " + dScorecard.Match[noMatches].Inning[ind].Runrate;
            }
            catch { }
        }

        private void abort_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lvTournament.SelectedItems.Count; i++)
                {
                    int inx = lvTournament.FocusedItem.Index;
                    if (dFixtures.Fixtures[inx].Status != clsDataFixtures.eStatus.Abort)
                    {
                        dFixtures.Fixtures[inx].Status = clsDataFixtures.eStatus.Abort;
                        int column = 9;
                        UpdatelvTournament(inx, column, "Task Abort");
                    }
                }
            }
            catch (Exception) { }
        }

        delegate void UpdatelvTournamentInvoker(int row, int column, string message);
        private void MainThread_UpdatelvTournament(int row, int column, string message)
        {
            try { lvTournament.Items[row].SubItems[column].Text = message; }
            catch { }
        }
        public void UpdatelvTournament(int row, int column, string message)
        {
            try { lvTournament.Invoke(new UpdatelvTournamentInvoker(MainThread_UpdatelvTournament), new object[] { row, column, message }); }
            catch { }
        }
        
        private void exit_Click(object sender, EventArgs e)
        {
            try
            {
                for (int no = 0; no < dFixtures.Fixtures.Count; no++)
                {
                    wf.WriteGeneralLog(LogDir, "Cricket", dFixtures.Fixtures[no].tourname, "Task Abort", "");
                    dFixtures.Fixtures[no].Status = clsDataFixtures.eStatus.Abort;
                    int column = 9;
                    UpdatelvTournament(no, column, "Task Abort");
                }
            }
            catch { }
            wf.WriteLogProxys(Proxy, LogDir);
            timerMain.Enabled = false;
            Close();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            timerMain.Enabled = false;
            stop_Click(sender,e);
            wf.WriteLogProxys(Proxy, LogDir);
            mainStart();
        }

        private void stop_Click(object sender, EventArgs e)
        {
            try
            {
                for (int no = 0; no < dFixtures.Fixtures.Count; no++)
                {
                    wf.WriteGeneralLog(LogDir, "Cricket", dFixtures.Fixtures[no].tourname, "Task Abort", "");
                    dFixtures.Fixtures[no].Status = clsDataFixtures.eStatus.Abort;
                    int column = 9;
                    UpdatelvTournament(no, column, "Task Abort");
                }
            }
            catch { }
            timerMain.Enabled = false;
        }

        #region AgentWrapper functions
        public string[] GetMarketList()
        {
            List<string> result = new List<string>();
            //if (fCompes != null)
            //    for (int i = 0; i < fCompes.clbCompetitions.Items.Count; i++)
            //        result.Add((fCompes.clbCompetitions.GetItemChecked(i) ? "[x] " : "[ ] ") + fCompes.clbCompetitions.Items[i]);
            return result.ToArray();
        }

        public void SetMarketList(string[] keys)
        {
            //for (int i = 0; i < fCompes.clbCompetitions.Items.Count; i++)
            //    fCompes.clbCompetitions.SetItemChecked(i, keys.Contains(fCompes.clbCompetitions.Items[i]));
            //try
            //{
            //    string Dir = ConfigurationSettings.AppSettings.Get("CompetitionDir");
            //    Dir += @"\Competitions.txt";
            //    using (StreamWriter sw = new StreamWriter(Dir, false))
            //        foreach (string str in fCompes.clbCompetitions.CheckedItems)
            //            sw.WriteLine(str);
            //    mainStart(false);
            //}
            //catch { }
        }

        public string[] GetTaskList()
        {
            //return lvCompetitions.Items.Cast<ListViewItem>()
            //                           .Select(lvi => lvi.Text + "+++" + lvi.SubItems[1].Text + "+++" + lvi.SubItems[2].Text + "+++" + lvi.SubItems[3].Text)
            //                           .ToArray();
            string[] str = { "0", "1" };
            return (str);
        }

        public XElement[] GetMarketElement(string key)
        {
            List<XElement> result = new List<XElement>();
            //ListViewItem lvi = lvCompetitions.FindItemWithText(key, false, 0, false);
            //if (lvi != null)
            //{
            //    int noComp = lvi.Index;
            //    const int CurOverOdds = 1;
            //    for (int noMatch = 0; noMatch < _mData.Competitions[noComp].Matches.Count; noMatch++)
            //    {
            //        string fileXMLname = "";
            //        if (_mData.Competitions[noComp].Matches[noMatch].MatchType == MarathonbetData.eMatchType.outright)
            //        {
            //            fileXMLname = _mData.Competitions[noComp].Competition;
            //            fileXMLname = fileXMLname.Replace("/", "-");
            //            fileXMLname = fileXMLname.Replace(":", "_");
            //            fileXMLname = fileXMLname.Replace(".", "");
            //        }
            //        else
            //        {
            //            fileXMLname = _mData.Competitions[noComp].Matches[noMatch].MatchStart.ToString("yyyyMMdd_HHmm_") +
            //                _mData.Competitions[noComp].Matches[noMatch].MatchNo.ToString("0000");
            //            fileXMLname = fileXMLname.Replace("/", "-");
            //        }
            //        string filename = fileXMLname.Replace("?", "") + "_" + CurOverOdds.ToString("0000") + ".xml";
            //        string dir = _mData.XmlOver + @"\" + _mData.Competitions[noComp].Sport.ToLower() + @"\" + filename;
            //        dir = dir.Replace("/", "-");
            //        try
            //        {
            //            result.Add(XElement.Load(dir));
            //        }
            //        catch { }
            //    }
            //}
            return result.ToArray();
        }

        public void RequestUpdate(string key)
        {
            //ListViewItem lvi = lvCompetitions.FindItemWithText(key, false, 0, false);
            //if (lvi != null && _mData.Competitions[lvi.Index].Status != MarathonbetData.eStatus.Abort)
            //    _mData.Competitions[lvi.Index].ReqThread = true;
        }
        #endregion AgentWrapper functions

        //private void RequestTask_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        for (int i = 0; i < lvTournament.SelectedItems.Count; i++)
        //        {
        //            int inx = lvTournament.FocusedItem.Index;
        //            if (dFixtures.Fixtures[inx].Status != clsDataFixtures.eStatus.Abort)
        //            {
        //                dFixtures.Fixtures[inx].ReqThread = true;
        //            }
        //        }
        //    }
        //    catch (Exception) { }
        //    scorecard_Click(sender, e);
        //}
    }
}
