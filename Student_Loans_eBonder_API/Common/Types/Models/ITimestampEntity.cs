namespace Student_Loans_eBonder_API.Common.Types.Models;

public interface ITimestampEntity
{
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
