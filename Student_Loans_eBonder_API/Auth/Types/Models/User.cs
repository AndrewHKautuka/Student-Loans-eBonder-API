using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Auth.Types.Models;

internal class User : IdentityUser, ITimestampEntity
{
	public DateTime? EmailConfirmedAt { get; set; }
	public DateTime? PasswordChangedAt { get; set; }
	public DateTime? PhoneNumberConfirmedAt { get; set; }
	[Required]
	public DateTime CreatedAt { get; set; }
	[Required]
	public DateTime UpdatedAt { get; set; }
	public DateTime? DeletedAt { get; set; }

}
