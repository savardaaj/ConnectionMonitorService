using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDPService
{
    public class Connection
    {
        public string protocol { get; set; }
        public string hostIP { get; set; }
        public string targetIP { get; set; }
        public string status { get; set; }
        public string labName { get; set; }
        public string serviceHost { get; set; }
    }
}
