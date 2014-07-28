using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;
using System.Threading;

namespace LiveCricketNDTVfeed
{
    public class clsDataScorecard
    {
        public class Batsmans
        {
            public string Batsman { get; set; }
            public string Runs { get; set; }
            public string Balls { get; set; }
            public string Fours { get; set; }
            public string Sixes { get; set; }
            public string Dots { get; set; }
            public string Strikerate { get; set; }
            public string Howout { get; set; }
        }

        public class Bowlers
        {
            public string Bowler { get; set; }
            public string Overs { get; set; }
            public string Maidens { get; set; }
            public string Runs { get; set; }
            public string Wickets { get; set; }
            public string Dots { get; set; }
            public string Economyrate { get; set; }
            public string Noballs { get; set; }
        }

        public class FallofWickets
        {
            public string Batsman { get; set; }
            public string Score { get; set; }
            public string Overs { get; set; }
        }

        public class Players
        {
            public string Team { get; set; }
            public string Position { get; set; }
            public string PlayersName { get; set; }
            public string PlayersNameId { get; set; }
        }

        public class Innings
        {
            public string Number { get; set; }
            public string Battingteam { get; set; }
            public string Total { get; set; }
            public string Wickets { get; set; }
            public string Overs { get; set; }
            public string Runrate { get; set; }
            public List<FallofWickets> FallofWicket = new List<FallofWickets>();
            public List<Bowlers> Bowler = new List<Bowlers>();
            public List<Batsmans> Batsman = new List<Batsmans>();
        }
        
        public class Matches
        {
            public string MatcheId { get; set; }
            public bool DetailChange = false;
            public List<Innings> Inning = new List<Innings>();
            public List<Players> Player = new List<Players>();
        }
        public List<Matches> Match = new List<Matches>();

        clsDataFixtures dFixt;
        //public int noComp;
        clsProxy Proxy;
        private clsWebPage web = new clsWebPage();
        private clsWriteFile wf = new clsWriteFile();
        public string HtmlDir, LogDir;
        private static TimeZoneInfo GMTStandardTime = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        internal void mainCompetitons(int noComp)
        {
            try
            {
                int column;
                dFixt = Form1.dFixtures;
                Proxy = Form1.Proxy;
                HtmlDir = Form1.HtmlDir;
                LogDir = Form1.LogDir;
                dFixt.Fixtures[noComp].Status = clsDataFixtures.eStatus.Running;
                column = 9;
                UpdatelvCompetition(noComp, column, "Running");
                //Competitions[noComp].LastRequest = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime);

                Match[noComp].DetailChange = false;
                getScorecard(noComp);

                if (Match[noComp].DetailChange)
                {
                    wf.genXML(noComp, Match[noComp], dFixt.Fixtures[noComp]);
                }
                try
                {
                    DateTime UKTime = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime);
                    dFixt.Fixtures[noComp].NextTimeReq = UKTime.AddSeconds(Form1.RequestTime);
                    dFixt.Fixtures[noComp].TSleep = Form1.RequestTime;
                    dFixt.Fixtures[noComp].Status = clsDataFixtures.eStatus.Sleeping;
                    column = 9;
                    UpdatelvCompetition(noComp, column, "Sleeping " + dFixt.Fixtures[noComp].TSleep.ToString() + " s");
                }
                catch { }
            }
            catch { }
            finally
            {
                dFixt.ThreadRun--;
                Thread.CurrentThread.Abort();
            }
        }

        private void getScorecard(int noComp)
        {
            int matchNo = -1;
            bool cMatch = true;
            string text,url,matchID;
            matchID = dFixt.Fixtures[noComp].matchfile;
            url = "http://ndtvsports.cricket.sportzdeck.stats.com/v3/api/data.aspx?matchcode="+matchID+"&filetype=scorecard";
            text = getHtml(url, Proxy, dFixt.Fixtures[noComp].teama + "-" + dFixt.Fixtures[noComp].teamb);
            try
            {
                for (int m = 0; m < Match.Count; m++)
                {
                    if (Match[m].MatcheId == dFixt.Fixtures[noComp].match_Id)
                    {
                        matchNo = m;
                        cMatch = false;
                    }
                }
                if (cMatch)
                {
                    Match.Add(new Matches());
                    matchNo = Match.Count - 1;
                    Match[matchNo].MatcheId = dFixt.Fixtures[noComp].match_Id;
                }

                if (!string.IsNullOrEmpty(text))
                {
                    int noI=0;
                    JObject json = JObject.Parse(text);
                    JArray Innings = (JArray)(json["Innings"]);
                    foreach (var I in Innings)
                    {
                        #region Innings

                        string Number="", Battingteam, Total, Wickets, Overs, Runrate;
                        bool cNumber = true;
                        Number = (string)(I["Number"]);
                        Battingteam = (string)(I["Battingteam"]);
                        Total = (string)(I["Total"]);
                        Wickets = (string)(I["Wickets"]);
                        Overs = (string)(I["Overs"]);
                        Runrate = (string)(I["Runrate"]);
                        for (int i = 0; i < Match[matchNo].Inning.Count; i++)
                        {
                            if (Match[matchNo].Inning[i].Number == Number)
                            {
                                noI = i;
                                cNumber = false;
                            }
                        }
                        if (cNumber)
                        {
                            Match[matchNo].Inning.Add(new Innings { });
                            noI = Match[matchNo].Inning.Count - 1;
                            Match[matchNo].Inning[noI].Number = Number;
                            Match[matchNo].Inning[noI].Battingteam = Battingteam;
                            Match[matchNo].Inning[noI].Total = Total;
                            Match[matchNo].Inning[noI].Wickets = Wickets;
                            Match[matchNo].Inning[noI].Overs = Overs;
                            Match[matchNo].Inning[noI].Runrate = Runrate;
                            Match[matchNo].DetailChange = true;
                        }
                        else
                        {
                            if (Match[matchNo].Inning[noI].Battingteam != Battingteam)
                            {
                                Match[matchNo].Inning[noI].Battingteam = Battingteam;
                                Match[matchNo].DetailChange = true;
                            }
                            if (Match[matchNo].Inning[noI].Total != Total)
                            {
                                Match[matchNo].Inning[noI].Total = Total;
                                Match[matchNo].DetailChange = true;
                            }
                            if (Match[matchNo].Inning[noI].Wickets != Wickets)
                            {
                                Match[matchNo].Inning[noI].Wickets = Wickets;
                                Match[matchNo].DetailChange = true;
                            }
                            if (Match[matchNo].Inning[noI].Overs != Overs)
                            {
                                Match[matchNo].Inning[noI].Overs = Overs;
                                Match[matchNo].DetailChange = true;
                            }
                            if (Match[matchNo].Inning[noI].Runrate != Runrate)
                            {
                                Match[matchNo].Inning[noI].Runrate = Runrate;
                                Match[matchNo].DetailChange = true;
                            }
                        }
                        #endregion Innings

                        #region Batsmen
                        int no=0;
                        JArray Batsmen = (JArray)(I["Batsmen"]);
                        foreach (var BatsmenArray in Batsmen)
                        {
                            bool cBatsman = true;
                            string Batsman, Runs, Balls, Fours, Sixes;
                            string Dots, Strikerate, Howout;
                            Batsman = (string)(BatsmenArray["Batsman"]) == "" ? "N/A" : (string)(BatsmenArray["Batsman"]);
                            Runs = (string)(BatsmenArray["Runs"]) == "" ? "N/A" : (string)(BatsmenArray["Runs"]);
                            Balls = (string)(BatsmenArray["Balls"]) == "" ? "N/A" : (string)(BatsmenArray["Balls"]);
                            Fours = (string)(BatsmenArray["Fours"]) == "" ? "N/A" : (string)(BatsmenArray["Fours"]);
                            Sixes = (string)(BatsmenArray["Sixes"]) == "" ? "N/A" : (string)(BatsmenArray["Sixes"]);
                            Dots = (string)(BatsmenArray["Dots"]) == "" ? "N/A" : (string)(BatsmenArray["Dots"]);
                            Strikerate = (string)(BatsmenArray["Strikerate"]) == "" ? "N/A" : (string)(BatsmenArray["Strikerate"]);
                            Howout = (string)(BatsmenArray["Howout"]) == "" ? "N/A" : (string)(BatsmenArray["Howout"]);

                            for (int b = 0; b < Match[matchNo].Inning[noI].Batsman.Count; b++)
                            {
                                if (Match[matchNo].Inning[noI].Batsman[b].Batsman == Batsman)
                                {
                                    no = b;
                                    cBatsman = false;
                                }
                            }
                            if (cBatsman)
                            {
                                Match[matchNo].Inning[noI].Batsman.Add(new Batsmans { });
                                no = Match[matchNo].Inning[noI].Batsman.Count - 1;
                                Match[matchNo].Inning[noI].Batsman[no].Batsman = Batsman;
                                Match[matchNo].Inning[noI].Batsman[no].Runs = Runs;
                                Match[matchNo].Inning[noI].Batsman[no].Balls = Balls;
                                Match[matchNo].Inning[noI].Batsman[no].Fours = Fours;
                                Match[matchNo].Inning[noI].Batsman[no].Sixes = Sixes;
                                Match[matchNo].Inning[noI].Batsman[no].Dots = Dots;
                                Match[matchNo].Inning[noI].Batsman[no].Strikerate = Strikerate;
                                Match[matchNo].Inning[noI].Batsman[no].Howout = Howout;
                            }
                            else
                            {
                                if (Match[matchNo].Inning[noI].Batsman[no].Runs != Runs)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Runs = Runs;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Batsman[no].Balls != Balls)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Balls = Balls;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Batsman[no].Fours != Fours)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Fours = Fours;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Batsman[no].Sixes != Sixes)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Sixes = Sixes;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Batsman[no].Dots != Dots)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Dots = Dots;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Batsman[no].Strikerate != Strikerate)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Strikerate = Strikerate;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Batsman[no].Howout != Howout)
                                {
                                    Match[matchNo].Inning[noI].Batsman[no].Howout = Howout;
                                    Match[matchNo].DetailChange = true;
                                }
                            }
                        }
                        #endregion Batsmen

                        #region Bowlers
                        JArray Bowlers = (JArray)(I["Bowlers"]);
                        foreach (var BowlersArray in Bowlers)
                        {
                            string Bowler = "-", Overs1 = "-", Maidens = "-", Runs = "-", Wickets1 = "-";
                            string Economyrate = "-", Noballs = "-", Dots = "-";
                            bool cBowler = true;

                            Bowler = (string)(BowlersArray["Bowler"]) == "" ? "N/A" : (string)(BowlersArray["Bowler"]);
                            Overs1 = (string)(BowlersArray["Overs"]) == "" ? "N/A" : (string)(BowlersArray["Overs"]);
                            Maidens = (string)(BowlersArray["Maidens"]) == "" ? "N/A" : (string)(BowlersArray["Maidens"]);
                            Runs = (string)(BowlersArray["Runs"]) == "" ? "N/A" : (string)(BowlersArray["Runs"]);
                            Wickets1 = (string)(BowlersArray["Wickets"]) == "" ? "N/A" : (string)(BowlersArray["Wickets"]);
                            Economyrate = (string)(BowlersArray["Economyrate"]) == "" ? "N/A" : (string)(BowlersArray["Economyrate"]);
                            Noballs = (string)(BowlersArray["Noballs"]) == "" ? "N/A" : (string)(BowlersArray["Noballs"]);
                            Dots = (string)(BowlersArray["Dots"]) == "" ? "N/A" : (string)(BowlersArray["Dots"]);

                            for (int b = 0; b < Match[matchNo].Inning[noI].Bowler.Count; b++)
                            {
                                if (Match[matchNo].Inning[noI].Bowler[b].Bowler == Bowler)
                                {
                                    no = b;
                                    cBowler = false;
                                }
                            }
                            if (cBowler)
                            {
                                Match[matchNo].Inning[noI].Bowler.Add(new Bowlers { });
                                no = Match[matchNo].Inning[noI].Bowler.Count - 1;
                                Match[matchNo].Inning[noI].Bowler[no].Bowler = Bowler;
                                Match[matchNo].Inning[noI].Bowler[no].Overs = Overs1;
                                Match[matchNo].Inning[noI].Bowler[no].Maidens = Maidens;
                                Match[matchNo].Inning[noI].Bowler[no].Runs = Runs;
                                Match[matchNo].Inning[noI].Bowler[no].Wickets = Wickets1;
                                Match[matchNo].Inning[noI].Bowler[no].Economyrate = Economyrate;
                                Match[matchNo].Inning[noI].Bowler[no].Noballs = Noballs;
                                Match[matchNo].Inning[noI].Bowler[no].Dots = Dots;
                            }
                            else
                            {
                                if (Match[matchNo].Inning[noI].Bowler[no].Overs != Overs1)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Overs = Overs1;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Bowler[no].Maidens != Maidens)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Maidens = Maidens;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Bowler[no].Runs != Runs)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Runs = Runs;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Bowler[no].Wickets != Wickets1)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Wickets = Wickets1;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Bowler[no].Economyrate != Economyrate)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Economyrate = Economyrate;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Bowler[no].Noballs != Noballs)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Noballs = Noballs;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].Bowler[no].Dots != Dots)
                                {
                                    Match[matchNo].Inning[noI].Bowler[no].Dots = Dots;
                                    Match[matchNo].DetailChange = true;
                                }
                            }
                        }
                        #endregion Bowlers

                        #region FallofWickets
                        JArray FallofWickets = (JArray)(I["FallofWickets"]);
                        foreach (var FWArray in FallofWickets)
                        {
                            string Batsman = "-", Score = "-", Overs1 = "-";
                            bool cFall = true;
                            Batsman = (string)(FWArray["Batsman"]) == "" ? "N/A" : (string)(FWArray["Batsman"]);
                            Score = (string)(FWArray["Score"]) == "" ? "N/A" : (string)(FWArray["Score"]);
                            Overs1 = (string)(FWArray["Overs"]) == "" ? "N/A" : (string)(FWArray["Overs"]);
                            for (int f = 0; f < Match[matchNo].Inning[noI].FallofWicket.Count; f++)
                            {
                                if (Match[matchNo].Inning[noI].FallofWicket[f].Batsman == Batsman)
                                {
                                    no = f;
                                    cFall = false;
                                }
                            }
                            if (cFall)
                            {
                                Match[matchNo].Inning[noI].FallofWicket.Add(new FallofWickets { });
                                no = Match[matchNo].Inning[noI].FallofWicket.Count - 1;
                                Match[matchNo].Inning[noI].FallofWicket[no].Batsman = Batsman;
                                Match[matchNo].Inning[noI].FallofWicket[no].Score = Score;
                                Match[matchNo].Inning[noI].FallofWicket[no].Overs = Overs1;
                            }
                            else
                            {
                                if (Match[matchNo].Inning[noI].FallofWicket[no].Score != Score)
                                {
                                    Match[matchNo].Inning[noI].FallofWicket[no].Score = Score;
                                    Match[matchNo].DetailChange = true;
                                }
                                if (Match[matchNo].Inning[noI].FallofWicket[no].Overs != Overs1)
                                {
                                    Match[matchNo].Inning[noI].FallofWicket[no].Overs = Overs1;
                                    Match[matchNo].DetailChange = true;
                                }
                            }
                        }
                        #endregion FallofWickets
                    }

                    #region Teams
                    JObject Teams = (JObject)(json["Teams"]);
                    foreach (var T in Teams)
                    {
                        string Name_Short, Name_Full;
                        string TeamID = T.Key;
                        JObject Tobj = (JObject)(Teams[TeamID]);
                        Name_Full = (string)(Tobj["Name_Full"]);
                        Name_Short = (string)(Tobj["Name_Short"]);
                        JObject Players = (JObject)(Tobj["Players"]);
                        int no;
                        foreach (var P in Players)
                        {
                            string PlayerID = P.Key;
                            JObject Pobj = (JObject)(Players[PlayerID]);
                            bool cPlayer = true;
                            foreach (var item in Match[matchNo].Player)
                            {
                                if (item.PlayersNameId == PlayerID)
                                    cPlayer = false;
                            }
                            if (cPlayer)
                            {
                                Match[matchNo].Player.Add(new Players { });
                                no = Match[matchNo].Player.Count - 1;
                                Match[matchNo].Player[no].PlayersNameId = PlayerID;
                                Match[matchNo].Player[no].Position = (string)(Pobj["Position"]);
                                Match[matchNo].Player[no].PlayersName = (string)(Pobj["Name_Full"]);
                                Match[matchNo].Player[no].Team = Name_Full;
                            }
                        }
                    }
                    #endregion Teams
                }
            }
            catch 
            {
                try
                {
                    #region Teams
                    JObject json1 = JObject.Parse(text);
                    JObject Teams = (JObject)(json1["Teams"]);
                    foreach (var T in Teams)
                    {
                        string Name_Short, Name_Full;
                        string TeamID = T.Key;
                        JObject Tobj = (JObject)(Teams[TeamID]);
                        Name_Full = (string)(Tobj["Name_Full"]);
                        Name_Short = (string)(Tobj["Name_Short"]);
                        JObject Players = (JObject)(Tobj["Players"]);
                        int no;
                        foreach (var P in Players)
                        {
                            string PlayerID = P.Key;
                            JObject Pobj = (JObject)(Players[PlayerID]);
                            bool cPlayer = true;
                            foreach (var item in Match[matchNo].Player)
                            {
                                if (item.PlayersNameId == PlayerID)
                                    cPlayer = false;
                            }
                            if (cPlayer)
                            {
                                Match[matchNo].Player.Add(new Players { });
                                no = Match[matchNo].Player.Count - 1;
                                Match[matchNo].Player[no].PlayersNameId = PlayerID;
                                Match[matchNo].Player[no].Position = (string)(Pobj["Position"]);
                                Match[matchNo].Player[no].PlayersName = (string)(Pobj["Name_Full"]);
                                Match[matchNo].Player[no].Team = Name_Full;
                                Match[matchNo].DetailChange = true;
                            }
                        }
                    }
                    #endregion Teams
                }
                catch { }
            }
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
                    wf.WriteGeneralLog(LogDir, "JSONSource", "", "Request Webpage ok," + dir + ",(bytes:" + bytes.ToString() + ")", ProxyName);
                }
                else
                    wf.WriteGeneralLog(LogDir, "JSONSource", "", "Request Webpage failed,", ProxyName);
                wf.WriteLogProxys(Proxy, LogDir);
            }
            catch { }
            return (text);
        }

        public ListView lvComp;
        delegate void UpdatelvCompetitionInvoker(int row, int column, string message);
        private void MainThread_UpdatelvCompetition(int row, int column, string message)
        {
            try
            {
                lvComp.Items[row].SubItems[column].Text = message;
            }
            catch { }
        }

        public void UpdatelvCompetition(int row, int column, string message)
        {
            try
            {
                lvComp.Invoke(new UpdatelvCompetitionInvoker(MainThread_UpdatelvCompetition), new object[] { row, column, message });
            }
            catch { }
        }


        internal void addMatches(clsDataFixtures Fixt)
        {
            int matchNo = -1;
            bool cMatch;
            for (int fix = 0; fix < Fixt.Fixtures.Count; fix++)
            {
                cMatch = true;
                for (int m = 0; m < Match.Count; m++)
                {
                    if (Match[m].MatcheId == Fixt.Fixtures[fix].match_Id)
                    {
                        matchNo = m;
                        cMatch = false;
                    }
                }
                if (cMatch)
                {
                    Match.Add(new Matches());
                    matchNo = Match.Count - 1;
                    Match[matchNo].MatcheId = Fixt.Fixtures[fix].match_Id;
                }
            }
        }
    }
}
