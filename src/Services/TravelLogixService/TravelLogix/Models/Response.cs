using Concepta.Application.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Concepta.Services.TravelLogix.Models
{
    public class DateFrom
    {
        public string Date { get; set; }
        public object Time { get; set; }
    }

    public class DateTo
    {
        public string Date { get; set; }
        public object Time { get; set; }
    }

    public class Currency
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class Classification
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class DescriptionList
    {
        public string Type { get; set; }
        public string LanguageCode { get; set; }
        public object LanguageName { get; set; }
        public string Value { get; set; }
    }

    public class ImageList
    {
        public string Type { get; set; }
        public string Order { get; set; }
        public string VisualizationOrder { get; set; }
        public object Description { get; set; }
        public string Url { get; set; }
    }

    public class TicketInfo
    {
        public string TicketClass { get; set; }
        public Destination Destination { get; set; }
        public Classification Classification { get; set; }
        public List<object> TicketFeature { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<DescriptionList> DescriptionList { get; set; }
        public List<ImageList> ImageList { get; set; }
    }

    public class Address
    {
        public object StreetTypeId { get; set; }
        public object StreetTypeName { get; set; }
        public object StreetName { get; set; }
        public object Number { get; set; }
        public object PostalCode { get; set; }
        public object City { get; set; }
        public object State { get; set; }
        public object CountryCode { get; set; }
    }

    public class ContactInfo
    {
        public Address Address { get; set; }
        public List<object> EmailList { get; set; }
        public List<object> PhoneList { get; set; }
        public List<object> FaxList { get; set; }
        public List<object> webList { get; set; }
    }

    public class IncomingOffice
    {
        public object Description { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public object FiscalNumber { get; set; }
        public string Code { get; set; }
    }

    public class Contract
    {
        public string Name { get; set; }
        public IncomingOffice IncomingOffice { get; set; }
    }

    public class PriceList
    {
        public double Amount { get; set; }
        public string Description { get; set; }
    }

    public class Type
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class Mode
    {
        public string Code { get; set; }
        public string Value { get; set; }
    }

    public class OperationDateList
    {
        public string Date { get; set; }
        public string MinimumDuration { get; set; }
        public string MaximumDuration { get; set; }
    }

    public class ChildAge
    {
        public string AgeFrom { get; set; }
        public string AgeTo { get; set; }
    }

    public class AvailableModality
    {
        public string Name { get; set; }
        public Contract Contract { get; set; }
        public List<PriceList> PriceList { get; set; }
        public Type Type { get; set; }
        public Mode Mode { get; set; }
        public List<OperationDateList> OperationDateList { get; set; }
        public ChildAge ChildAge { get; set; }
        public string ContentSequence { get; set; }
        public string Code { get; set; }
    }

    public class Document
    {
        public object DocHolderName { get; set; }
        public List<object> DocLimitations { get; set; }
        public object DocIssueAuthority { get; set; }
        public object DocIssueLocation { get; set; }
        public object DocID { get; set; }
        public object DocType { get; set; }
        public object EffectiveDate { get; set; }
        public object ExpireDate { get; set; }
    }

    public class BirthDate
    {
        public object Date { get; set; }
        public object Time { get; set; }
    }

    public class GuestListClass
    {
        public bool AgeFieldSpecified { get; set; }
        public object CustomerId { get; set; }
        public string Age { get; set; }
        public object Name { get; set; }
        public object LastName { get; set; }
        public List<object> AdditionalInfo { get; set; }
        public Document Document { get; set; }
        public BirthDate BirthDate { get; set; }
        public object CountryCode { get; set; }
        public object GuestList { get; set; }
        public string Type { get; set; }
    }

    public class Paxes
    {
        public string AdultCount { get; set; }
        public bool AdultCounSpecified { get; set; }
        public string ChildCount { get; set; }
        public bool ChildCountSpecified { get; set; }
        public object ChildAges { get; set; }
        public List<GuestListClass> GuestList { get; set; }
    }

    public class Address2
    {
        public object StreetTypeId { get; set; }
        public object StreetTypeName { get; set; }
        public object StreetName { get; set; }
        public object Number { get; set; }
        public object PostalCode { get; set; }
        public object City { get; set; }
        public object State { get; set; }
        public object CountryCode { get; set; }
    }

    public class ContactInfo2
    {
        public Address2 Address { get; set; }
        public List<object> EmailList { get; set; }
        public List<object> PhoneList { get; set; }
        public List<object> FaxList { get; set; }
        public List<object> webList { get; set; }
    }

    public class IncomingOffice2
    {
        public object Description { get; set; }
        public ContactInfo2 ContactInfo { get; set; }
        public object FiscalNumber { get; set; }
        public string Code { get; set; }
    }

    public class Reference
    {
        public object FileNumber { get; set; }
        public IncomingOffice2 IncomingOffice { get; set; }
    }

    public class Supplier
    {
        public object Name { get; set; }
        public object VatNumber { get; set; }
        public object Reference { get; set; }
    }

    public class SellingPrice
    {
        public string Mandatory { get; set; }
        public string Value { get; set; }
    }

    public class Currency2
    {
        public object Code { get; set; }
        public object Value { get; set; }
    }

    public class AdditionalCostList
    {
        public Currency2 Currency { get; set; }
        public string PvpEquivalent { get; set; }
        public List<object> AdditionalCost { get; set; }
    }

    public class Result
    {
        public string AvailToken { get; set; }
        public DateFrom DateFrom { get; set; }
        public DateTo DateTo { get; set; }
        public Currency Currency { get; set; }
        public string ProviderCurrency { get; set; }
        public TicketInfo TicketInfo { get; set; }
        public List<AvailableModality> AvailableModality { get; set; }
        public Paxes Paxes { get; set; }
        public List<object> CancellationPolicyList { get; set; }
        public List<object> ServiceDetailList { get; set; }
        public Reference Reference { get; set; }
        public string Status { get; set; }
        public string DirectPayment { get; set; }
        public List<object> ContractList { get; set; }
        public Supplier Supplier { get; set; }
        public List<object> CommentList { get; set; }
        public List<object> AcceptedCardTypes { get; set; }
        public string TotalAmount { get; set; }
        public string NetPrice { get; set; }
        public string Commission { get; set; }
        public SellingPrice SellingPrice { get; set; }
        public string ValuationFileNumber { get; set; }
        public List<object> SupplementList { get; set; }
        public List<object> DiscountList { get; set; }
        public AdditionalCostList AdditionalCostList { get; set; }
        public List<object> ServiceExtraInfoList { get; set; }
        public List<object> ErrorList { get; set; }
        public List<object> ModificationPolicyList { get; set; }
        public object SPUI { get; set; }
    }

    public class TravelLogixApiResponse
    {
        public string Code { get; set; }
        public object ErrorMessage { get; set; }
        public List<Result> Result { get; set; }

        public static TicketAvailabilityQueryResponse ToDomain(TravelLogixApiResponse response)
        {
            var ticketInto = response.Result?.FirstOrDefault()?.TicketInfo;

            return new TicketAvailabilityQueryResponse()
            {
                Classification = ticketInto?.Classification?.Code,
                Code = ticketInto?.Code,
                Description = ticketInto?.DescriptionList?.FirstOrDefault().Value,
                ImageFull = ticketInto?.ImageList?.FirstOrDefault(x => x.Type == "S")?.Url,
                ImageThumb = ticketInto?.ImageList?.FirstOrDefault(x => x.Type == "L")?.Url,
                Destination = ticketInto?.Destination?.Code,
                Name = ticketInto?.Name,
                AvailableModality = response?.Result?.SelectMany(x => x.AvailableModality)?.Select(x => new _AvailableModality()
                {
                    Code = x.Code,
                    Contract = x.Contract?.Name ?? "Error retrieving",
                    Name = x.Name,
                    OperationDateList = x.OperationDateList?.Select(odl => new _OperationDateList() { From = odl.Date, To = odl.Date }),
                    ServicePrice = x.PriceList?.Select(p => p.Amount)?.FirstOrDefault() ?? 0
                })
            };
        }
    }
}
