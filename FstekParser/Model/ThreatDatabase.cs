using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExcelDataReader;
using Newtonsoft.Json;

namespace FstekParser
{
    public class ThreatDatabase
    {
        private static readonly string dataLink = "https://bdu.fstec.ru/files/documents/thrlist.xlsx";
        private static readonly string baseDir = Directory.GetDirectoryRoot("d") + "FstekParser\\";
        private static readonly string threatsJSON = baseDir + "threats.JSON";

        public static readonly ThreatDatabase Instance = new ThreatDatabase();

        private ThreatDatabase()
        {
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
        }

        public List<Threat> GetThreats()
        {
            using (var file = File.OpenText(threatsJSON))
            {
                using (var jsonData = new JsonTextReader(file))
                {
                    var json = new JsonSerializer();
                    return json.Deserialize<List<Threat>>(jsonData);
                }
            }
        }


        public List<Threat> UpdateThreats()
        {
            var threats = DownloadThreatsFromFstek();

            SaveThreatsAsync(threats);
            return threats;
        }

        private async void SaveThreatsAsync(List<Threat> threats)
        {
            await Task.Run(() => SaveThreats(threats));
        }

        private void SaveThreats(List<Threat> threats)
        {
            using (var file = File.Create(threatsJSON))
            {
                using (var writer = new StreamWriter(file))
                {
                    var json = new JsonSerializer();
                    json.Serialize(writer, threats);
                }
            }
        }

        private List<Threat> DownloadThreatsFromFstek()
        {
            var threats = new List<Threat>();

            byte[] data;
            using (var client = new WebClient())
            {
                data = client.DownloadData(dataLink);
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (var dataStream = new MemoryStream(data))
            {
                using (var excel = ExcelReaderFactory.CreateReader(dataStream))
                {
                    var result = excel.AsDataSet();

                    var rows = result.Tables[0].Rows;

                    for (var i = 2; i < rows.Count; ++i)
                    {
                        var items = rows[i].ItemArray;

                        var threat = new Threat
                        {
                            Id = int.Parse(items[0].ToString()),
                            Name = items[1].ToString(),
                            Description = items[2].ToString(),
                            Source = items[3].ToString(),
                            Target = items[4].ToString(),
                            ConfidenceViolation = items[5].ToString().Equals("1"),
                            IntegrityViolation = items[6].ToString().Equals("1"),
                            AccessViolation = items[7].ToString().Equals("1"),
                            UploadDate = DateTime.Parse(items[8].ToString()),
                            LastModificationDate = DateTime.Parse(items[9].ToString())
                        };

                        threats.Add(threat);
                    }
                }
            }

            return threats;
        }
    }
}