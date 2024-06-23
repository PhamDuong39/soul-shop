// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Infrastructure;
using Shop.Module.Core.Cache;

namespace Shop.Module.Core.Controllers;

[ApiController]
[Route("api/caches")]
[Authorize(Roles = "admin")]
public class CacheApiController(IStaticCacheManager cache) : ControllerBase
{

    /// <summary>
    /// 清除所有缓存。
    /// </summary>
    /// <returns>表示操作结果的 <see cref="Result"/> 对象。</returns>
    [HttpDelete("clear")]
    public async Task<Result> Upload()
    {
        cache.Clear();
        await Task.CompletedTask;
        return Result.Ok();
    }
}
