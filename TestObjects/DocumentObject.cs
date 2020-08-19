using log4net;
using Newtonsoft.Json;
using System;

namespace AutomatedTestingLib {
    public class DocumentObject
    {
        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        public int TestCaseId { get; set; }
        public string DocumentId { get; set; }
        public string Type { get; set; }
        public string Definition { get; set; }
        public string Title { get; set; }
        public bool IsRecognized { get; set; } = false; 
        public string Author { get; set; }
        public string Summary { get; set; }
        public MetaDataObject MetaData { get; set; } = new MetaDataObject();

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