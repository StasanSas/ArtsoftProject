namespace TaskService.Application.Arguments;

public record GetJobsArgument(string? startName, int? page, int? pageSize);