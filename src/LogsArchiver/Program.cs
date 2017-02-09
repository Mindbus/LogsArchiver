using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogsArchiver.Filter;
using LogsArchiver.Input;
using LogsArchiver.Output;
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
            
            var input = CreateSingleInstance<IInput>(Configuration, "input");
            var filters = CreateMultipleInstances<IFilter>(Configuration, "filters").ToList();
            var output = CreateSingleInstance<IOutput>(Configuration, "output");

            Run(input, filters, output).Wait();
        }

        private static async Task Run(IInput input, IEnumerable<IFilter> filters, IOutput output)
        {
            var files = await input.GetFiles();
#if DEBUG
            foreach (var logFile in files)
            {
                Console.WriteLine($"{logFile.FileName} - {logFile.TimeStamp}");
            }
#endif

            foreach (var filter in filters)
            {
                files = await filter.Filter(files);
            }

#if DEBUG
            foreach (var logFile in files)
            {
                Console.WriteLine($"{logFile.FileName} - {logFile.TimeStamp}");
            }
#endif

            foreach (var logFile in files)
            {
                await output.Archive(logFile);
            }
        }

        private static T CreateSingleInstance<T>(IConfigurationRoot configuration, string configurationPrefix)
        {
            var objectType = typeof(T);
            var namespacePrexif = objectType.Namespace;
            var inputTypeName = namespacePrexif + "." + configuration[configurationPrefix + ":type"];
            var type = Type.GetType(inputTypeName);
            return (T)Activator.CreateInstance(type, configuration);
        }

        private static IEnumerable<T> CreateMultipleInstances<T>(IConfigurationRoot configuration, string configurationPrefix)
        {
            var objectType = typeof(T);
            var namespacePrexif = objectType.Namespace;

            var keys = GetKeysIfTheyExist(configuration, configurationPrefix, "type");

            return keys.Select(key =>
            {
                var inputTypeName = namespacePrexif + "." + Configuration[key.Key];
                var type = Type.GetType(inputTypeName);
                return (T) Activator.CreateInstance(type, Configuration, key.Index);
            });
        }

        private static IEnumerable<SettingKey> GetKeysIfTheyExist(IConfigurationRoot configuration, string configurationPrefix, string configurationsufix)
        {
            int index = 0;
            do
            {
                var key = $"{configurationPrefix}:{index}:{configurationsufix}";
                if (configuration[key] == null)
                {
                    break;
                }
                yield return new SettingKey { Index = index, Key = key };
                index++;
            } while (true);
        }

        private struct SettingKey
        {
            public string Key { get; set; }
            public int Index { get; set; }
        }
    }
}
