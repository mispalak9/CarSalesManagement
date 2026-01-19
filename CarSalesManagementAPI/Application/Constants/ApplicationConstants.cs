namespace CarSalesManagementAPI.Application.Constants;

public static class ApplicationConstants
{
    public static class Commission
    {
        public const decimal BonusEligibilityThreshold = 500000;
        public const decimal BonusPercentage = 0.02m; // 2%
    }

    public static class FileUpload
    {
        public const long MaxFileSizeBytes = 5242880; // 5MB
        public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    }
}
