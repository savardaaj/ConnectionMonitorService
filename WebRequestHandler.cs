using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RDPService
{
    class WebRequestHandler
    {
        public WebRequestHandler()
        {

        }

        public string postData(string url, List<Connection> connections)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            var result = "";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strRet = JsonConvert.SerializeObject(connections);
                streamWriter.Write(strRet);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}
