using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections.Specialized;
using System.Diagnostics;
using System.ServiceModel;
using System.Xml.Linq;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace LiveCricketNDTVfeed
{
    [ServiceContract]
    public interface IPS01
    {
        [OperationContract]
        string[] GetMarketList();

        [OperationContract(IsOneWay = true)]
        void SetMarketList(string[] keys);

        [OperationContract]
        string[] GetTaskList();

        [OperationContract]
        XElement[] GetMarketElement(string key);

        [OperationContract(IsOneWay = true)]
        void RequestUpdate(string key);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class Form1 : IPS01
    {
        private static string[] AgentParams = Environment.GetCommandLineArgs();
        private static bool AgentSemiautomatic = false;
        private static string AgentLogFolder;
        private static int AgentHeartbeat;
        private static string AgentName;
        private static int AgentTaskCount = 0;
        private static int AgentQueuedDownload = 0;
        private static long AgentLastDownloadSuccess = 0;
        private static long AgentLastPriceSuccess = 0;
        private static long AgentLastPriceChange = 0;
        private static EventWaitHandle AgentWaitHandle = null;
        private static ServiceHost AgentHost = null;
        private static System.Windows.Forms.Timer AgentTimer;
        private static UdpClient AgentUdpClient = null;
        private static byte[] AgentBytes;
        private static IPEndPoint AgentEndPoint = new IPEndPoint(IPAddress.Loopback, 9010);

        private void InitializeAgentWrapper()
        {
            try
            {
                // Get command-line arguments.
                if (AgentParams.Length > 1 && AgentParams[1].Equals("semi", StringComparison.OrdinalIgnoreCase))
                {
                    // Semiautomatic mode.
                    AgentSemiautomatic = true;

                    // Read configuration file.
                    AgentLogFolder = System.Configuration.ConfigurationSettings.AppSettings["LogFolder"] ?? @".\Log";
                    if (!Int32.TryParse(System.Configuration.ConfigurationSettings.AppSettings["Heartbeat"], out AgentHeartbeat))
                        AgentHeartbeat = 2000;

                    // Create application folders.
                    if (!Directory.Exists(AgentLogFolder))
                        Directory.CreateDirectory(AgentLogFolder);

                    // Create event wait handle.
                    AgentName = Process.GetCurrentProcess().ProcessName;
                    AgentWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, @"Global\" + AgentName);
                    AgentWaitHandle.Set();

                    // Open service host.
                    AgentHost = new ServiceHost(this, new Uri("net.pipe://localhost/" + AgentName));
                    NetNamedPipeBinding netNamedPipeBinding = new NetNamedPipeBinding();
                    netNamedPipeBinding.MaxBufferSize = 6553600;
                    netNamedPipeBinding.MaxReceivedMessageSize = 6553600;
                    //netNamedPipeBinding.ReaderQuotas.MaxStringContentLength = 819200;
                    AgentHost.AddServiceEndpoint(typeof(IPS01), netNamedPipeBinding, "PollerService");
                    AgentHost.Open();

                    // Create UDP client.
                    AgentUdpClient = new UdpClient();
                    AgentBytes = new byte[Encoding.ASCII.GetByteCount(AgentName) + 32];
                    Encoding.ASCII.GetBytes(AgentName, 0, AgentName.Length, AgentBytes, 32);

                    // Start timer.
                    AgentTimer = new System.Windows.Forms.Timer(components);
                    AgentTimer.Tick += new EventHandler(AgentTimer_Tick);
                    AgentTimer.Interval = AgentHeartbeat;
                    AgentTimer.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                AgentLog("InitializeAgent() error : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + " - " + ex.Message + "\r\n" + ex.StackTrace);
                if (AgentWaitHandle != null)
                {
                    AgentWaitHandle.Close();
                    AgentWaitHandle = null;
                }
                if (AgentHost != null)
                {
                    try
                    {
                        AgentHost.Close();
                    }
                    catch
                    {
                        AgentHost.Abort();
                    }
                    AgentHost = null;
                }
                if (AgentUdpClient != null)
                {
                    try
                    {
                        AgentUdpClient.Close();
                    }
                    catch
                    {
                        // Do nothing.
                    }
                    AgentUdpClient = null;
                }
            }
        }

        private void AgentTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Encode AgentTaskCount.
                AgentBytes[0] = (byte)AgentTaskCount;
                AgentBytes[1] = (byte)(AgentTaskCount >> 8);
                AgentBytes[2] = (byte)(AgentTaskCount >> 16);
                AgentBytes[3] = (byte)(AgentTaskCount >> 24);

                // Encode AgentQueuedDownload.
                AgentBytes[4] = (byte)AgentQueuedDownload;
                AgentBytes[5] = (byte)(AgentQueuedDownload >> 8);
                AgentBytes[6] = (byte)(AgentQueuedDownload >> 16);
                AgentBytes[7] = (byte)(AgentQueuedDownload >> 24);

                // Encode AgentLastDownloadSuccess.
                AgentBytes[8] = (byte)AgentLastDownloadSuccess;
                AgentBytes[9] = (byte)(AgentLastDownloadSuccess >> 8);
                AgentBytes[10] = (byte)(AgentLastDownloadSuccess >> 16);
                AgentBytes[11] = (byte)(AgentLastDownloadSuccess >> 24);
                AgentBytes[12] = (byte)(AgentLastDownloadSuccess >> 32);
                AgentBytes[13] = (byte)(AgentLastDownloadSuccess >> 40);
                AgentBytes[14] = (byte)(AgentLastDownloadSuccess >> 48);
                AgentBytes[15] = (byte)(AgentLastDownloadSuccess >> 56);

                // Encode AgentLastPriceSuccess.
                AgentBytes[16] = (byte)AgentLastPriceSuccess;
                AgentBytes[17] = (byte)(AgentLastPriceSuccess >> 8);
                AgentBytes[18] = (byte)(AgentLastPriceSuccess >> 16);
                AgentBytes[19] = (byte)(AgentLastPriceSuccess >> 24);
                AgentBytes[20] = (byte)(AgentLastPriceSuccess >> 32);
                AgentBytes[21] = (byte)(AgentLastPriceSuccess >> 40);
                AgentBytes[22] = (byte)(AgentLastPriceSuccess >> 48);
                AgentBytes[23] = (byte)(AgentLastPriceSuccess >> 56);

                // Encode AgentLastPriceChange.
                AgentBytes[24] = (byte)AgentLastPriceChange;
                AgentBytes[25] = (byte)(AgentLastPriceChange >> 8);
                AgentBytes[26] = (byte)(AgentLastPriceChange >> 16);
                AgentBytes[27] = (byte)(AgentLastPriceChange >> 24);
                AgentBytes[28] = (byte)(AgentLastPriceChange >> 32);
                AgentBytes[29] = (byte)(AgentLastPriceChange >> 40);
                AgentBytes[30] = (byte)(AgentLastPriceChange >> 48);
                AgentBytes[31] = (byte)(AgentLastPriceChange >> 56);

                // Send heartbeat.
                AgentUdpClient.Send(AgentBytes, AgentBytes.Length, AgentEndPoint);
            }
            catch
            {
                // Do nothing.
            }
        }

        private void TerminateAgentWrapper()
        {
            if (AgentSemiautomatic)
            {
                if (AgentWaitHandle != null)
                {
                    AgentWaitHandle.Close();
                    AgentWaitHandle = null;
                }
                if (AgentHost != null)
                {
                    try
                    {
                        AgentHost.Close();
                    }
                    catch (Exception ex)
                    {
                        AgentLog("TerminateAgent() error : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + " - " + ex.Message + "\r\n" + ex.StackTrace);
                        AgentHost.Abort();
                    }
                    AgentHost = null;
                }
                if (AgentUdpClient != null)
                {
                    try
                    {
                        AgentUdpClient.Close();
                    }
                    catch (Exception ex)
                    {
                        AgentLog("TerminateAgent() error : " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff ") + " - " + ex.Message + "\r\n" + ex.StackTrace);
                    }
                    AgentUdpClient = null;
                }
            }
        }

        private static void AgentLog(string value)
        {
            try
            {
                File.AppendAllText(Path.Combine(AgentLogFolder, "AgentWrapper.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + value + "\r\n");
            }
            catch
            {
                // Do nothing.
            }
        }
    }
}
