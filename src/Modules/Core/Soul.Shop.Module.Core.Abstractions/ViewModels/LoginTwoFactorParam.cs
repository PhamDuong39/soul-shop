﻿namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class LoginTwoFactorParam
{
    public string SelectedProvider { get; set; }

    public string Code { get; set; }

    public bool RememberMe { get; set; }

    public bool RememberBrowser { get; set; }
}