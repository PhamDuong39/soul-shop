namespace Soul.Shop.Module.Minio.Abstractions.Options
{
    public class MinIoConfigOption
    {
        public const string Position = "MinIoConfig";
        public string? AccessKey { get; set; }
        public string? EndPoint { get; set; }
        public string? SecretKey { get; set; }

        public bool Secure { get; set; }

        // bổ sung default path và default bucket
        public string? DefaultBucket { get; set; }

        public string? DefaultLocation { get; set; }
    }
}
