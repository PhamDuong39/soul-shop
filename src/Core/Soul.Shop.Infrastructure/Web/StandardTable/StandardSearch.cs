﻿using Newtonsoft.Json.Linq;

namespace Soul.Shop.Infrastructure.Web.StandardTable;

public class StandardSearch
{
    public JObject PredicateObject { get; set; }
}

public class StandardSearch<T>
{
    public T PredicateObject { get; set; }
}