using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsArchiver
{
    public class LogFile
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
