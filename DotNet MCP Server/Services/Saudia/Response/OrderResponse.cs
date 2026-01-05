using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace McpServerApp.Services.Saudia.Responses;

    public record OrderResponse(
        [property: JsonPropertyName("data")] List<OrderData> Data,
        [property: JsonPropertyName("dictionaries")] Dictionaries Dictionaries
    );

    public record OrderData(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("numericId")] string NumericId,
        [property: JsonPropertyName("creationPointOfSale")] CreationPointOfSale CreationPointOfSale,
        [property: JsonPropertyName("creationDateTime")] DateTime CreationDateTime,
        [property: JsonPropertyName("expirationDateTime")] DateTime ExpirationDateTime,
        [property: JsonPropertyName("paymentTimeLimit")] DateTimeOffset PaymentTimeLimit,
        [property: JsonPropertyName("issuanceTimeLimit")] DateTimeOffset IssuanceTimeLimit,
        [property: JsonPropertyName("air")] Air Air,
        [property: JsonPropertyName("travelers")] List<Traveler> Travelers,
        [property: JsonPropertyName("contacts")] List<Contact> Contacts,
        [property: JsonPropertyName("remarks")] List<Remark> Remarks,
        [property: JsonPropertyName("specialServiceRequests")] List<SpecialServiceRequest> SpecialServiceRequests,
        [property: JsonPropertyName("orderEligibilities")] OrderEligibilities OrderEligibilities,
        [property: JsonPropertyName("orderTotal")] OrderTotal OrderTotal
    );

    public record CreationPointOfSale(
        [property: JsonPropertyName("pointOfSaleId")] string PointOfSaleId,
        [property: JsonPropertyName("countryCode")] string CountryCode
    );

    public record Air(
        [property: JsonPropertyName("prices")] Prices Prices,
        [property: JsonPropertyName("bounds")] List<Bound> Bounds,
        [property: JsonPropertyName("fareInfos")] List<FareInfo> FareInfos,
        [property: JsonPropertyName("freeCheckedBaggageAllowanceItems")]
        List<FreeCheckedBaggageAllowanceItem> FreeCheckedBaggageAllowanceItems
    );

    public record Prices(
        [property: JsonPropertyName("unitPrices")] List<UnitPrice> UnitPrices,
        [property: JsonPropertyName("totalPrices")] List<TotalPrice> TotalPrices
    );

    public record UnitPrice(
        [property: JsonPropertyName("travelerIds")] List<string> TravelerIds,
        [property: JsonPropertyName("flightIds")] List<string> FlightIds,
        [property: JsonPropertyName("prices")] List<PriceDetail> Prices
    );

    public record PriceDetail(
        [property: JsonPropertyName("base")] Money Base,
        [property: JsonPropertyName("total")] Money Total,
        [property: JsonPropertyName("taxes")] List<Tax> Taxes,
        [property: JsonPropertyName("totalTaxes")] Money TotalTaxes,
        [property: JsonPropertyName("discount")] Discount Discount
    );

    public record TotalPrice(
        [property: JsonPropertyName("base")] Money Base,
        [property: JsonPropertyName("total")] Money Total,
        [property: JsonPropertyName("totalTaxes")] Money TotalTaxes,
        [property: JsonPropertyName("discount")] Discount Discount
    );

    public record Money(
        [property: JsonPropertyName("value")] decimal Value,
        [property: JsonPropertyName("currencyCode")] string CurrencyCode
    );

    public record Tax(
        [property: JsonPropertyName("value")] decimal Value,
        [property: JsonPropertyName("currencyCode")] string CurrencyCode,
        [property: JsonPropertyName("code")] string Code
    );

    public record Discount(
        [property: JsonPropertyName("originalTotal")] decimal OriginalTotal,
        [property: JsonPropertyName("discountCode")] string DiscountCode
    );

    public record Bound(
        [property: JsonPropertyName("airBoundId")] string AirBoundId,
        [property: JsonPropertyName("fareFamilyCode")] string FareFamilyCode,
        [property: JsonPropertyName("originLocationCode")] string OriginLocationCode,
        [property: JsonPropertyName("destinationLocationCode")] string DestinationLocationCode,
        [property: JsonPropertyName("flights")] List<BoundFlight> Flights,
        [property: JsonPropertyName("duration")] int Duration
    );

    public record BoundFlight(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("cabin")] string Cabin,
        [property: JsonPropertyName("bookingClass")] string BookingClass,
        [property: JsonPropertyName("statusCode")] string StatusCode,
        [property: JsonPropertyName("fareFamilyCode")] string FareFamilyCode
    );

    public record FareInfo(
        [property: JsonPropertyName("fareClass")] string FareClass,
        [property: JsonPropertyName("ticketDesignator")] string TicketDesignator,
        [property: JsonPropertyName("travelerIds")] List<string> TravelerIds,
        [property: JsonPropertyName("flightIds")] List<string> FlightIds
    );

    public record FreeCheckedBaggageAllowanceItem(
        [property: JsonPropertyName("details")] BaggageDetails Details,
        [property: JsonPropertyName("flightIds")] List<string> FlightIds,
        [property: JsonPropertyName("travelerIds")] List<string> TravelerIds
    );

    public record BaggageDetails(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("quantity")] int Quantity
    );

    public record Traveler(
        [property: JsonPropertyName("passengerTypeCode")] string PassengerTypeCode,
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("names")] List<PersonName> Names,
        [property: JsonPropertyName("dateOfBirth")] DateTime DateOfBirth,
        [property: JsonPropertyName("regulatoryDetails")] List<RegulatoryDetail> RegulatoryDetails
    );

    public record PersonName(
        [property: JsonPropertyName("firstName")] string FirstName,
        [property: JsonPropertyName("lastName")] string LastName,
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("nameType")] string NameType,
        [property: JsonPropertyName("isPreferred")] bool IsPreferred
    );

    public record RegulatoryDetail(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("airlineCode")] string AirlineCode,
        [property: JsonPropertyName("regulatoryApisType")] string RegulatoryApisType,
        [property: JsonPropertyName("regulatoryDocument")] RegulatoryDocument RegulatoryDocument
    );

    public record RegulatoryDocument(
        [property: JsonPropertyName("number")] string Number,
        [property: JsonPropertyName("expiryDate")] DateTime ExpiryDate,
        [property: JsonPropertyName("issuanceCountryCode")] string IssuanceCountryCode,
        [property: JsonPropertyName("name")] SimpleName Name,
        [property: JsonPropertyName("nationalityCode")] string NationalityCode,
        [property: JsonPropertyName("gender")] string Gender,
        [property: JsonPropertyName("birthDate")] DateTime BirthDate,
        [property: JsonPropertyName("documentType")] string DocumentType
    );

    public record SimpleName(
        [property: JsonPropertyName("firstName")] string FirstName,
        [property: JsonPropertyName("lastName")] string LastName,
        [property: JsonPropertyName("nameType")] string NameType
    );

    public record Contact(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("travelerIds")] List<string>? TravelerIds,
        [property: JsonPropertyName("category")] string Category,
        [property: JsonPropertyName("contactType")] string ContactType,
        [property: JsonPropertyName("deviceType")] string? DeviceType,
        [property: JsonPropertyName("purpose")] string Purpose,
        [property: JsonPropertyName("countryPhoneExtension")] string? CountryPhoneExtension,
        [property: JsonPropertyName("number")] string? Number,
        [property: JsonPropertyName("address")] string? Address,
        [property: JsonPropertyName("lang")] string? Lang,
        [property: JsonPropertyName("freeFlowText")] string? FreeFlowText
    );

    public record Remark(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("remarkType")] string RemarkType,
        [property: JsonPropertyName("freetext")] string Freetext
    );

    public record SpecialServiceRequest(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("code")] string Code,
        [property: JsonPropertyName("airlineCode")] string AirlineCode,
        [property: JsonPropertyName("statusCode")] string StatusCode,
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("freetext")] string Freetext,
        [property: JsonPropertyName("travelerIds")] List<string>? TravelerIds
    );

    public record OrderEligibilities(
        [property: JsonPropertyName("typeOfPnr")] string TypeOfPnr,
        [property: JsonPropertyName("pnrTypeId")] int PnrTypeId
    );

    public record OrderTotal(
        [property: JsonPropertyName("totalAmount")] decimal TotalAmount,
        [property: JsonPropertyName("currencyCode")] string CurrencyCode
    );

    public record Dictionaries(
        [property: JsonPropertyName("location")] Dictionary<string, Location> Location,
        [property: JsonPropertyName("country")] Dictionary<string, string> Country,
        [property: JsonPropertyName("airline")] Dictionary<string, string> Airline,
        [property: JsonPropertyName("aircraft")] Dictionary<string, string> Aircraft,
        [property: JsonPropertyName("flight")] Dictionary<string, Flight> Flight,
        [property: JsonPropertyName("tax")] Dictionary<string, string> Tax,
        [property: JsonPropertyName("currency")] Dictionary<string, Currency> Currency,
        [property: JsonPropertyName("specialServiceRequest")] Dictionary<string, SsrDictionary> SpecialServiceRequest,
        [property: JsonPropertyName("bookingStatus")] Dictionary<string, BookingStatus> BookingStatus,
        [property: JsonPropertyName("meal")] Dictionary<string, string> Meal,
        [property: JsonPropertyName("discount")] Dictionary<string, DiscountDictionary> Discount,
        [property: JsonPropertyName("promoCodeOfferCodeMapping")]
        List<PromoCodeOfferCodeMapping> PromoCodeOfferCodeMapping
    );

    public record Location(
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("airportName")] string AirportName,
        [property: JsonPropertyName("cityCode")] string CityCode,
        [property: JsonPropertyName("cityName")] string CityName,
        [property: JsonPropertyName("countryCode")] string CountryCode,
        [property: JsonPropertyName("timeZone")] string TimeZone
    );

    public record Flight(
        [property: JsonPropertyName("marketingAirlineCode")] string MarketingAirlineCode,
        [property: JsonPropertyName("operatingAirlineCode")] string OperatingAirlineCode,
        [property: JsonPropertyName("marketingFlightNumber")] string MarketingFlightNumber,
        [property: JsonPropertyName("operatingAirlineFlightNumber")] string OperatingAirlineFlightNumber,
        [property: JsonPropertyName("departure")] FlightEndpoint Departure,
        [property: JsonPropertyName("arrival")] FlightEndpoint Arrival,
        [property: JsonPropertyName("aircraftCode")] string AircraftCode,
        [property: JsonPropertyName("duration")] int Duration,
        [property: JsonPropertyName("isOpenSegment")] bool IsOpenSegment,
        [property: JsonPropertyName("isInformational")] bool IsInformational,
        [property: JsonPropertyName("secureFlightIndicator")] bool SecureFlightIndicator,
        [property: JsonPropertyName("meals")] MealInfo Meals,
        [property: JsonPropertyName("flightStatus")] string FlightStatus,
        [property: JsonPropertyName("aircraftConfigurationVersion")] string AircraftConfigurationVersion
    );

    public record FlightEndpoint(
        [property: JsonPropertyName("locationCode")] string LocationCode,
        [property: JsonPropertyName("dateTime")] DateTimeOffset DateTime,
        [property: JsonPropertyName("terminal")] string Terminal
    );

    public record MealInfo(
        [property: JsonPropertyName("bookingClass")] string BookingClass,
        [property: JsonPropertyName("mealCodes")] List<string> MealCodes
    );

    public record Currency(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("decimalPlaces")] int DecimalPlaces
    );

    public record SsrDictionary(
        [property: JsonPropertyName("name")] string Name
    );

    public record BookingStatus(
        [property: JsonPropertyName("name")] string Name
    );

    public record DiscountDictionary(
        [property: JsonPropertyName("airlineCode")] string AirlineCode,
        [property: JsonPropertyName("reasonCode")] string ReasonCode,
        [property: JsonPropertyName("isPromotion")] bool IsPromotion
    );

    public record PromoCodeOfferCodeMapping(
        [property: JsonPropertyName("offerRef")] string OfferRef,
        [property: JsonPropertyName("offerCode")] string OfferCode,
        [property: JsonPropertyName("promoCode")] string PromoCode
    );
