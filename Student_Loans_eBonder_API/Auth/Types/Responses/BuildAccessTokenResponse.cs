namespace Student_Loans_eBonder_API.Auth.Types.Responses;

public class BuildAccessTokenResponse
{
	public required string Token { get; set; }
	public required DateTime Expires { get; set; }
}
