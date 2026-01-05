namespace McpServerApp.Services.Saudia;

public static class SaudiaConstants{

    public const string host = "http://dapi-uat.dcloud.saudia.com";

    public const string ClientId = "ddb05723-34db-40d1-8236-31f7379ef537";
    public const string ClientSecret = "12345678"; // Replace with actual secret
    public const string Scope = "259a88f9-c8c4-4083-9a20-a80d018987d7/.default";
    public const string Fact = "{\"keyValuePairs\":[{\"key\":\"flow\",\"value\":\"REVENUE\"},{\"key\":\"market\",\"value\":\"IND\"},{\"key\":\"originCity\",\"value\":\"JED\"},{\"key\":\"channel\",\"value\":\"DESKTOP\"}]}";

    public const string guestOfficeId = "LONSV08AA";

    public const string AuthEndpoint = $"{host}/session/auth/b2b/token/initialization";
}