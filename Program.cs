using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace lp_launcher
{
    public static class Program
    {
        private const string _installPathV5 = @"C:\Program Files (x86)\LINQPad5\LINQPad.exe";
        private const string _installPathV6 = @"C:\Program Files\LINQPad6\LINQPad6.exe";

        private static void Main(string[] args)
        {
            var queryFile = args[0];

            using var fs = new StreamReader(queryFile);

            var headerLines = new StringBuilder();

            var line = default(string);

            while (line != "</Query>")
            {
                line = fs.ReadLine();
                headerLines.Append(line);
            }

            var header = XDocument.Parse(headerLines.ToString()).Root;

            var executablePath = header.Element("RuntimeVersion") is object
                ? _installPathV6
                : _installPathV5;

            Process.Start(executablePath, queryFile);
        }
    }
}
