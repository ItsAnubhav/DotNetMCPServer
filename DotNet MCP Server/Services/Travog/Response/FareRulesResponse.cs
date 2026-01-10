using System.Text.Json.Serialization;

namespace DotNet_MCP_Server.Services.Travog.Response
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public record AirlineCharges(
        [property: JsonPropertyName("AdtCharge")] string AdtCharge,
        [property: JsonPropertyName("YthCharge")] string YthCharge,
        [property: JsonPropertyName("ChdCharge")] string ChdCharge,
        [property: JsonPropertyName("InfCharge")] string InfCharge
    );

    public record BaggageInfo(
        [property: JsonPropertyName("Segment")] IReadOnlyList<Segment> Segment
    );

    public record CancellationReply(
        [property: JsonPropertyName("XHostCharges")] XHostCharges XHostCharges,
        [property: JsonPropertyName("AirlineCharges")] AirlineCharges AirlineCharges,
        [property: JsonPropertyName("CanxRemarks")] object CanxRemarks,
        [property: JsonPropertyName("RefundType")] string RefundType,
        [property: JsonPropertyName("FareBasisCode")] string FareBasisCode,
        [property: JsonPropertyName("Airline")] string Airline,
        [property: JsonPropertyName("FareType")] string FareType,
        [property: JsonPropertyName("BookingClass")] string BookingClass
    );

    public record Curr(
        [property: JsonPropertyName("CurrC")] string CurrC,
        [property: JsonPropertyName("CurrN")] string CurrN
    );

    public record Data(
        [property: JsonPropertyName("FareRules")] FareRules FareRules
    );

    public record Description(
        [property: JsonPropertyName("Title")] string Title,
        [property: JsonPropertyName("text")] string text
    );

    public record FareRules(
        [property: JsonPropertyName("CancellationReply")] CancellationReply CancellationReply,
        [property: JsonPropertyName("FaresRulesReply")] FaresRulesReply FaresRulesReply,
        [property: JsonPropertyName("Curr")] Curr Curr,
        [property: JsonPropertyName("JournyType")] string JournyType,
        [property: JsonPropertyName("CompanyID")] string CompanyID,
        [property: JsonPropertyName("noa")] string noa,
        [property: JsonPropertyName("noc")] string noc,
        [property: JsonPropertyName("noy")] string noy,
        [property: JsonPropertyName("noi")] string noi
    );

    public record FaresRulesReply(
        [property: JsonPropertyName("RuleInfo")] RuleInfo RuleInfo,
        [property: JsonPropertyName("BaggageInfo")] BaggageInfo BaggageInfo
    );

    public record Pax(
        [property: JsonPropertyName("Cabin")] string Cabin,
        [property: JsonPropertyName("CheckIn")] string CheckIn,
        [property: JsonPropertyName("text")] string text
    );

    public record FareRulesDetail(
        [property: JsonPropertyName("success")] bool? success,
        [property: JsonPropertyName("message")] string message,
        [property: JsonPropertyName("data")] Data data
    );

    public record RuleInfo(
        [property: JsonPropertyName("Title")] string Title,
        [property: JsonPropertyName("Description")] IReadOnlyList<Description> Description
    );

    public record Segment(
        [property: JsonPropertyName("Name")] string Name,
        [property: JsonPropertyName("Pax")] Pax Pax
    );

    public record XHostCharges(
        [property: JsonPropertyName("AdtBeforeDepartureChargeAmount")] object AdtBeforeDepartureChargeAmount,
        [property: JsonPropertyName("AdtAfterDepartureChargeAmount")] object AdtAfterDepartureChargeAmount,
        [property: JsonPropertyName("AdtVoluntaryChangeChargeAmount")] object AdtVoluntaryChangeChargeAmount,
        [property: JsonPropertyName("AdtInvoluntaryChangeChargeAmount")] object AdtInvoluntaryChangeChargeAmount,
        [property: JsonPropertyName("AdtCanxCharge")] object AdtCanxCharge,
        [property: JsonPropertyName("AdtReIssueCharge")] object AdtReIssueCharge,
        [property: JsonPropertyName("AdtReroutingChargeAmount")] object AdtReroutingChargeAmount,
        [property: JsonPropertyName("AdtNoShowChargesChargeAmount")] object AdtNoShowChargesChargeAmount,
        [property: JsonPropertyName("YthBeforeDepartureChargeAmount")] object YthBeforeDepartureChargeAmount,
        [property: JsonPropertyName("YthAfterDepartureChargeAmount")] object YthAfterDepartureChargeAmount,
        [property: JsonPropertyName("YthVoluntaryChangeChargeAmount")] object YthVoluntaryChangeChargeAmount,
        [property: JsonPropertyName("YthInvoluntaryChangeChargeAmount")] object YthInvoluntaryChangeChargeAmount,
        [property: JsonPropertyName("YthCanxCharge")] object YthCanxCharge,
        [property: JsonPropertyName("YthReIssueCharge")] object YthReIssueCharge,
        [property: JsonPropertyName("YthReroutingChargeAmount")] object YthReroutingChargeAmount,
        [property: JsonPropertyName("YthNoShowChargesChargeAmount")] object YthNoShowChargesChargeAmount,
        [property: JsonPropertyName("ChdBeforeDepartureChargeAmount")] object ChdBeforeDepartureChargeAmount,
        [property: JsonPropertyName("ChdAfterDepartureChargeAmount")] object ChdAfterDepartureChargeAmount,
        [property: JsonPropertyName("ChdVoluntaryChangeChargeAmount")] object ChdVoluntaryChangeChargeAmount,
        [property: JsonPropertyName("ChdInvoluntaryChangeChargeAmount")] object ChdInvoluntaryChangeChargeAmount,
        [property: JsonPropertyName("ChdCanxCharge")] object ChdCanxCharge,
        [property: JsonPropertyName("ChdReIssueCharge")] object ChdReIssueCharge,
        [property: JsonPropertyName("ChdReroutingChargeAmount")] object ChdReroutingChargeAmount,
        [property: JsonPropertyName("ChdNoShowChargesChargeAmount")] object ChdNoShowChargesChargeAmount,
        [property: JsonPropertyName("InfBeforeDepartureChargeAmount")] object InfBeforeDepartureChargeAmount,
        [property: JsonPropertyName("InfAfterDepartureChargeAmount")] object InfAfterDepartureChargeAmount,
        [property: JsonPropertyName("InfVoluntaryChangeChargeAmount")] object InfVoluntaryChangeChargeAmount,
        [property: JsonPropertyName("InfInvoluntaryChangeChargeAmount")] object InfInvoluntaryChangeChargeAmount,
        [property: JsonPropertyName("InfCanxCharge")] object InfCanxCharge,
        [property: JsonPropertyName("InfReIssueCharge")] object InfReIssueCharge,
        [property: JsonPropertyName("InfReroutingChargeAmount")] object InfReroutingChargeAmount,
        [property: JsonPropertyName("InfNoShowChargesChargeAmount")] object InfNoShowChargesChargeAmount
    );


}
