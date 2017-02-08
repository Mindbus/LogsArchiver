using System.Threading.Tasks;

namespace LogsArchiver.Output
{
    public interface IOutput
    {
        Task Archive(LogFile logFile);
    }
}
