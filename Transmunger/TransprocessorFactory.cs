using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Transmunger
{
    public class TransprocessorFactory
    {
        public static RegexProcessor DeserializeProcessor(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                DataContractSerializer ser = new DataContractSerializer(typeof(RegexProcessor));
                return (RegexProcessor)ser.ReadObject(ms);
            }
        }

        internal static List<RegexProcessor> DeserializeProcessors(string processors)
        {
            var processorList = new List<RegexProcessor>();
            foreach (var processor in processors.Split('-'))
            {
                processorList.Add(TransprocessorFactory.DeserializeProcessor(processor));
            }
            return processorList;
        }
    }
}
