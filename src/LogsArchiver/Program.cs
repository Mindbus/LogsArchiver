using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogsArchiver.Input;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver
{
    public class Program
    {
        internal static IConfigurationRoot Configuration { get; private set; }

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");
            Configuration = builder.Build();
            
            const string prefix = "LogsArchiver.Input.";
            var inputTypeName = prefix + Configuration["input:type"];
            var type = Type.GetType(inputTypeName);
            var input = (IInput)Activator.CreateInstance(type, Configuration);

            var files = input.GetFiles().Result;
            foreach (var logFile in files)
            {
                Console.WriteLine($"{logFile.FileName} - {logFile.TimeStamp}");
            }
        }
    }
}
