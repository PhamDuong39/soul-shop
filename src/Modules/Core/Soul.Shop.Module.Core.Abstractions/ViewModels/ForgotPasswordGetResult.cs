using System;
using System.Collections.Generic;
using System.Text;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class ForgotPasswordGetResult
{
    public string UserName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}