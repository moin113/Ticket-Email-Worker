using Refit;
using System.Threading.Tasks;
using TicketEmailWorker.Model;

namespace TicketEmailWorker.Contracts
{
    public interface IFlightApiClient
    {
        [Get("/api/ticketdetail/{ticketdetailId}")]
        Task<TicketDetails> GetTicketDetails(int ticketdetailId);
    }
}
