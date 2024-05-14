namespace Soul.Shop.Module.Minio.Abstractions.Options
{
    public class LogOption
    {
        public const string Position = "Logging";
        public LogLevel? LogLevel { get; set; }
        public File? File { get; set; }
        public Kafka? Kafka { get; set; }
    }

    public class LogLevel
    {
        public string? Default { get; set; }
        public string? Microsoft { get; set; }
        public string? SwitchLevelLog { get; set; }
    }

    public class File
    {
        public string? Path { get; set; }
    }
    
    public class Kafka
    {
        public string? BootstrapServers { get; set; }
        public string? Topic { get; set; }
    }
}
