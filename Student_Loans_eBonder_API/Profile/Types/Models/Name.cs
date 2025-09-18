using System.ComponentModel.DataAnnotations.Schema;

using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Profile.Types.Models;

internal class Name : ITimestampEntity
{
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public long Id { get; set; }
	public string? PreferredShortName { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public required ICollection<NameComponent> Components { get; set; }
}
