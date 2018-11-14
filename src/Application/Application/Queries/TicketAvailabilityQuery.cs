using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Concepta.Application.Queries
{
    /// <summary>
    /// This query search for hotels availability
    /// </summary>
    public class TicketAvailabilityQuery : IRequest<TicketAvailabilityQueryResponse>
    {
        public string Language { get; set; }
        public string Currency { get; set; }
        public string Destination { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTO { get; set; }
        public _Occupancy Occupancy { get; set; }

        public class _Occupancy
        {
            public int AdultCount { get; set; }
            public int ChildCount { get; set; }
            public IEnumerable<string> ChildAges { get; set; }
        }
    }


    /// <summary>
    /// This is the answer to the Search Room Availability Query
    /// </summary>
    public class TicketAvailabilityQueryResponse
    {
        public string Destination { get; set; }
        public string Code { get; set; }
        public string Classification { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageThumb { get; set; }
        public string ImageFull { get; set; }
        public IEnumerable<_AvailableModality> AvailableModality { get; set; }
    }
    public class _OperationDateList
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class _AvailableModality
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Contract { get; set; }
        public double ServicePrice { get; set; }
        public IEnumerable<_OperationDateList> OperationDateList { get; set; }
    }


}
