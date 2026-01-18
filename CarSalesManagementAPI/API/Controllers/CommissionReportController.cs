using Microsoft.AspNetCore.Mvc;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Application.Services;

namespace CarSalesManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommissionReportController : ControllerBase
{
    private readonly ICommissionService _commissionService;
    private readonly ILogger<CommissionReportController> _logger;

    public CommissionReportController(ICommissionService commissionService, ILogger<CommissionReportController> logger)
    {
        _commissionService = commissionService;
        _logger = logger;
    }

    [HttpGet("salesman/{salesmanId}")]
    public async Task<ActionResult<ApiResponse<CommissionReportDto>>> GetSalesmanReport(
        int salesmanId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest(new ApiResponse<CommissionReportDto>
            {
                Success = false,
                Message = "Invalid month.",
                Errors = new List<string> { "Month must be between 1 and 12." }
            });
        }

        if (year < 2000 || year > DateTime.Now.Year)
        {
            return BadRequest(new ApiResponse<CommissionReportDto>
            {
                Success = false,
                Message = "Invalid year.",
                Errors = new List<string> { $"Year must be between 2000 and {DateTime.Now.Year}." }
            });
        }

        var response = await _commissionService.GenerateCommissionReportAsync(salesmanId, month, year);
        
        if (!response.Success)
        {
            if (response.Errors.Any(e => e.Contains("not found")))
            {
                return NotFound(response);
            }
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("all-salesmen")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CommissionReportDto>>>> GetAllSalesmenReport(
        [FromQuery] int month,
        [FromQuery] int year)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest(new ApiResponse<IEnumerable<CommissionReportDto>>
            {
                Success = false,
                Message = "Invalid month.",
                Errors = new List<string> { "Month must be between 1 and 12." }
            });
        }

        if (year < 2000 || year > DateTime.Now.Year)
        {
            return BadRequest(new ApiResponse<IEnumerable<CommissionReportDto>>
            {
                Success = false,
                Message = "Invalid year.",
                Errors = new List<string> { $"Year must be between 2000 and {DateTime.Now.Year}." }
            });
        }

        var response = await _commissionService.GenerateAllSalesmenCommissionReportAsync(month, year);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
