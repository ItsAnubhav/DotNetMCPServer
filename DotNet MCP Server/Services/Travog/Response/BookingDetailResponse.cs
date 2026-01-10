using System;
using System.Collections.Generic;

namespace McpServerApp.Services.Travog.Response
{
    public class BookingDetailResponse
    {
        public bool success { get; set; }
        public string message { get; set; }
        public BookingData data { get; set; }
    }

    public class BookingData
    {
        public BookingDetails bookingDetails { get; set; }
        public Dictionaries dictionaries { get; set; }
    }

    public class BookingDetails
    {
        public CompanyDetails companyDetails { get; set; }
        public MasterDetails masterDetails { get; set; }
        public ClientInfo clientInfo { get; set; }
        public PassengerInfo passengerInfo { get; set; }
        public Products products { get; set; }
        public PaymentDetails paymentDetails { get; set; }
    }

    #region Company

    public class CompanyDetails
    {
        public string code { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public string email { get; set; }
        public string cityCode { get; set; }
        public string countryCode { get; set; }
        public string postCode { get; set; }
        public string webSiteURL { get; set; }
        public string companyLang { get; set; }
        public string companyCur { get; set; }
    }

    #endregion

    #region Master

    public class MasterDetails
    {
        public long bookingRef { get; set; }
        public int branchId { get; set; }
        public int clientId { get; set; }
        public bool isSubagent { get; set; }
        public int b2bBranchId { get; set; }
        public string languageCode { get; set; }
        public string dateOfBooking { get; set; }
        public string tripStartDate { get; set; }
        public string destinationCode { get; set; }
        public bool onHold { get; set; }
        public string sourceName { get; set; }
        public bool bookingLockStatus { get; set; }
        public int lockedById { get; set; }
        public string lockedByName { get; set; }
        public string status { get; set; }
        public string balanceDueDate { get; set; }
        public string services { get; set; }
        public string salesChannel { get; set; }
        public bool isPackage { get; set; }
        public bool groupBooking { get; set; }
        public string bookingMode { get; set; }
        public PricingDetails pricingDetails { get; set; }
    }

    public class PricingDetails
    {
        public decimal totalNet { get; set; }
        public decimal totalWsGross { get; set; }
        public decimal totalB2B2BNet { get; set; }
        public decimal totalGross { get; set; }
        public decimal totalNetTax { get; set; }
        public decimal totalWSGrossTax { get; set; }
        public decimal totalWS { get; set; }
        public decimal totalTax { get; set; }
        public decimal b2B2BNetTaxCharge { get; set; }
        public decimal grossTaxCharge { get; set; }
    }

    #endregion

    #region Client

    public class ClientInfo
    {
        public int clientId { get; set; }
        public string clientName { get; set; }
        public string accountNo { get; set; }
        public int b2bUserId { get; set; }
        public string clientType { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string cityCode { get; set; }
        public string stateCode { get; set; }
        public string countryCode { get; set; }
        public string postCode { get; set; }
        public string phone { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public bool active { get; set; }
        public string paymentMethod { get; set; }
        public string financeEmailAddress { get; set; }
        public string financeEmailAlternate { get; set; }
        public bool financialDocPrinted { get; set; }
        public B2bBranch b2bBranch { get; set; }
        public B2bUser b2bUser { get; set; }
    }

    public class B2bBranch
    {
        public int id { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string cityCode { get; set; }
        public string stateCode { get; set; }
        public string postCode { get; set; }
        public string countryCode { get; set; }
        public string email { get; set; }
        public bool poolType { get; set; }
    }

    public class B2bUser
    {
        public int userId { get; set; }
        public string userTitle { get; set; }
        public string userFName { get; set; }
        public string userLName { get; set; }
        public string userPhoneNo { get; set; }
        public string userEmail { get; set; }
    }

    #endregion

    #region Passenger

    public class PassengerInfo
    {
        public int noOfAdult { get; set; }
        public int noOfChild { get; set; }
        public int noOfInfant { get; set; }
        public int noOfYouth { get; set; }
        public int noOfSenior { get; set; }
        public List<Pax> pax { get; set; }
    }

    public class Pax
    {
        public int id { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public bool leadPax { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public string alternateEmail { get; set; }
        public string phoneNo { get; set; }
        public string dateOfBirth { get; set; }
        public int age { get; set; }
        public Passport passport { get; set; }
        public string identityNo { get; set; }
        public string visaNumber { get; set; }
        public string visaExpiry { get; set; }
        public Preference preference { get; set; }
    }

    public class Passport
    {
        public string number { get; set; }
        public string expiryDate { get; set; }
        public string nationalityCode { get; set; }
        public string issueCountryCode { get; set; }
    }

    public class Preference
    {
        public string mealCode { get; set; }
        public string seatCode { get; set; }
        public string otherPref { get; set; }
    }

    #endregion

    #region Products

    public class Products
    {
        public List<AirInfo> airInfo { get; set; }
    }

    public class AirInfo
    {
        public int flightId { get; set; }
        public string originCode { get; set; }
        public string destinationCode { get; set; }
        public string airlineCode { get; set; }
        public string market { get; set; }
        public string refundable { get; set; }
        public string startDate { get; set; }
        public int supplierCode { get; set; }
        public string gds { get; set; }
        public string gdsName { get; set; }
        public string gdsPNR { get; set; }
        public string displaySupplierName { get; set; }
        public string lastTixDate { get; set; }
        public bool isInvoiced { get; set; }
        public bool isExitsSupplierCredential { get; set; }
        public string status { get; set; }
        public int noOfPassenger { get; set; }
        public string custCurrency { get; set; }
        public decimal custCurrencyROE { get; set; }
        public List<FlightSegment> flightSegment { get; set; }
        public List<AirPassenger> passengerInfo { get; set; }
        public TicketInfo ticketInfo { get; set; }
        public Fares fares { get; set; }
        public TaxBreakUp taxBreakUp { get; set; }
    }

    public class FlightSegment
    {
        public string flightNumber { get; set; }
        public string airlineCode { get; set; }
        public string airlinePNRNo { get; set; }
        public string segmentDestination { get; set; }
        public string depAirportCode { get; set; }
        public string depDate { get; set; }
        public string depTime { get; set; }
        public string depDay { get; set; }
        public string depTerminal { get; set; }
        public string arrAirportCode { get; set; }
        public string arrivalTerminal { get; set; }
        public string arrivalDate { get; set; }
        public string arrivalTime { get; set; }
        public string arrivalDay { get; set; }
        public string elapsedFlightTime { get; set; }
        public string @class { get; set; }
        public int noOfSeats { get; set; }
        public string status { get; set; }
    }

    public class AirPassenger
    {
        public int id { get; set; }
        public string infTravelerWith { get; set; }
    }

    public class TicketInfo
    {
        public List<Ticket> ticket { get; set; }
    }

    public class Ticket
    {
        public int id { get; set; }
        public int paxId { get; set; }
        public string number { get; set; }
        public string issueDate { get; set; }
        public string validatingAirline { get; set; }
        public string airlineNumericCode { get; set; }
        public string status { get; set; }
        public int createdById { get; set; }
        public string createdByName { get; set; }
    }

    public class Fares
    {
        public PaxType paxType { get; set; }
    }

    public class PaxType
    {
        public string type { get; set; }
        public string subType { get; set; }
        public int noOfAdult { get; set; }
        public string fareType { get; set; }
        public decimal baseFare { get; set; }
        public decimal transactionFee { get; set; }
        public decimal wsNett { get; set; }
        public decimal wsGross { get; set; }
        public decimal b2b2bNet { get; set; }
        public decimal netTax { get; set; }
        public decimal yq { get; set; }
    }

    public class TaxBreakUp
    {
        public AdtTaxBreakUp adtTaxBreakUp { get; set; }
    }

    public class AdtTaxBreakUp
    {
        public List<TaxDetail> taxDetails { get; set; }
    }

    public class TaxDetail
    {
        public int id { get; set; }
        public string key { get; set; }
        public string name { get; set; }
        public string passengerType { get; set; }
        public decimal suppCurrTax { get; set; }
        public decimal compCurrTax { get; set; }
        public bool supplierTax { get; set; }
    }

    #endregion

    #region Payment

    public class PaymentDetails
    {
        public List<Payment> payment { get; set; }
        public List<PaymentAllocation> paymentAllocation { get; set; }
    }

    public class Payment
    {
        public int paymentId { get; set; }
        public decimal amount { get; set; }
        public string payMode { get; set; }
        public string payDate { get; set; }
        public string payAllocationId { get; set; }
        public int agentId { get; set; }
        public string companyId { get; set; }
    }

    public class PaymentAllocation
    {
        public string paymentMode { get; set; }
        public string paymentModeDisplay { get; set; }
        public string paymentType { get; set; }
        public string cardName { get; set; }
        public string balanceDueDate { get; set; }
        public string paymentAllocationDate { get; set; }
        public string method { get; set; }
        public string cardCompany { get; set; }
        public string status { get; set; }
        public decimal amount { get; set; }
        public List<object> bifurcation { get; set; }
    }

    #endregion

    #region Dictionaries

    public class Dictionaries
    {
        public Dictionary<string, Airport> airport { get; set; }
        public Dictionary<string, City> city { get; set; }
        public Dictionary<string, Country> country { get; set; }
        public Dictionary<string, Airline> airline { get; set; }
    }

    public class Airport
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cityCode { get; set; }
        public string cityName { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string zoneCode { get; set; }
        public string zoneName { get; set; }
    }

    public class City
    {
        public string code { get; set; }
        public string name { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string zoneCode { get; set; }
        public string zoneName { get; set; }
    }

    public class Country
    {
        public string code { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
    }

    public class Airline
    {
        public string code { get; set; }
        public string name { get; set; }
        public string normalLogo { get; set; }
        public string squareLogo { get; set; }
    }

    #endregion
}
