using Microsoft.AspNetCore.Mvc;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Application.Services;
using FluentValidation;

namespace CarSalesManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarModelsController : ControllerBase
{
    private readonly ICarModelService _carModelService;
    private readonly ILogger<CarModelsController> _logger;

    public CarModelsController(ICarModelService carModelService, ILogger<CarModelsController> logger)
    {
        _carModelService = carModelService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarModelDto>>>> GetAll(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? orderBy = null)
    {
        var response = await _carModelService.GetAll(searchTerm, orderBy);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CarModelDto>>> GetById(int id)
    {
        var response = await _carModelService.GetById(id);
        
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

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CarModelDto>>> Create([FromBody] CreateCarModelDto dto)
    {
        var response = await _carModelService.Create(dto);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return CreatedAtAction(nameof(GetById), new { id = response.Data?.ModelID }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(int id, [FromBody] UpdateCarModelDto dto)
    {
        if (id != dto.ModelID)
        {
            return BadRequest(new ApiResponse<bool>
            {
                Success = false,
                Message = "Model ID mismatch.",
                Errors = new List<string> { "The ID in the URL does not match the ID in the request body." }
            });
        }

        var response = await _carModelService.Update(dto);
        
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

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
    {
        var response = await _carModelService.Delete(id);
        
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

    [HttpGet("{modelId}/images")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarModelImageDto>>>> GetImages(int modelId)
    {
        var response = await _carModelService.GetImagesByModelId(modelId);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{modelId}/images")]
    public async Task<ActionResult<ApiResponse<string>>> UploadImage(int modelId, IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ApiResponse<string>
            {
                Success = false,
                Message = "No file provided.",
                Errors = new List<string> { "Please select a file to upload." }
            });
        }

        var response = await _carModelService.UploadImage(modelId, file);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{modelId}/images/{imageId}/set-default")]
    public async Task<ActionResult<ApiResponse<bool>>> SetDefaultImage(int modelId, int imageId)
    {
        var response = await _carModelService.SetDefaultImage(imageId, modelId);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{modelId}/images/{imageId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImage(int modelId, int imageId)
    {
        var response = await _carModelService.DeleteImage(imageId);
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("brands")]
    public async Task<ActionResult<ApiResponse<IEnumerable<BrandDto>>>> GetAllBrands()
    {
        var response = await _carModelService.GetAllBrands();
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("classes")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarClassDto>>>> GetAllClasses()
    {
        var response = await _carModelService.GetAllClasses();
        
        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
