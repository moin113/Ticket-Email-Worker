using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEmailWorker.Model
{
    public class PassengerDetails
    {
        public int Id { get; set; }
        public int RouteDetailId { get; set; }
        public string Title { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsWheelChairRequired { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool IsInfant { get; set; }
        public bool? IsAdult { get; set; }
        public bool IsChild { get; set; }
        public string? Sex { get; set; }
        public string? SeatNumber { get; set; }
        public string? TicketNumber { get; set; }
        public int? TicketDetailId { get; set; }
        public string? PassportNumber { get; set; }
        public DateTime? DateofExpiry { get; set; }
        public string? Nationality { get; set; }
    }
}
