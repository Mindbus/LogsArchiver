using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogsArchiver.Filter
{
    public interface IFilter
    {
        Task<IEnumerable<LogFile>> Filter(IEnumerable<LogFile> files);
    }
}
