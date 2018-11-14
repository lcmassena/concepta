using Concepta.Application.Queries;
using System.Collections.Generic;

namespace Concepta.Services.TravelLogix.Models
{
    /// <summary>
    /// Request data to TravelLogix Api
    /// </summary>
    public class TravelLogixApiRequest
    {
        public string Language { get; set; }
        public string Currency { get; set; }
        public string Destination { get; set; }
        public string DateFrom { get; set; }
        public string DateTO { get; set; }
        public _Occupancy Occupancy { get; set; }

        public class _Occupancy
        {
            public string AdultCount { get; set; }
            public string ChildCount { get; set; }
            public IEnumerable<string> ChildAges { get; set; }
        }

        public static TravelLogixApiRequest ToRequest(TicketAvailabilityQuery query)
        {
            return new TravelLogixApiRequest()
            {
                Currency = query.Currency,
                DateFrom = query.DateFrom.ToString("MM/dd/yyyy"),
                DateTO = query.DateTO.ToString("MM/dd/yyyy"),
                Destination = query.Destination,
                Language = query.Language,
                Occupancy = new _Occupancy()
                {
                    AdultCount = (query.Occupancy?.AdultCount ?? 0).ToString(),
                    ChildAges = query.Occupancy?.ChildAges,
                    ChildCount = (query.Occupancy?.ChildCount ?? 0).ToString()
                }
            };
        }
    }
}