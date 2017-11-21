using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RDPService
{
    class RDPMon
    {
        
        List<Connection> returnList = new List<Connection>();

        public RDPMon()
        {
            
        }
        
        /*
         *  Populates the variable ReturnList with dummy data used for testing 
         */
        public List<Connection> populateReturnList()
        {
            returnList.Add(new Connection
            {
                protocol = "TCP",
                status = "ACTIVE",
                targetIP = "192.168.0.1",
                hostIP = "192.168.0.0",
                serviceHost = Environment.MachineName,
                labName = "Home Connection"
            });
            returnList.Add(new Connection
            {
                targetIP = "192.168.0.1",
                serviceHost = Environment.MachineName,
                labName = "test1"
            });
            returnList.Add(new Connection
            {
                targetIP = "192.168.0.2",
                serviceHost = Environment.MachineName,
                labName = "test2"
            });
            returnList.Add(new Connection
            {
                targetIP = "192.168.0.3",
                serviceHost = Environment.MachineName,
                labName = "test3"
            });
            returnList.Add(new Connection
            {
                protocol = "TCP",
                status = "ACTIVE",
                targetIP = "192.168.0.5",
                hostIP = "192.168.0.0",
                serviceHost = Environment.MachineName,
                labName = "Client 3"
            });
            return returnList;
        }

        /*
         * Called from 
         */
        public List<Connection> GetRDPConnections()
        {
            List<Connection> ConnectionList = new List<Connection>();
            try
            {
                using (Process p = new Process())
                {
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                    startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardError = true;
                    startInfo.UseShellExecute = false;
                    startInfo.FileName = "cmd.exe";
                    //Returns connections for Teamviewer and RDP
                    startInfo.Arguments = "/C netstat -na | find \"5938\" & netstat -na | find \"3389\"";
                    p.StartInfo = startInfo;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus == "0")
                    {
                        //Get The Rows
                        string[] rows = Regex.Split(content, "\r\n");
                        foreach (string row in rows)
                        {
                            //Split it baby
                            string[] tokens = Regex.Split(row, "\\s+");
                            if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")) //if TCP or UDP
                                && (tokens[3].Split(':')[1].Equals("3389") || tokens[3].Split(':')[1].Equals("5938"))
                                && tokens[4].Equals("ESTABLISHED")) //If RDP/Teamviewer port and Established Connection
                            {
                                string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                                string targetAddress = Regex.Replace(tokens[3], @"\[(.*?)\]", "1.1.1.1");

                                ConnectionList.Add(new Connection
                                {
                                    protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                    hostIP = localAddress.Split(':')[0],
                                    targetIP = targetAddress.Split(':')[0],
                                    status = tokens[4],
                                    serviceHost = Environment.MachineName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return ConnectionList;
        }
    }
}
