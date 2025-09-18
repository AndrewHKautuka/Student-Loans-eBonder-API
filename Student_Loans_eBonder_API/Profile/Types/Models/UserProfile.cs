using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Profile.Types.Models;

internal class UserProfile : ITimestampEntity
{
	[Key]
	[ForeignKey(nameof(User))]
	public required string UserId { get; set; }
	public required User User { get; set; }
	public long NameId { get; set; }
	public required Name Name { get; set; }
	public string? ProfilePictureUrl { get; set; }
	public string? SignatureScanUrl { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
