// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Shop.Module.Core.Services;

public interface IEmailSender
{
    public Task<bool> SendEmailAsync(string email, string subject, string message, bool isHtml = false);
}
