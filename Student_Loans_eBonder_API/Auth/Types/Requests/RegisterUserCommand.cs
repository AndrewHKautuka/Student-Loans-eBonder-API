using System.ComponentModel.DataAnnotations;

namespace Student_Loans_eBonder_API.Auth.Types.Requests;

internal class RegisterUserCommand
{
	[EmailAddress]
	public required string Email { get; set; }
	public required string Password { get; set; }
}
