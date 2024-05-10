using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class Category : EntityBase
{
    public Category()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    [Required] [StringLength(450)] public string Slug { get; set; }

    public string MetaTitle { get; set; } //tiêu đề

    public string MetaKeywords { get; set; } //từ khóa

    public string MetaDescription { get; set; } //mô tả ngắn

    public string Description { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IncludeInMenu { get; set; } //có hiển thị trong menu

    public int? ParentId { get; set; }

    public Category Parent { get; set; }

    public int? MediaId { get; set; }

    public Media Media { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    [JsonIgnore]
    public IList<Category> Children { get; protected set; } = new List<Category>(); //danh sách danh mục con
}