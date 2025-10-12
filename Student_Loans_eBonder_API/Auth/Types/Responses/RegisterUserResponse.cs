namespace Student_Loans_eBonder_API.Auth.Types.Responses;

public class RegisterUserResponse
{
	public required string Token { get; set; }
	public required DateTime Expires { get; set; }
}
