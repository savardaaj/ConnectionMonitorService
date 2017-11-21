using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Timers;

namespace RDPService
{
    public partial class Service1 : ServiceBase
    {
        private List<Connection> connections = new List<Connection>();
        private string url = "http://localhost/Connections/Handler.ashx";
        private System.Timers.Timer timer;
        RDPMon rdpCons;

        public Service1()
        {
            InitializeComponent();
            rdpCons = new RDPMon();
            //timer = new Timer(30000D);  //30000 milliseconds = 30 seconds
            //timer.AutoReset = true;
            //timer.Elapsed += new ElapsedEventHandler(timer_elasped); //function that will be called after 30 seconds
            connections = new List<Connection>();
            
            // while loop strictly for testing. Use timer_elapsed in production
            while (true)
            {
                //Returns actual computer connections
                connections = rdpCons.GetRDPConnections();

                //Returns dummy data
                //connections = rdpCons.populateReturnList();
                
                postData(url, connections);
            }
        }

        //it will continue to be called after 30 second intervals
        //do some operation that you want to perform through this service here
        private void timer_elasped(object sender, ElapsedEventArgs e)
        {
            //Send this data to the server
            postData(url, connections);
        }

        /*
         * Creates an HttpWebRequest to POST the connection array in JSON to a web server specified in the URL
         */
        protected void postData(string url, List<Connection> connections)
        {
            try
            {
                WebRequestHandler wrh = new WebRequestHandler();
                wrh.postData(url, connections);
            }
            catch(Exception e)
            {
                //Server error probably
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                timer.Start();
            }
            catch (Exception ex)
            {
                //log anywhere
            }
        }

        protected override void OnStop()
        {
            timer.Stop();
        }
    }
}
