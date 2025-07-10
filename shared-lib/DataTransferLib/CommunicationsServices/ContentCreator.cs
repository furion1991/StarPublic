using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DataTransferLib.CommunicationsServices
{
    public static class ContentCreator
    {
        public static StringContent CreateStringContent(string jsonContent)
        {
            return new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }
    }
}
