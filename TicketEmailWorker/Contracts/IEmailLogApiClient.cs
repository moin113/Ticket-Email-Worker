using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketEmailWorker.Model;

namespace TicketEmailWorker.Contracts
{
    public interface IEmailLogApiClient
    {
        [Post("/api/email/log")]
        Task LogEmailAsync([Body] EmailLog emailLog);
    }
}
