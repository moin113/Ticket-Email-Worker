using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEmailWorker.Model
{
    public class EmailLog
    {
        public string CCEmail { get; set; } = string.Empty;
        public string BccEmail { get; set; } = string.Empty;
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;

        public DateTime SentAt { get; set; } 

    }
}
