// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Core.ViewModels;

public class LoginPhoneParam
{
    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone is valid")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "code is valid")]
    public string Code { get; set; }

    public bool RememberMe { get; set; }
}
