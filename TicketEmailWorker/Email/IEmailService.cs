using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Threading;
using System.Threading.Tasks;

namespace TicketEmailWorker.Email
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string fromEmail,
                             string toEmail,
                             string subject,
                             string cCEmail,
                             string body,
                             CancellationToken cancellationToken);
    }
}


