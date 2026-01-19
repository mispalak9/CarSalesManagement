using AutoMapper;
using CarSalesManagementAPI.Application.DTOs;
using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace CarSalesManagementAPI.Application.Services;

public class CarModelService : ICarModelService
{
    private readonly ICarModelRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public CarModelService(
        ICarModelRepository repository,
        IMapper mapper,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _repository = repository;
        _mapper = mapper;
        _environment = environment;
        _configuration = configuration;
    }

    public async Task<ApiResponse<IEnumerable<CarModelDto>>> GetAll(string? searchTerm = null, string? orderBy = null)
    {
        try
        {
            var models = await _repository.GetAll(searchTerm, orderBy);
            var modelIds = models.Select(m => m.ModelID);
            var allImages = await _repository.GetImagesByModelIds(modelIds);
            var imagesDict = allImages.GroupBy(img => img.ModelID)
                                   .ToDictionary(g => g.Key, g => g.ToList());

            var modelDtos = models.Select(model =>
            {
                var dto = _mapper.Map<CarModelDto>(model);
                dto.Images = _mapper.Map<List<CarModelImageDto>>(
                    imagesDict.GetValueOrDefault(model.ModelID, new List<CarModelImage>())
                );
                return dto;
            });

            return new ApiResponse<IEnumerable<CarModelDto>>
            {
                Success = true,
                Message = "Car models retrieved successfully.",
                Data = modelDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<CarModelDto>>
            {
                Success = false,
                Message = "Error retrieving car models.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<CarModelDto>> GetById(int id)
    {
        try
        {
            var model = await _repository.GetById(id);
            if (model == null)
            {
                return new ApiResponse<CarModelDto>
                {
                    Success = false,
                    Message = "Car model not found.",
                    Errors = new List<string> { $"Car model with ID {id} not found." }
                };
            }

            var dto = _mapper.Map<CarModelDto>(model);
            var images = await _repository.GetImagesByModelId(id);
            dto.Images = _mapper.Map<List<CarModelImageDto>>(images);

            return new ApiResponse<CarModelDto>
            {
                Success = true,
                Message = "Car model retrieved successfully.",
                Data = dto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarModelDto>
            {
                Success = false,
                Message = "Error retrieving car model.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<CarModelDto>> Create(CreateCarModelDto dto)
    {
        try
        {
            var existing = await _repository.GetByModelCode(dto.ModelCode.ToUpper());
            if (existing != null)
            {
                return new ApiResponse<CarModelDto>
                {
                    Success = false,
                    Message = "Model Code already exists.",
                    Errors = new List<string> { "A car model with this Model Code already exists." }
                };
            }

            var brand = await _repository.GetBrandById(dto.BrandID);
            if (brand == null)
            {
                return new ApiResponse<CarModelDto>
                {
                    Success = false,
                    Message = "Invalid Brand ID.",
                    Errors = new List<string> { "The specified brand does not exist." }
                };
            }

            var carClass = await _repository.GetClassById(dto.ClassID);
            if (carClass == null)
            {
                return new ApiResponse<CarModelDto>
                {
                    Success = false,
                    Message = "Invalid Class ID.",
                    Errors = new List<string> { "The specified class does not exist." }
                };
            }

            var model = _mapper.Map<CarModel>(dto);
            model.ModelCode = dto.ModelCode.ToUpper();
            model.CreatedBy = null;
            model.CreatedOn = DateTime.Now;

            var modelId = await _repository.Create(model);

            var createdModel = await _repository.GetById(modelId);
            var resultDto = _mapper.Map<CarModelDto>(createdModel);

            return new ApiResponse<CarModelDto>
            {
                Success = true,
                Message = "Car model created successfully.",
                Data = resultDto
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<CarModelDto>
            {
                Success = false,
                Message = "Error creating car model.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> Update(UpdateCarModelDto dto)
    {
        try
        {
            var existing = await _repository.GetById(dto.ModelID);
            if (existing == null)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "Car model not found.",
                    Errors = new List<string> { $"Car model with ID {dto.ModelID} not found." }
                };
            }

            if (existing.ModelCode != dto.ModelCode.ToUpper())
            {
                var codeExists = await _repository.GetByModelCode(dto.ModelCode.ToUpper());
                if (codeExists != null && codeExists.ModelID != dto.ModelID)
                {
                    return new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Model Code already exists.",
                        Errors = new List<string> { "A car model with this Model Code already exists." }
                    };
                }
            }

            var model = _mapper.Map<CarModel>(dto);
            model.ModelCode = dto.ModelCode.ToUpper();
            model.LastUpdatedBy = null; // Will be updated with proper authentication later
            model.LastUpdatedOn = DateTime.Now;

            var updated = await _repository.Update(model);

            return new ApiResponse<bool>
            {
                Success = updated,
                Message = updated ? "Car model updated successfully." : "Failed to update car model.",
                Data = updated
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error updating car model.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> Delete(int id)
    {
        try
        {
            var deleted = await _repository.Delete(id);
            return new ApiResponse<bool>
            {
                Success = deleted,
                Message = deleted ? "Car model deleted successfully." : "Car model not found.",
                Data = deleted
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error deleting car model.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<CarModelImageDto>>> GetImagesByModelId(int modelId)
    {
        try
        {
            var images = await _repository.GetImagesByModelId(modelId);
            var imageDtos = _mapper.Map<IEnumerable<CarModelImageDto>>(images);

            return new ApiResponse<IEnumerable<CarModelImageDto>>
            {
                Success = true,
                Message = "Images retrieved successfully.",
                Data = imageDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<CarModelImageDto>>
            {
                Success = false,
                Message = "Error retrieving images.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<string>> UploadImage(int modelId, IFormFile file)
    {
        try
        {
            var model = await _repository.GetById(modelId);
            if (model == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Car model not found.",
                    Errors = new List<string> { $"Car model with ID {modelId} not found." }
                };
            }

            // Validate file
            var maxSize = _configuration.GetValue<long>("FileUpload:MaxFileSize", 5242880); // 5MB default
            if (file.Length > maxSize)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "File size exceeds maximum allowed size.",
                    Errors = new List<string> { $"Maximum file size is {maxSize / 1024 / 1024}MB." }
                };
            }

            var allowedExtensions = _configuration.GetSection("FileUpload:AllowedExtensions").Get<string[]>() 
                ?? new[] { ".jpg", ".jpeg", ".png", ".gif" };
            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Invalid file extension.",
                    Errors = new List<string> { $"Allowed extensions: {string.Join(", ", allowedExtensions)}" }
                };
            }

            var uploadPath = _configuration["FileUpload:UploadPath"] ?? "wwwroot/uploads/carmodels";
            var fullUploadPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, uploadPath);
            if (!Directory.Exists(fullUploadPath))
            {
                Directory.CreateDirectory(fullUploadPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(fullUploadPath, fileName);
            var relativePath = Path.Combine(uploadPath, fileName).Replace('\\', '/');

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new CarModelImage
            {
                ModelID = modelId,
                ImagePath = relativePath,
                ImageName = file.FileName,
                ImageSize = file.Length,
                IsDefault = false,
                SortOrder = 0,
                CreatedBy = null,
                CreatedOn = DateTime.Now
            };

            var imageId = await _repository.AddImage(image);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Image uploaded successfully.",
                Data = relativePath
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Error uploading image.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> SetDefaultImage(int imageId, int modelId)
    {
        try
        {
            var result = await _repository.SetDefaultImage(imageId, modelId);
            return new ApiResponse<bool>
            {
                Success = result,
                Message = result ? "Default image set successfully." : "Failed to set default image.",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error setting default image.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteImage(int imageId)
    {
        try
        {
            var result = await _repository.DeleteImage(imageId);
            return new ApiResponse<bool>
            {
                Success = result,
                Message = result ? "Image deleted successfully." : "Failed to delete image.",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Error deleting image.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<BrandDto>>> GetAllBrands()
    {
        try
        {
            var brands = await _repository.GetAllBrands();
            var brandDtos = _mapper.Map<IEnumerable<BrandDto>>(brands);

            return new ApiResponse<IEnumerable<BrandDto>>
            {
                Success = true,
                Message = "Brands retrieved successfully.",
                Data = brandDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<BrandDto>>
            {
                Success = false,
                Message = "Error retrieving brands.",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<CarClassDto>>> GetAllClasses()
    {
        try
        {
            var classes = await _repository.GetAllClasses();
            var classDtos = _mapper.Map<IEnumerable<CarClassDto>>(classes);

            return new ApiResponse<IEnumerable<CarClassDto>>
            {
                Success = true,
                Message = "Classes retrieved successfully.",
                Data = classDtos
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<CarClassDto>>
            {
                Success = false,
                Message = "Error retrieving classes.",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
