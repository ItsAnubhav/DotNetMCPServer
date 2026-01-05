using System.Text.Json.Serialization;

namespace McpServerApp.Services.Saudia.Responses
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record Acknowledge(
        [property: JsonPropertyName("airBoundId")] string airBoundId,
        [property: JsonPropertyName("disruption")] Disruption disruption,
        [property: JsonPropertyName("waitlistConfirmation")] WaitlistConfirmation waitlistConfirmation,
        [property: JsonPropertyName("seats")] Seats seats
    );

    public record Aircraft(
        [property: JsonPropertyName("320")] string _320,
        [property: JsonPropertyName("773")] string _773
    );

    public record Airline(
        [property: JsonPropertyName("SV")] string SV
    );

    public record Arrival(
        [property: JsonPropertyName("locationCode")] string locationCode,
        [property: JsonPropertyName("dateTime")] DateTime? dateTime,
        [property: JsonPropertyName("terminal")] string terminal
    );

    public record Base(
        [property: JsonPropertyName("value")] int? value,
        [property: JsonPropertyName("currencyCode")] string currencyCode
    );

    public record Cancel(
        [property: JsonPropertyName("isEligible")] bool? isEligible
    );

    public record CancelAndRefund(
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );

    public record Change(
        [property: JsonPropertyName("airBoundId")] string airBoundId,
        [property: JsonPropertyName("flightIds")] IReadOnlyList<string> flightIds,
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReason")] string nonEligibilityReason
    );

    public record Contact(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("category")] string category,
        [property: JsonPropertyName("contactType")] string contactType,
        [property: JsonPropertyName("deviceType")] string deviceType,
        [property: JsonPropertyName("purpose")] string purpose,
        [property: JsonPropertyName("countryPhoneExtension")] string countryPhoneExtension,
        [property: JsonPropertyName("number")] string number,
        [property: JsonPropertyName("freeFlowText")] string freeFlowText,
        [property: JsonPropertyName("address")] string address,
        [property: JsonPropertyName("travelerIds")] IReadOnlyList<string> travelerIds
    );

    public record Country(
        [property: JsonPropertyName("AE")] string AE,
        [property: JsonPropertyName("IN")] string IN,
        [property: JsonPropertyName("SA")] string SA
    );

    public record CreationPointOfSale(
        [property: JsonPropertyName("pointOfSaleId")] string pointOfSaleId,
        [property: JsonPropertyName("countryCode")] string countryCode
    );

    public record Currency(
        [property: JsonPropertyName("SAR")] SAR SAR
    );

    public record OrderData(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("numericId")] string numericId,
        [property: JsonPropertyName("creationPointOfSale")] CreationPointOfSale creationPointOfSale,
        [property: JsonPropertyName("creationDateTime")] DateTime? creationDateTime,
        [property: JsonPropertyName("lastModificationDateTime")] DateTime? lastModificationDateTime,
        [property: JsonPropertyName("expirationDateTime")] DateTime? expirationDateTime,
        [property: JsonPropertyName("paymentTimeLimit")] DateTime? paymentTimeLimit,
        [property: JsonPropertyName("issuanceTimeLimit")] DateTime? issuanceTimeLimit,
        [property: JsonPropertyName("isGroupBooking")] bool? isGroupBooking,
        [property: JsonPropertyName("air")] Air air,
        [property: JsonPropertyName("services")] IReadOnlyList<Service> services,
        [property: JsonPropertyName("travelers")] IReadOnlyList<Traveler> travelers,
        [property: JsonPropertyName("contacts")] IReadOnlyList<Contact> contacts,
        [property: JsonPropertyName("remarks")] IReadOnlyList<Remark> remarks,
        [property: JsonPropertyName("specialServiceRequests")] IReadOnlyList<SpecialServiceRequest> specialServiceRequests,
        [property: JsonPropertyName("orderEligibilities")] OrderEligibilities orderEligibilities,
        [property: JsonPropertyName("orderTotal")] OrderTotal orderTotal
    );


    public record Departure(
        [property: JsonPropertyName("locationCode")] string locationCode,
        [property: JsonPropertyName("dateTime")] DateTime? dateTime,
        [property: JsonPropertyName("terminal")] string terminal
    );

    public record Description(
        [property: JsonPropertyName("type")] string type,
        [property: JsonPropertyName("content")] string content
    );

    public record BaggageDetails(
        [property: JsonPropertyName("type")] string type,
        [property: JsonPropertyName("quantity")] int? quantity
    );

    public record Dictionaries(
        [property: JsonPropertyName("country")] Country country,
        [property: JsonPropertyName("airline")] Airline airline,
        [property: JsonPropertyName("aircraft")] Aircraft aircraft,
        [property: JsonPropertyName("flight")] Flight flight,
        [property: JsonPropertyName("currency")] Currency currency,
        [property: JsonPropertyName("specialServiceRequest")] SpecialServiceRequest specialServiceRequest
    );

    public record Disruption(
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );

    public record DOCS(
        [property: JsonPropertyName("name")] string name
    );

   
    public record NonEligibilityReason(
        [property: JsonPropertyName("code")] string code,
        [property: JsonPropertyName("title")] string title
    );

    public record OrderEligibilities(
        [property: JsonPropertyName("acknowledge")] IReadOnlyList<Acknowledge> acknowledge,
        [property: JsonPropertyName("cancel")] Cancel cancel,
        [property: JsonPropertyName("cancelAndRefund")] CancelAndRefund cancelAndRefund,
        [property: JsonPropertyName("change")] IReadOnlyList<Change> change,
        [property: JsonPropertyName("seatChange")] IReadOnlyList<SeatChange> seatChange,
        [property: JsonPropertyName("serviceChange")] IReadOnlyList<ServiceChange> serviceChange,
        [property: JsonPropertyName("isOnlinePnr")] bool? isOnlinePnr,
        [property: JsonPropertyName("typeOfPnr")] string typeOfPnr,
        [property: JsonPropertyName("pnrTypeId")] int? pnrTypeId,
        [property: JsonPropertyName("serviceOfficeId")] string serviceOfficeId,
        [property: JsonPropertyName("isTicketed")] bool? isTicketed,
        [property: JsonPropertyName("isRebookingAllowed")] bool? isRebookingAllowed,
        [property: JsonPropertyName("isAdditionOfAncillariesAllowed")] bool? isAdditionOfAncillariesAllowed,
        [property: JsonPropertyName("unpaidAncillaries")] IReadOnlyList<object> unpaidAncillaries,
        [property: JsonPropertyName("isUnpaidAncillaryPaymentAllowed")] bool? isUnpaidAncillaryPaymentAllowed,
        [property: JsonPropertyName("isPaidAncillaryInDifferentCurrencies")] bool? isPaidAncillaryInDifferentCurrencies,
        [property: JsonPropertyName("isApisAllowed")] bool? isApisAllowed,
        [property: JsonPropertyName("isFFUpdatedAllowed")] bool? isFFUpdatedAllowed,
        [property: JsonPropertyName("isSpecialAssistAndMealUpdateAllowed")] bool? isSpecialAssistAndMealUpdateAllowed,
        [property: JsonPropertyName("isAlreadyRebooked")] bool? isAlreadyRebooked,
        [property: JsonPropertyName("isOnholdPaymentAllowed")] bool? isOnholdPaymentAllowed,
        [property: JsonPropertyName("isCancelAndRefundAllowed")] bool? isCancelAndRefundAllowed,
        [property: JsonPropertyName("isCancelAndRefundFormAllowed")] bool? isCancelAndRefundFormAllowed,
        [property: JsonPropertyName("canBeRetrived")] bool? canBeRetrived,
        [property: JsonPropertyName("isSelfReaccommodationAllowed")] bool? isSelfReaccommodationAllowed,
        [property: JsonPropertyName("isCancelAndRefundIneligibleReason")] string isCancelAndRefundIneligibleReason,
        [property: JsonPropertyName("upgradeWithMiles")] bool? upgradeWithMiles,
        [property: JsonPropertyName("isInvoluntaryRefundEnabled")] bool? isInvoluntaryRefundEnabled,
        [property: JsonPropertyName("hasCashandMilesAncillaries")] bool? hasCashandMilesAncillaries,
        [property: JsonPropertyName("isCashandMilesPayment")] bool? isCashandMilesPayment,
        [property: JsonPropertyName("isUpgradedWithMiles")] bool? isUpgradedWithMiles,
        [property: JsonPropertyName("isFlightBookedInMCP")] bool? isFlightBookedInMCP,
        [property: JsonPropertyName("pnrCurrency")] string pnrCurrency,
        [property: JsonPropertyName("unsupportedcurrencyPNR")] bool? unsupportedcurrencyPNR,
        [property: JsonPropertyName("vOrderDataKey")] string vOrderDataKey
    );

    public record OrderTotal(
        [property: JsonPropertyName("totalAmount")] int? totalAmount,
        [property: JsonPropertyName("currencyCode")] string currencyCode
    );

    public record PassengerChange(
        [property: JsonPropertyName("travelerIds")] IReadOnlyList<string> travelerIds,
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );

    public record Remark(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("remarkType")] string remarkType,
        [property: JsonPropertyName("freetext")] string freetext
    );

    public record OrderResponse(
        [property: JsonPropertyName("data")] Data data,
        [property: JsonPropertyName("dictionaries")] Dictionaries dictionaries
    );

    public record RUH(
        [property: JsonPropertyName("type")] string type,
        [property: JsonPropertyName("airportName")] string airportName,
        [property: JsonPropertyName("cityCode")] string cityCode,
        [property: JsonPropertyName("cityName")] string cityName,
        [property: JsonPropertyName("countryCode")] string countryCode,
        [property: JsonPropertyName("timeZone")] string timeZone
    );

    public record SAR(
        [property: JsonPropertyName("name")] string name,
        [property: JsonPropertyName("decimalPlaces")] int? decimalPlaces
    );

    public record SeatChange(
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );

    public record Seats(
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );

    public record Service(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("descriptions")] IReadOnlyList<Description> descriptions,
        [property: JsonPropertyName("quantity")] int? quantity,
        [property: JsonPropertyName("tags")] IReadOnlyList<string> tags,
        [property: JsonPropertyName("flightIds")] IReadOnlyList<string> flightIds,
        [property: JsonPropertyName("statusCode")] string statusCode,
        [property: JsonPropertyName("travelerId")] string travelerId,
        [property: JsonPropertyName("isChargeable")] bool? isChargeable
    );

    public record ServiceChange(
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );

    public record SpecialServiceRequest(
        [property: JsonPropertyName("id")] string id,
        [property: JsonPropertyName("code")] string code,
        [property: JsonPropertyName("airlineCode")] string airlineCode,
        [property: JsonPropertyName("statusCode")] string statusCode,
        [property: JsonPropertyName("quantity")] int? quantity,
        [property: JsonPropertyName("freetext")] string freetext,
        [property: JsonPropertyName("travelerIds")] IReadOnlyList<string> travelerIds,
        [property: JsonPropertyName("flightIds")] IReadOnlyList<string> flightIds
    );



    public record Total(
        [property: JsonPropertyName("value")] int? value,
        [property: JsonPropertyName("currencyCode")] string currencyCode
    );

    public record TotalFees(
        [property: JsonPropertyName("value")] int? value,
        [property: JsonPropertyName("currencyCode")] string currencyCode
    );

    public record TotalTaxes(
        [property: JsonPropertyName("value")] int? value,
        [property: JsonPropertyName("currencyCode")] string currencyCode
    );

    public record TotalTaxesAndFees(
        [property: JsonPropertyName("value")] int? value,
        [property: JsonPropertyName("currencyCode")] string currencyCode
    );


    public record WaitlistConfirmation(
        [property: JsonPropertyName("isEligible")] bool? isEligible,
        [property: JsonPropertyName("nonEligibilityReasons")] IReadOnlyList<NonEligibilityReason> nonEligibilityReasons
    );


}
