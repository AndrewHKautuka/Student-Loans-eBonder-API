using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Profile.Types.Models;

public class UserProfile : ITimestampEntity
{
	[Key]
	[ForeignKey(nameof(User))]
	public string UserId { get; set; } = null!;
	public User User { get; set; } = null!;
	public long NameId { get; set; }
	public Name Name { get; set; }
	public string? ProfilePictureUrl { get; set; }
	public string? SignatureScanUrl { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
