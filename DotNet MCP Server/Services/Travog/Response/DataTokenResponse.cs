namespace McpServerApp.Services.Travog.Response;

public class DataTokenResponse{
    public string companyId { get; set; }
    public string identityToken { get; set; }
    public string scopeToken { get; set; }
    public string refreshToken { get; set; }
    public DateTime accessTokenExpiresIn { get; set; }
    public DateTime refreshTokenExpiresIn { get; set; }
}

public class RootDataTokenResponse{
    public bool success { get; set; }
    public string message { get; set; }
    public DataTokenResponse data { get; set; }
}
