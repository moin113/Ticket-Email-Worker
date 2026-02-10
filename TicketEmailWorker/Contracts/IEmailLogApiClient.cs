using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEmailWorker.Contracts
{
    public interface IEmailLogApiClient
    {
        [Post("/api/email/log")]
        Task LogEmailAsync([Body] object payload);
    }
}
