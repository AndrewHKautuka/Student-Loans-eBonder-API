using Microsoft.EntityFrameworkCore;

using Student_Loans_eBonder_API.Auth.Types.Models;
using Student_Loans_eBonder_API.Common.Types.Models;

namespace Student_Loans_eBonder_API.Student.Types.Models;

[Index(nameof(UserId), IsUnique = true)]
internal class Student : ITimestampEntity
{
	public Guid Id { get; set; }
	public required string UserId { get; set; }
	public required User User { get; set; }
	public string? NationalIdNumber { get; set; }
	public string? NationalIdScanUrl { get; set; }
	public DateOnly? DateOfBirth { get; set; }
	public Sex? Sex { get; set; }
	public string? PostalAddress { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
