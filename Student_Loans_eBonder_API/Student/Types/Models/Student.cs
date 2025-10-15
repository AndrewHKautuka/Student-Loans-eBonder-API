using Microsoft.EntityFrameworkCore;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Student.Types.Models;

[Index(nameof(UserId), IsUnique = true)]
public class Student : ITimestampEntity
{
	public Guid Id { get; set; }
	public string UserId { get; set; } = null!;
	public User User { get; set; } = null!;
	public string? NationalIdNumber { get; set; }
	public string? NationalIdScanUrl { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public Sex? Sex { get; set; }
	public string? PostalAddress { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
