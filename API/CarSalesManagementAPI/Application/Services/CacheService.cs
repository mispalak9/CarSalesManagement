using CarSalesManagementAPI.Domain.Entities;
using CarSalesManagementAPI.Domain.Interfaces;

namespace CarSalesManagementAPI.Application.Services;

public interface ICacheService
{
    Task<IEnumerable<CarClass>> GetCarClasses();
    Task<IEnumerable<CommissionRule>> GetCommissionRules();
    Task<IEnumerable<Brand>> GetBrands();
    void ClearCache();
}

public class CacheService : ICacheService
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly ICarModelRepository _carModelRepository;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);
    
    private DateTime _lastRefresh = DateTime.MinValue;
    private IEnumerable<CarClass>? _cachedCarClasses;
    private IEnumerable<CommissionRule>? _cachedCommissionRules;
    private IEnumerable<Brand>? _cachedBrands;

    public CacheService(ICommissionRepository commissionRepository, ICarModelRepository carModelRepository)
    {
        _commissionRepository = commissionRepository;
        _carModelRepository = carModelRepository;
    }

    public async Task<IEnumerable<CarClass>> GetCarClasses()
    {
        if (_cachedCarClasses == null || DateTime.UtcNow - _lastRefresh > _cacheDuration)
        {
            await RefreshCache();
        }
        return _cachedCarClasses ?? new List<CarClass>();
    }

    public async Task<IEnumerable<CommissionRule>> GetCommissionRules()
    {
        if (_cachedCommissionRules == null || DateTime.UtcNow - _lastRefresh > _cacheDuration)
        {
            await RefreshCache();
        }
        return _cachedCommissionRules ?? new List<CommissionRule>();
    }

    public async Task<IEnumerable<Brand>> GetBrands()
    {
        if (_cachedBrands == null || DateTime.UtcNow - _lastRefresh > _cacheDuration)
        {
            await RefreshCache();
        }
        return _cachedBrands ?? new List<Brand>();
    }

    private async Task RefreshCache()
    {
        var carClassesTask = _commissionRepository.GetAllCarClasses();
        var commissionRulesTask = _commissionRepository.GetAllCommissionRules();
        var brandsTask = _carModelRepository.GetAllBrands();

        _cachedCarClasses = await carClassesTask;
        _cachedCommissionRules = await commissionRulesTask;
        _cachedBrands = await brandsTask;
        _lastRefresh = DateTime.UtcNow;
    }

    public void ClearCache()
    {
        _cachedCarClasses = null;
        _cachedCommissionRules = null;
        _cachedBrands = null;
        _lastRefresh = DateTime.MinValue;
    }
}
