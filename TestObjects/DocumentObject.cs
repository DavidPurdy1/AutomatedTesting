using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace ConsoleTests
{
    public class DocumentObject
    {
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        public int TestCaseId { get; set; }
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PageCount { get; set; }
        public List<string> CustomFieldData { get; set; } = new List<string>();


        public DocumentObject() {
            DocumentId = Guid.NewGuid().ToString();
        }
        public void PrintInfo() {
            Print(JsonConvert.SerializeObject(this));
        }
        public void Print(string toPrint, string method = "") {
            debugLog.Info(method + " " + toPrint);
        }
    }
}