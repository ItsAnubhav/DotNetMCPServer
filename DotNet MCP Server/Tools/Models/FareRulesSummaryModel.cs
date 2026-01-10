using DotNet_MCP_Server.Services.Travog.Response;

namespace DotNet_MCP_Server.Tools.Models
{
    public class FareRulesSummary
    {
        // Core fare identity
        public string airline { get; set; }
        public string fare_basis { get; set; }
        public string booking_class { get; set; }
        public string fare_type { get; set; }
        public string refund_type { get; set; }

        // Charges (adult-focused for efficiency)
        public decimal? change_fee { get; set; }
        public decimal? cancellation_fee { get; set; }
        public decimal? no_show_fee { get; set; }

        // Baggage
        public string cabin_baggage { get; set; }
        public string checkin_baggage { get; set; }

        // Human-readable rules
        public string penalties_text { get; set; }

        // 👇 Factory
        public static FareRulesSummary FromDetails(FareRulesDetail details)
        {
            
            var canx = details.data.FareRules.CancellationReply;
            var rules = details.data.FareRules.FaresRulesReply;

            var penaltyRule = rules.RuleInfo.Description
                .FirstOrDefault(x => x.Title == "PENALTIES")?.text;

            var baggage = rules.BaggageInfo.Segment?.FirstOrDefault()?.Pax;

            return new FareRulesSummary
            {
                airline = canx.Airline,
                fare_basis = canx.FareBasisCode,
                booking_class = canx.BookingClass,
                fare_type = canx.FareType,
                refund_type = canx.RefundType,

                change_fee = ParseDecimal(canx.AirlineCharges.AdtCharge),
                cancellation_fee = ExtractUsdAmount(penaltyRule, "CANCEL"),
                no_show_fee = ExtractUsdAmount(penaltyRule, "NO-SHOW"),

                cabin_baggage = baggage?.Cabin,
                checkin_baggage = baggage?.CheckIn,

                penalties_text = penaltyRule
            };
        }

        private static decimal? ParseDecimal(string value)
            => decimal.TryParse(value, out var d) ? d : null;

        private static decimal? ExtractUsdAmount(string text, string keyword)
        {
            if (string.IsNullOrEmpty(text)) return null;

            var match = System.Text.RegularExpressions.Regex.Match(
                text,
                $"{keyword}.*?USD\\s*(\\d+)",
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );

            return match.Success ? decimal.Parse(match.Groups[1].Value) : null;
        }
    }

}
