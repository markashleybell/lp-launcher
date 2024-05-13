using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace lp_launcher
{
    public static class Program
    {
        private const string _installPathV5 = @"C:\Program Files (x86)\LINQPad5\LINQPad.exe";
        private const string _installPathV8 = @"C:\Program Files\LINQPad8\LINQPad8.exe";

        private static int Main(string[] args)
        {
            var queryFile = args[0];

            string executablePath;

            using var fs = new StreamReader(queryFile);

            var headerLines = new StringBuilder();

            var line = fs.ReadLine();

            var singleLineHeaderMatch = Regex.Match(line, "<Query Kind=\"(?<type>.*?)\" ?/>", RegexOptions.IgnoreCase);

            if (singleLineHeaderMatch.Success)
            {
                // There is no RuntimeVersion element in the header,
                // but we still want to launch V8 if this is an F# query

                var type = singleLineHeaderMatch.Groups["type"].Value;

                executablePath = type.Equals("FSharpProgram")
                    ? _installPathV8
                    : _installPathV5;
            }
            else
            {
                // This is a block header, so keep reading lines until we find
                // the end, then launch V8 if this query uses a newer runtime

                headerLines.Append(line);

                while (line != "</Query>")
                {
                    line = fs.ReadLine();
                    headerLines.Append(line);
                }

                var header = XDocument.Parse(headerLines.ToString()).Root;

                executablePath = header.Element("RuntimeVersion") is object
                    ? _installPathV8
                    : _installPathV5;
            }

            Process.Start(executablePath, queryFile);

            return 0;
        }
    }
}
