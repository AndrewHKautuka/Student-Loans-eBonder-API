using Mapster;

using Microsoft.EntityFrameworkCore;

using Student_Loans_eBonder_API.Student.Types.Commands;

namespace Student_Loans_eBonder_API.Student.Services;

public partial class StudentService(ILogger<StudentService> logger, ApplicationDBContext dBContext)
{
	public async Task<bool> CreateStudent(CreateStudentCommand command)
	{
		LogCheckStudentExistanceMessage(logger);

		var existingStudent = await dBContext.Student.FirstOrDefaultAsync(x => x.UserId == command.UserId);

		if (existingStudent is not null)
		{
			LogStudentAlreadyExistMessage(logger);
			return false;
		}

		LogStudentDoesNotExistMessage(logger);

		LogCreateNewStudentMessage(logger);
		var student = command.Adapt<Student.Types.Models.Student>();

		LogAddNewStudentMessage(logger);
		await dBContext.Student.AddAsync(student);
		await dBContext.SaveChangesAsync();

		return true;
	}

	[LoggerMessage(Level = LogLevel.Information, Message = "Checking if user already has a student record")]
	static partial void LogCheckStudentExistanceMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "User already has a student record, no new student record to be created")]
	static partial void LogStudentAlreadyExistMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "User does not have a student record, a new student record shall be created")]
	static partial void LogStudentDoesNotExistMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "Creating new student record")]
	static partial void LogCreateNewStudentMessage(ILogger logger);

	[LoggerMessage(Level = LogLevel.Information, Message = "Adding new student record")]
	static partial void LogAddNewStudentMessage(ILogger logger);
}
