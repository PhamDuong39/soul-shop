﻿using Soul.Shop.Module.Core.Abstractions.Models;
using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Abstractions.Entities;

public class Media : EntityBase
{
    public Media()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [StringLength(450)] public string Caption { get; set; }

    public int FileSize { get; set; }

    [Required] [StringLength(450)] public string FileName { get; set; }

    public string Url { get; set; }

    public string? Path { get; set; }

    [StringLength(450)] public string Host { get; set; }

    [StringLength(450)] public string Hash { get; set; }

    [StringLength(32)] public string Md5 { get; set; }

    public MediaType MediaType { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}