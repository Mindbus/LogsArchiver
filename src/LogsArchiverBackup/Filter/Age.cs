using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LogsArchiver.Filter
{
    public class Age : IFilter
    {
        private readonly IConfigurationRoot _configuration;
        private readonly int _index;
        private static readonly Regex AgeRegEx = new Regex(@"(?<ammount>[0-9]*)(?<quantifier>[d|h|m])", RegexOptions.Compiled);

        public Age(IConfigurationRoot configuration, int index)
        {
            _configuration = configuration;
            _index = index;
        }

        public async Task<IEnumerable<LogFile>> Filter(IEnumerable<LogFile> files)
        {
            var ageSpan = ReadAge();
            if (ageSpan == null)
            {
                return null;
            }
            var maxDate = DateTime.Now.Subtract((TimeSpan)ageSpan);
            return files.Where(f => f?.TimeStamp != null && f.TimeStamp < maxDate);
        }

        private TimeSpan? ReadAge()
        {
            var ageSetting = _configuration[$"filters:{_index}:age"];
            var match = AgeRegEx.Match(ageSetting);
            if (!match.Success)
            {
                return null;
            }

            var ammount = match.Groups["ammount"].Value;
            int ammountValue;
            if (!int.TryParse(ammount, out ammountValue))
            {
                return null;
            }

            var quantifyer = match.Groups["quantifier"].Value;
            int days, hours, minutes;
            days = hours = minutes = 0;
            switch (quantifyer)
            {
                case "d":
                    days = ammountValue;
                    break;
                case "h":
                    hours = ammountValue;
                    break;
                case "m":
                    minutes = ammountValue;
                    break;
                default:
                    return null;
            }
            return new TimeSpan(days, hours, minutes, 0);
        }
    }
}
