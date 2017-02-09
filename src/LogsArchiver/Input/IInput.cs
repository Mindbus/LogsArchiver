using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogsArchiver.Input
{
    public interface IInput
    {
        Task<IEnumerable<LogFile>> GetFiles();
    }
}
