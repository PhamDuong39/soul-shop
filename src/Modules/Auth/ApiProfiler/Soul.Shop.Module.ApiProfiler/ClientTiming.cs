﻿using System.Runtime.Serialization;

namespace Soul.Shop.Module.ApiProfiler;

[DataContract]
public class ClientTiming
{
    [DataMember(Order = 1)] public string Name { get; set; }

    [DataMember(Order = 2)] public decimal Start { get; set; }

    [DataMember(Order = 3)] public decimal Duration { get; set; }

    public Guid Id { get; set; }

    public Guid MiniProfilerId { get; set; }
}