using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace LiveCricketNDTVfeed
{
    public class clsDataFixtures
    {
        public class clsFixtures
        {
            public string daynight { get; set; }
            public string gmt_offset { get; set; }
            public string group { get; set; }
            public string league { get; set; }
            public string live { get; set; }
            public string match_Id { get; set; }
            public string matchfile { get; set; }
            public string matchnumber { get; set; }
            public string matchstatus { get; set; }
            public string matchdate_gmt { get; set; }
            public string matchdate_local { get; set; }
            public string matchtime_gmt { get; set; }
            public string matchtime_local { get; set; }
            public string seriesname { get; set; }
            public string series_short { get; set; }
            public string teama { get; set; }
            public string teama_short { get; set; }
            public string teama_Id { get; set; }
            public string teamb { get; set; }
            public string teamb_short { get; set; }
            public string teamb_Id { get; set; }
            public string tourname { get; set; }
            public string venue { get; set; }
            public string current_score { get; set; }
            public string teamscores { get; set; }
            public eStatus Status { get; set; }
            public bool ReqThread=false;
            public DateTime NextTimeReq;
            public int TSleep;
        }
        public List<clsFixtures> Fixtures = new List<clsFixtures>();
        public enum eStatus { Loading, Running, Sleeping, ThreadQ, Abort }
        public int ThreadRun = 0;
        private static TimeZoneInfo GMTStandardTime = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        internal void getFixtures(string text)
        {
            const int min = 0;
            const int max = 10;
            Random RandomNo = new Random();

            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    JObject json = JObject.Parse(text);
                    JObject data = (JObject)(json["data"]);
                    JArray matches = (JArray)data["matches"];
                    foreach (var aMatches in matches)
                    {
                        //if (true)
                        //if ((string)(aMatches["live"]) != "0")
                        string matchSta = ((string)(aMatches["matchstatus"])).Trim();
                        if ((matchSta.ToLower() != "match ended")&&(!string.IsNullOrEmpty(matchSta)))
                        {
                            Fixtures.Add(new clsFixtures { });
                            int no;
                            no = Fixtures.Count - 1;
                            Fixtures[no].daynight = (string)(aMatches["daynight"]);
                            Fixtures[no].gmt_offset = (string)(aMatches["gmt_offset"]);
                            Fixtures[no].group = (string)(aMatches["group"]);
                            Fixtures[no].league = (string)(aMatches["league"]);
                            Fixtures[no].live = (string)(aMatches["live"]);
                            Fixtures[no].match_Id = (string)(aMatches["match_Id"]);
                            Fixtures[no].matchfile = (string)(aMatches["matchfile"]);
                            Fixtures[no].matchnumber = (string)(aMatches["matchnumber"]);
                            Fixtures[no].matchstatus = (string)(aMatches["matchstatus"]);

                            DateTime date;
                            string str = (string)(aMatches["matchdate_gmt"]) + " " + (string)(aMatches["matchtime_gmt"]);
                            date = DateTime.ParseExact(str, "M/d/yyyy HH:mm", null);
                            Fixtures[no].matchdate_gmt = date.ToString("dd/MM/yyyy HH:mm");
                            str = (string)(aMatches["matchdate_local"]) + " " + (string)(aMatches["matchtime_local"]);
                            date = DateTime.ParseExact(str, "M/d/yyyy HH:mm", null);
                            Fixtures[no].matchdate_local = date.ToString("dd/MM/yyyy HH:mm");
                            Fixtures[no].matchtime_gmt = (string)(aMatches["matchtime_gmt"]);
                            Fixtures[no].matchtime_local = (string)(aMatches["matchtime_local"]);
                            Fixtures[no].seriesname = (string)(aMatches["seriesname"]);
                            Fixtures[no].series_short = (string)(aMatches["series_short_display_name"]);
                            Fixtures[no].teama = (string)(aMatches["teama"]);
                            Fixtures[no].teama_short = (string)(aMatches["teama_short"]);
                            Fixtures[no].teama_Id = (string)(aMatches["teama_Id"]);
                            Fixtures[no].teamb = (string)(aMatches["teamb"]);
                            Fixtures[no].teamb_short = (string)(aMatches["teamb_short"]);
                            Fixtures[no].teamb_Id = (string)(aMatches["teamb_Id"]);
                            Fixtures[no].tourname = (string)(aMatches["tourname"]);
                            Fixtures[no].venue = (string)(aMatches["venue"]);
                            Fixtures[no].current_score = (string)(aMatches["current_score"]);
                            Fixtures[no].teamscores = (string)(aMatches["teamscores"]);

                            Fixtures[no].Status = eStatus.Sleeping;
                            int TSleep = RandomNo.Next(min, max);
                            Fixtures[no].TSleep = TSleep;
                            DateTime UKTime = TimeZoneInfo.ConvertTime(DateTime.Now, GMTStandardTime);
                            Fixtures[no].NextTimeReq = UKTime.AddSeconds(TSleep);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
