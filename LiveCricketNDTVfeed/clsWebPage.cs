using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;
using System.Windows.Forms;
using System.Web;
using System.Text;

public class clsWebPage
{
    const int TimeoutDefault = 30000;
    const int BUFFER_SIZE = 32768;
    public string GetRandomProxy(clsProxy Proxy, ref int proxyId)
    {
        if (Proxy.ProxyList.Count > 0)
        {
            Random RandomClass = new Random();

            int proxyCount = Proxy.ProxyList.Count;
            proxyId = RandomClass.Next(1, Proxy.ProxyList.Count - 1);
            string portnum;
            portnum = Proxy.ProxyList[proxyId].Proxy;
            return (portnum);
        }
        else
        {
            return "";
        }
    }
   

    public string GetbyWebRequest(string url, clsProxy Proxy, ref string ProxyName)
    {
        int noReq = 0, proxyinx = -1, timeout = -1;
        string content = "";
    ReCall:
        try
        {
            Application.DoEvents();
            ProxyName = GetRandomProxy(Proxy, ref proxyinx);
            Proxy.ProxyList[proxyinx].TotalRequests++;
            Proxy.ProxyList[proxyinx].UseStatus = 1;
            Proxy.ProTotalRequests++;

            WebRequest request = WebRequest.Create(url);
            request.Timeout = timeout;
            request.Proxy = new WebProxy(ProxyName);
            using (WebResponse response = request.GetResponse())
            {
                using (Stream s = response.GetResponseStream())
                {
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(s, encode))
                    {
                        content = sr.ReadToEnd();
                        if ((content.IndexOf("Welcome to the CoDeeN HTTP CDN Service") == -1) &&
                            (content.IndexOf("This is an evaluation version limited") == -1))
                            return (content);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ProxyName = ex.Message + "," + ProxyName;
            Proxy.ProFailedRequests++;
            Proxy.ProxyList[proxyinx].FailedRequests++;
            Proxy.ProxyList[proxyinx].UseStatus = -1;
            noReq++;
        }
        if (noReq > 50)
        {
            return (content);
        }
        else
            goto ReCall;
    }

}