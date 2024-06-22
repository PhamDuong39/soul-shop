// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using StackExchange.Redis;
using System;
using Shop.Module.EmailSenderSmtp;

namespace Shop.Module.Core.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="options"></param>
        public RedisService( RedisOption options)
        {
            _database =  ConnectionMultiplexer.ConnectAsync(options.RedisCachingConnection).Result.GetDatabase();
        }

        /// <summary>
        /// Save key value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        public void Set(string key, string value, TimeSpan? expiry = null)
        {
            if (expiry.HasValue)
            {
                _database.StringSet(key, value, expiry.Value);
            }
            else
            {
                _database.StringSet(key, value);
            }
        }

        public string Get(string key)
        {
            return _database.StringGet(key);
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }
    }

    public interface IRedisService
    {
        // Save key value
        void Set(string key, string value, TimeSpan? expiry = null);

        // Get value by key
        string Get(string key);

        // Remove key
        void Remove(string key);
    }
}
