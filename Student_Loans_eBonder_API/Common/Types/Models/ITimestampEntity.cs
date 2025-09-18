using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student_Loans_eBonder_API.Common.Types.Models;

internal interface ITimestampEntity
{
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
