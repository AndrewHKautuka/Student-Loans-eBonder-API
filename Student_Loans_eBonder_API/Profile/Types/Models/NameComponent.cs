using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Profile.Types.Models;

[PrimaryKey(nameof(NameId), nameof(PositionOrder))]
internal class NameComponent : ITimestampEntity
{
	public long NameId { get; set; }
	public required Name Name { get; set; }
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Range(1, int.MaxValue, ErrorMessage = "The position order must be at least 1.")]
	public short PositionOrder { get; set; }
	public NameComponentType Type { get; set; }
	public required string Value { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
