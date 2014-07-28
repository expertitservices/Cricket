using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace LiveCricketNDTVfeed
{
    class clsWriteFile
    {
        private static TimeZoneInfo GMTStandardTime = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        public void writeLog(string LogDir, string competition, string OddsStatus, string message)
        {
            try
            {
                DateTime UKTime;
                UKTime = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime);
                LogDir = LogDir + @"\" + competition + ".txt";
                using (StreamWriter sw = new StreamWriter(LogDir, true))
                {
                    string str = "";
                    str = UKTime.ToString("dd/MMM/yyyy HH:mm:ss ");
                    sw.WriteLine(str + OddsStatus + message);
                }
            }
            catch (Exception) { }
        }

        public void WriteLogProxys(clsProxy Proxy, string LogDir)
        {
            try
            {
                if (Proxy.ProxyList.Count > 0)
                {
                    string proxyUsingFilePath = Path.Combine(LogDir, "_ProxyUsing.txt");
                    using (StreamWriter sw = File.CreateText(proxyUsingFilePath))
                    {
                        sw.WriteLine("Proxy,TotalRequests,SuccessRequests,FailedRequests");
                        foreach (clsProxy.ProxyWithStatus ps in Proxy.ProxyList)
                        {
                            sw.WriteLine(ps.Proxy + "," + ps.TotalRequests + "," + ps.SuccessRequests
                                            + "," + ps.FailedRequests);
                        }
                    }
                }
            }
            catch { }
        }

        public void WriteGeneralStatus(string LogDir, string name, string value)
        {
            string logFileName = LogDir + @"\_GeneralStatus.txt";

            if (!File.Exists(logFileName))
            {
                using (StreamWriter sw = File.AppendText(logFileName))
                {
                    sw.Write("TotalRequests=0\r\nFailedRequests=0\r\nWorkedProxies=0\r\nUnusedProxies=0\r\nFailedProxies=0\r\n\r\n");
                }
            }

            string[] logLines = File.ReadAllLines(logFileName);
            for (int lineNo = 0; lineNo < logLines.Length && lineNo < 6; lineNo++)
            {
                string tempLine = logLines[lineNo];
                string[] splitLine = tempLine.Split('=');
                if (splitLine.Length == 2)
                {
                    if (splitLine[0] == name)
                    {
                        if (string.IsNullOrEmpty(value))
                            logLines[lineNo] = name + "=" + (Convert.ToInt64(splitLine[1]) + 1);
                        else
                            logLines[lineNo] = name + "=" + value;
                        break;
                    }
                }
            }
            File.WriteAllLines(logFileName, logLines);
        }

        public void WriteGeneralLog(string LogDir, string sport, string comp, string msg, string proxy)
        {
            string logFileName = LogDir + @"\_GeneralLog.txt";

            string sLine = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss,") + sport + "," + comp
                                + "," + (msg == null ? msg : msg) + "," + proxy + "\r\n";
            File.AppendAllText(logFileName, sLine);
        }

        internal void writeProxy(clsProxy Proxy, string proxyPath)
        {
            try
            {
                if (Proxy.ProxyList.Count > 0)
                {
                    using (StreamWriter sw = File.CreateText(proxyPath))
                    {
                        sw.WriteLine("[Successful proxy in last session]");
                        for (int i = 0; i < Proxy.ProxyList.Count; i++)
                        {
                            if (Proxy.ProxyList[i].UseStatus == 1)
                            {
                                sw.WriteLine(Proxy.ProxyList[i].Proxy);
                            }
                        }
                        sw.WriteLine("[Unused proxy in last session]");
                        for (int i = 0; i < Proxy.ProxyList.Count; i++)
                        {
                            if (Proxy.ProxyList[i].UseStatus == 0)
                            {
                                sw.WriteLine(Proxy.ProxyList[i].Proxy);
                            }
                        }
                        sw.WriteLine("[Failed proxy in last session]");
                        for (int i = 0; i < Proxy.ProxyList.Count; i++)
                        {
                            if (Proxy.ProxyList[i].UseStatus == -1)
                            {
                                sw.WriteLine(Proxy.ProxyList[i].Proxy);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        public void IOStatsFileName(string FileType, string FileName, DateTime TimeStart, DateTime TimeEnd, int noWrite, int noRead)
        {
            int filebytes = 0, ReadBytes, WriteBytes;
            string sLine;
            try
            {
                try
                {
                    FileStream fs = File.OpenRead(FileName);
                    byte[] bytes = new byte[fs.Length];
                    filebytes = Convert.ToInt32(fs.Length);
                    fs.Close();
                }
                catch { }

                NameValueCollection appSettings = ConfigurationSettings.AppSettings;
                string IOStatsFileName = appSettings.Get("IOStatsFileName");

                FileInfo Fin = new FileInfo(IOStatsFileName);
                if (!Fin.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(IOStatsFileName))
                    {
                        sLine = "DateTimeStart, DateTimeEnd, Number of times the poller write a file to disk, Number of times the poller read a file from disk, The number of bytes the poller wrote to disk, The number of bytes the poller read from the disk";
                        sw.WriteLine(sLine);
                    }
                }
                sLine = TimeStart.ToString("dd/MM/yyyy HH:mm:ss,") + TimeEnd.ToString("dd/MM/yyyy HH:mm:ss,");
                ReadBytes = 0;
                WriteBytes = 0;
                if (FileType == "write")
                {
                    WriteBytes = filebytes;
                }
                else
                {
                    ReadBytes = filebytes;
                }
                sLine += noWrite.ToString() + "," + noRead.ToString() + "," + WriteBytes.ToString() + "," + ReadBytes.ToString() + "\r\n";
                File.AppendAllText(IOStatsFileName, sLine);
            }
            catch { }
        }

        internal void genXML(int noComp, clsDataScorecard.Matches Matches, clsDataFixtures.clsFixtures Fixtures)
        {
            string dMatch,filename = "";
            DateTime UKTime;
            UKTime = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime);
            try
            {
                dMatch = Fixtures.tourname + "," + Fixtures.teama + " vs " + Fixtures.teamb;
                filename = (Form1.XMLDir + @"\" + dMatch).Replace("/", "-");
                Directory.CreateDirectory(filename);
                filename += @"\" + UKTime.ToString("yyyyMMddHHmmssfff") + ".xml";

                XmlTextWriter xmlTw = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
                xmlTw.Formatting = Formatting.Indented;
                xmlTw.WriteStartDocument();
                try
                {
                    xmlTw.WriteStartElement("Match");
                    xmlTw.WriteAttributeString("Tournament", Fixtures.tourname);
                    xmlTw.WriteAttributeString("Description", Fixtures.teama + " vs " + Fixtures.teamb);
                    xmlTw.WriteAttributeString("Team1", Fixtures.teama);
                    xmlTw.WriteAttributeString("Team2", Fixtures.teamb);
                    xmlTw.WriteAttributeString("Location", Fixtures.venue);
                    xmlTw.WriteAttributeString("StartTimeGMT", Fixtures.matchdate_gmt);
                    xmlTw.WriteAttributeString("StartTimeLocal", Fixtures.matchdate_local);

                    xmlTw.WriteStartElement("Players");
                    foreach (var item in Matches.Player)
                    {
                        try
                        {
                            xmlTw.WriteStartElement("Player");
                            xmlTw.WriteAttributeString("Team", item.Team);
                            xmlTw.WriteAttributeString("Name", item.PlayersName);
                            //xmlTw.WriteAttributeString("IsRunner", "N/A");
                            xmlTw.WriteEndElement();
                        }
                        catch { }
                        
                    }
                    xmlTw.WriteEndElement();


                    foreach (var Inn in Matches.Inning)
                    {
                        try
                        {
                            string team = Inn.Battingteam;
                            if (team == Fixtures.teama_Id)
                                team = Fixtures.teama;
                            if (team == Fixtures.teamb_Id)
                                team = Fixtures.teamb;

                            string num = Inn.Number.ToLower();
                            switch (num)
                            {
                                case "first": num = "1"; break;
                                case "second": num = "2"; break;
                                case "third": num = "3"; break;
                                case "fourth": num = "4"; break;
                                case "fifth": num = "5"; break;
                                case "sixth": num = "6"; break;
                                case "seventh": num = "7"; break;
                                case "fieighth": num = "8"; break;
                                case "ninth": num = "9"; break;
                                case "tenth": num = "10"; break;
                            }

                            xmlTw.WriteStartElement("Inning");
                            xmlTw.WriteAttributeString("Number", num);
                            xmlTw.WriteAttributeString("Team", team);
                            xmlTw.WriteAttributeString("Total", Inn.Total);
                            xmlTw.WriteAttributeString("Wickets", Inn.Wickets);
                            xmlTw.WriteAttributeString("Overs", Inn.Overs);
                            xmlTw.WriteAttributeString("Runrate", Inn.Runrate);
                            
                            string name;
                            try
                            {
                                if (Inn.Batsman.Count > 0)
                                {
                                    xmlTw.WriteStartElement("Batsmans");
                                    foreach (var Bat in Inn.Batsman)
                                    {
                                        xmlTw.WriteStartElement("Batsman");
                                        name = getPlayer(Bat.Batsman, noComp);
                                        xmlTw.WriteAttributeString("Name", name);
                                        xmlTw.WriteAttributeString("Runs", Bat.Runs);
                                        xmlTw.WriteAttributeString("Balls", Bat.Balls);
                                        xmlTw.WriteAttributeString("Fours", Bat.Fours);
                                        xmlTw.WriteAttributeString("Sixes", Bat.Sixes);
                                        xmlTw.WriteAttributeString("Dots", Bat.Dots);
                                        xmlTw.WriteAttributeString("Strikerate", Bat.Strikerate);
                                        xmlTw.WriteAttributeString("Howout", Bat.Howout);
                                        xmlTw.WriteEndElement();
                                    }
                                    xmlTw.WriteEndElement();
                                }
                            }
                            catch { }
                            try
                            {
                                if (Inn.Bowler.Count > 0)
                                {
                                    xmlTw.WriteStartElement("Bowlers");
                                    foreach (var Bow in Inn.Bowler)
                                    {
                                        xmlTw.WriteStartElement("Bowler");
                                        name = getPlayer(Bow.Bowler, noComp);
                                        xmlTw.WriteAttributeString("Name", name);
                                        xmlTw.WriteAttributeString("Overs", Bow.Overs);
                                        xmlTw.WriteAttributeString("Maidens", Bow.Maidens);
                                        xmlTw.WriteAttributeString("Runs", Bow.Runs);
                                        xmlTw.WriteAttributeString("Wickets", Bow.Wickets);
                                        xmlTw.WriteAttributeString("Dots", Bow.Dots);
                                        xmlTw.WriteAttributeString("Economyrate", Bow.Economyrate);
                                        xmlTw.WriteAttributeString("Noballs", Bow.Noballs);
                                        xmlTw.WriteEndElement();
                                    }
                                    xmlTw.WriteEndElement();
                                }
                            }
                            catch { }

                            try
                            {
                                if (Inn.FallofWicket.Count > 0)
                                {
                                    xmlTw.WriteStartElement("FallofWickets");
                                    foreach (var Fall in Inn.FallofWicket)
                                    {
                                        xmlTw.WriteStartElement("Batsman");
                                        name = getPlayer(Fall.Batsman, noComp);
                                        xmlTw.WriteAttributeString("Name", name);
                                        xmlTw.WriteAttributeString("Score", Fall.Score);
                                        xmlTw.WriteAttributeString("Overs", Fall.Overs);
                                        xmlTw.WriteEndElement();
                                    }
                                    xmlTw.WriteEndElement();
                                }
                            }
                            catch { }

                            xmlTw.WriteEndElement();//xmlTw.WriteStartElement("Inning");
                        }
                        catch { }
                    }
                }
                catch { }
                finally
                {
                    xmlTw.WriteEndElement();//xmlTw.WriteStartElement("Match");
                    xmlTw.WriteEndDocument();//xmlTw.WriteStartDocument();
                    xmlTw.Flush();
                    xmlTw.Close();
                }
            }
            catch { }
            //return (dir);
        }
        private string getPlayer(string id,int no)
        {
            foreach (var item in Form1.dScorecard.Match[no].Player)
            {
                if (id == item.PlayersNameId)
                    return (item.PlayersName);
            }
            return ("Player");
        }
    }
}