using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Security.Core.Domain
{
    public partial class Client
    {
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Name { get; set; }
        public string RedirectUri { get; set; }
        public bool Enabled { get; set; }
    }
}
