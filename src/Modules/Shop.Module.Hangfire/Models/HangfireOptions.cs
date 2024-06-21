namespace Shop.Module.Hangfire.Models;

public class HangfireOptions
{
    public bool RedisEnabled { get; set; }


    public string RedisConnection { get; set; }


    public string Username { get; set; }


    public string Password { get; set; }
}
