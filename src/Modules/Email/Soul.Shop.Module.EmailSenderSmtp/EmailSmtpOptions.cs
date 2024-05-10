namespace Soul.Shop.Module.EmailSenderSmtp;

public class EmailSmtpOptions
{
    public string SmtpUserName { get; set; }

    public string SmtpPassword { get; set; }

    public string SmtpHost { get; set; } = "smtp.gmail.com";

    public int SmtpPort { get; set; } = 587;
}