using Microsoft.Extensions.Logging;

using Moq;

using Student_Loans_eBonder_API.Student.Services;
using Student_Loans_eBonder_API.Student.Types.Commands;
using Student_Loans_eBonder_API.Tests.Common;

namespace Student_Loans_eBonder_API.Tests.Student;

public class StudentServiceTests : DatabaseTestBase
{
	private Mock<ILogger<StudentService>> _mockLogger = null!;
	private StudentService _studentService = null!;

	public override async Task InitializeAsync()
	{
		await base.InitializeAsync();

		_mockLogger = MockLogger<StudentService>();
		_studentService = new StudentService(_mockLogger.Object, TestDbContext);
	}

	[Fact]
	public async Task StudentService_WhenCreateStudentCalledAndStudentDoesNotExist_ShouldCreateAndPersist()
	{
		// Arrange
		var userId = "user-without-student";
		var command = new CreateStudentCommand { UserId = userId };

		// Act
		var created = await _studentService.CreateStudent(command);

		// Assert
		Assert.True(created);
		var student = TestDbContext.Student.SingleOrDefault(s => s.UserId == userId);
		Assert.NotNull(student);
		Assert.IsType<Student_Loans_eBonder_API.Student.Types.Models.Student>(student);
	}

	[Fact]
	public async Task StudentService_WhenCreateStudentCalledAndStudentAlreadyExists_ShouldReturnFalseAndNotDuplicate()
	{
		// Arrange
		var userId = "user-with-student";
		TestDbContext.Student.Add(new Student_Loans_eBonder_API.Student.Types.Models.Student { UserId = userId });
		await SaveChangesAsync();

		var command = new CreateStudentCommand { UserId = userId };

		// Act
		var created = await _studentService.CreateStudent(command);

		// Assert
		Assert.False(created);
		var count = TestDbContext.Student.Count(s => s.UserId == userId);
		Assert.Equal(1, count);
	}
}


