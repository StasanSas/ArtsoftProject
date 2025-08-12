using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application.Arguments;
using TaskService.Application.Interfaces.Services;
using TaskService.Domain;
using TaskService.WebApi.Dto;
using TaskService.WebApi.Mapping;

namespace TaskService.WebApi.Controllers;

[ApiController]
[Route("api/task")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;
    
    private readonly IJobHistoryService _jobHistoryService;

    public JobController(IJobService jobService, IJobHistoryService jobHistoryService)
    {
        _jobService = jobService;
        _jobHistoryService = jobHistoryService;
    }
    
    /// <summary>
    /// Получить задачи с пагинацией и фильрацией
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkflowDto>), 200)]
    public IActionResult GetAll(
        [FromQuery] string? startName,
        [FromQuery] int? page = 1,
        [FromQuery] int? pageSize = 20)
    {
        var filter = new GetJobsArgument(startName, page, pageSize);
        var workFlows = _jobService.GetAllJobs(filter);
        return Ok(workFlows.Select(j => j.ToWorkflowDto()));
    }

    /// <summary>
    /// Получить задачу по ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkflowDto), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetById(Guid id)
    {
        return Ok(_jobService.GetJob(id).ToWorkflowDto());
    }

    /// <summary>
    /// Создать новую задачу
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), 201)]
    [ProducesResponseType(400)]
    public IActionResult Create([FromBody] JobDto jobDto)
    {
        var job = new NewJob(jobDto.Name, jobDto.Description);
        var idJob = _jobService.CreateJob(job);
        return CreatedAtAction(nameof(GetById), new { id = idJob});
    }

    /// <summary>
    /// Обновить существующую задачу
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public IActionResult Update(Guid id, [FromBody] JobDto jobDto)
    {
        var job = new Job(id, jobDto.Name, jobDto.Description);
        _jobService.UpdateJob(job);
        return NoContent();
    }

    /// <summary>
    /// Мягкое удаление задачи
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public IActionResult Delete(Guid id)
    {
        _jobService.DeleteJob(id);
        return NoContent();
    }

    /// <summary>
    /// Назначить исполнителя задачи
    /// </summary>
    [HttpPut("{id}/assign")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    public IActionResult Assign(Guid jobId, [FromBody][Required] Guid executorId)
    {
        _jobService.AssignExecutor(jobId, executorId);
        return NoContent();
    }
    
    /// <summary>
    /// Возвращает историю изменения работы
    /// </summary>
    [HttpGet("{id}/history")]
    [ProducesResponseType(typeof(IEnumerable<JobHistoryEventDto>), 200)]
    public IActionResult GetHistory(Guid jobId)
    {
        var jobHistoryEvents = _jobHistoryService.GetHistoryJobEvent(jobId);
        return Ok(jobHistoryEvents.Select(e => e.ToJobHistoryEventDto()));
    }
}