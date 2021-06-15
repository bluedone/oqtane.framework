using Oqtane.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Oqtane.Services
{
    public interface IJobLogService
    {
        Task<List<JobLog>> GetJobLogsAsync();

        Task<JobLog> GetJobLogAsync(int jobLogId);
    }
}
