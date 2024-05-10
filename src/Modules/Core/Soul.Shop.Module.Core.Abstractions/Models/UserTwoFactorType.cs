using System.ComponentModel;

namespace Soul.Shop.Module.Core.Abstractions.Models;

public enum UserTwoFactorType
{
    [Description("Phone")] Phone = 0,
    [Description("Mail")] Email = 1
}