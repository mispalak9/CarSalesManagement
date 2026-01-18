namespace CarSalesManagementAPI.Application.Constants;

public static class ApplicationConstants
{
    public static class Brands
    {
        public const string Audi = "Audi";
        public const string Jaguar = "Jaguar";
        public const string LandRover = "Land Rover";
        public const string Renault = "Renault";

        public static readonly string[] ValidBrands = { Audi, Jaguar, LandRover, Renault };
    }

    public static class CarClasses
    {
        public const string AClass = "A-Class";
        public const string BClass = "B-Class";
        public const string CClass = "C-Class";

        public static readonly string[] ValidClasses = { AClass, BClass, CClass };
    }

    public static class Commission
    {
        public const decimal BonusEligibilityThreshold = 500000;
        public const decimal BonusPercentage = 0.02m; // 2%
        public const int ClassAId = 1; // Assuming A-Class has ID 1
    }

    public static class FileUpload
    {
        public const long MaxFileSizeBytes = 5242880; // 5MB
        public static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    }
}
