using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;

public class clsProxy
{
    public int ProTotalRequests = 0;
    public int ProFailedRequests = 0;

    public class ProxyWithStatus
    {
        public string Proxy;
        public int UseStatus = 0;
        public int FailedRequests = 0;
        public int SuccessRequests = 0;
        public int TotalRequests = 0;

        public ProxyWithStatus(string proxy)
        {
            Proxy = proxy;
        }

        public ProxyWithStatus(string proxy, int useStatus, int failedRequests, int successRequests, int totalRequests)
        {
            Proxy = proxy;
            UseStatus = useStatus;
            FailedRequests = failedRequests;
            SuccessRequests = successRequests;
            TotalRequests = totalRequests;
        }

        public override string ToString()
        {
            return Proxy;
        }
    }
    public List<ProxyWithStatus> ProxyList = new List<ProxyWithStatus>();


   
    
}

