using Oqtane.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Documentation;
using Oqtane.Shared;

namespace Oqtane.Services
{
    [PrivateApi("Don't show in the documentation, as everything should use the Interface")]
    public class JobLogService : ServiceBase, IJobLogService
    {
        private readonly SiteState _siteState;

        public JobLogService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl("JobLog", _siteState.Alias);

        public async Task<List<JobLog>> GetJobLogsAsync()
        {
            List<JobLog> joblogs = await GetJsonAsync<List<JobLog>>(Apiurl);
            return joblogs.OrderBy(item => item.StartDate).ToList();
        }

        public async Task<JobLog> GetJobLogAsync(int jobLogId)
        {
            return await GetJsonAsync<JobLog>($"{Apiurl}/{jobLogId}");
        }
    }
}
