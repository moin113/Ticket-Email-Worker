using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketEmailWorker.Model
{
    public class TicketDetails
    {


        public int Id { get; set; }
        public string PhoneNo { get; set; }
        public string EmailId { get; set; }
        public int NumberOfSeat { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? AdultPrice { get; set; }
        public decimal? ChildPrice { get; set; }
        public decimal? InfantPrice { get; set; }
        public string? FlightName { get; set; }
        public string? FlightCode { get; set; }
        public DateTime? FlightDate { get; set; }
        public string? FromCity { get; set; }
        public string? ToCity { get; set; }
        public string? FromCityCode { get; set; }
        public string? ToCityCode { get; set; }
        public string? DepartureTime { get; set; }
        public string? ArrivalTime { get; set; }
        public DateTime RequestedDate { get; set; }
        public IEnumerable<Model.PassengerDetails> PassengerDetails { get; set; }
        public string? Username { get; set; }
        public string? PNRNumber { get; set; }
        public bool IsCancelled { get; set; }
        public int? TicketDetailId { get; set; }
        public string? Destination { get; set; }

        public DateTime? CancelledAt { get; set; }
        public decimal? RefundAmount { get; set; }

        public string? Status { get; set; }

        //public virtual TblPaymentDetail TblPaymentDetail { get; set; } = null!;
        public string? TransactionId { get; set; }

        public int? RouteType { get; set; }

        public int AdultCount => PassengerDetails != null ? PassengerDetails.Count(x => x?.IsAdult == true) : 0;
        public int ChildCount => PassengerDetails != null ? PassengerDetails.Count(x => x?.IsAdult == true) : 0;
        public int InfantCount => PassengerDetails != null ? PassengerDetails.Count(x => x?.IsAdult == true) : 0;
        public bool IsChild => ChildCount > 0;
        public bool IsInfant => InfantCount > 0;
        public string FormattedFlightDate
        {
            get
            {
                return FlightDate.HasValue ? FlightDate.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string FormattedRequestedDate
        {
            get
            {
                return RequestedDate != default(DateTime) ? RequestedDate.ToString("dd/MM/yyyy") : string.Empty;
            }
        }


        public string FormattedDepartureTime
        {
            get
            {
                if (!string.IsNullOrEmpty(DepartureTime))
                {
                    if (DateTime.TryParse(DepartureTime, out DateTime departureDateTime))
                    {
                        return departureDateTime.ToString("HH:mm") + " HRS";
                    }
                }
                return string.Empty;
            }
        }

        public string FormattedArrivalTime
        {
            get
            {
                if (!string.IsNullOrEmpty(ArrivalTime))
                {
                    if (DateTime.TryParse(ArrivalTime, out DateTime arrivalDateTime))
                    {
                        return arrivalDateTime.ToString("HH:mm") + " HRS";
                    }
                }
                return string.Empty;
            }
        }




        public string DepartureToArrivalDuration
        {
            get
            {
                if (!string.IsNullOrEmpty(DepartureTime) && !string.IsNullOrEmpty(ArrivalTime))
                {
                    TimeSpan departure = TimeSpan.Parse(DepartureTime);
                    TimeSpan arrival = TimeSpan.Parse(ArrivalTime);
                    TimeSpan duration = arrival - departure;

                    if (duration.TotalMinutes < 0)
                    {
                        duration = TimeSpan.FromHours(24) + duration;
                    }

                    return $"{(int)duration.TotalHours} hours {(int)duration.Minutes} minutes";
                }
                return string.Empty;
            }
        }

    }
}
