using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AutomatedTestingLib {
    public class MetaDataObject {

        static readonly ILog debugLog = LogManager.GetLogger("Automated Testing Logs");
        public List<int> Numbers { get; set; } = new List<int>();

        public List<string> Strings { get; set; } = new List<string>();

        public List<DateTime> Dates { get; set; } = new List<DateTime>();

        public void AddData(object arg) {

                var type = arg.GetType();
                if (GetListType(Dates).Equals(type)) {
                    Dates.Add((DateTime)arg);
                } else if (GetListType(Numbers).Equals(type)) {
                    Numbers.Add((int)arg);
                } else if (GetListType(Strings).Equals(type)) {
                    Strings.Add((string)arg);
                } else {
                    Print("Inputted Type:" + type.ToString() + " is not string, int, or DateTime");
                    throw new InvalidOperationException("Inputted Type:" + type.ToString() + " is not string, int, or DateTime");
                }

        }
        public int GetCustomFieldCount() {
            return Numbers.Count + Strings.Count + Dates.Count; 
        }
        private static Type GetListType(object list) {
            if (list == null)
                throw new ArgumentNullException("list inputted is null");

            var type = list.GetType();

            if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(List<>))
                throw new ArgumentException("Type must be List<>, but was " + type.FullName, "someList");

            return type.GetGenericArguments()[0];
        }
        public string ConvertToString() {
            return JsonConvert.SerializeObject(this);
        }
        public void PrintInfo() {
            JsonConvert.SerializeObject(this); 
        }
        public void Print(string toPrint, string method = "") {
            debugLog.Info(method + " " + toPrint);
        }
    }
}