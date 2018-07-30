using NotVisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;

namespace pets4home.core
{
    class CsvReader
    {

        private readonly StringReader reader;
        private readonly CsvTextFieldParser parser;
        private readonly string[] headers;
        private readonly Queue<IDictionary<string, string>> rows = new Queue<IDictionary<string, string>>();

        public CsvReader(string path)
        {
            reader = new StringReader(File.ReadAllText(path));
            parser = new CsvTextFieldParser(reader);
            if (parser.EndOfData)
            {
                throw new Exception("Unexpected csv format in file: " + path);
            }
            using (reader)
            using (parser)
            {

                headers = TrimFields(parser.ReadFields());

                while (!parser.EndOfData)
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();
                    string[] fields = TrimFields(parser.ReadFields());
                    int i = 0;
                    foreach (string header in headers)
                    {
                        row[header] = fields[i];
                        i++;
                    }
                    rows.Enqueue(row);
                }
            }
        }

        public bool hasMoreRows()
        {
            return (rows.Count > 0);
        }

        public IDictionary<string, string> getNextRow()
        {
            return rows.Dequeue();
        }

        private string[] TrimFields(string[] strings)
        {
            List<string> trimmed = new List<string>();
            foreach (string s in strings)
            {
                trimmed.Add(s.Trim());
            }
            return trimmed.ToArray();
        }
    }
}