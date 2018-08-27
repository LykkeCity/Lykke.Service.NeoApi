using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.Service.NEO.Api.Services.Models
{
    public class AddressInfo
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public string address { get; set; }
        public bool isvalid { get; set; }
    }
}
