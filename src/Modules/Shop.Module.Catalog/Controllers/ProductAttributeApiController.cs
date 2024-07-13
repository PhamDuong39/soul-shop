using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Web.StandardTable;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// Product attribute API controller, responsible for product attribute management operations, such as query, create, update and delete.
/// </summary>
[Authorize(Roles = "admin")]
[Route("api/product-attributes")]
public class ProductAttributeApiController : ControllerBase
{
    private readonly IRepository<ProductAttribute> _productAttrRepository;
    private readonly IRepository<ProductAttributeData> _productAttrDataRepository;
    private readonly IRepository<ProductAttributeValue> _productAttrValueRepository;
    private readonly IRepository<ProductAttributeTemplateRelation> _productAttrTempRelaRepo;

    public ProductAttributeApiController(
        IRepository<ProductAttribute> productAttrRepository,
        IRepository<ProductAttributeData> productAttrDataRepository,
        IRepository<ProductAttributeValue> productAttrValueRepository,
        IRepository<ProductAttributeTemplateRelation> productAttrTempRelaRepo)
    {
        _productAttrRepository = productAttrRepository;
        _productAttrDataRepository = productAttrDataRepository;
        _productAttrValueRepository = productAttrValueRepository;
        _productAttrTempRelaRepo = productAttrTempRelaRepo;
    }

    /// <summary>
    /// Get a list of all product attributes.
    /// </summary>
    /// <returns> Return a list of product attributes. </returns>
    [HttpGet]
    public async Task<Result<List<ProductAttributeResult>>> List()
    {
        var attributes = await _productAttrRepository
            .Query()
            .Where(c => !c.IsDeleted)
            .Select(x => new ProductAttributeResult
            {
                Id = x.Id,
                Name = x.Name,
                GroupName = x.Group.Name,
                GroupId = x.GroupId
            }).ToListAsync();
        return Result.Ok(attributes);
    }

    /// <summary>
    /// Group by attribute group and get the product attribute array.
    /// </summary>
    /// <returns>Return the grouped product attribute list. </returns>
    [HttpGet("group-array")]
    public async Task<Result<List<ProductAttributeGroupArrayResult>>> GroupArray()
    {
        var attributes = await _productAttrRepository
            .Query()
            .Where(c => !c.IsDeleted)
            .Select(x => new ProductAttributeResult
            {
                Id = x.Id,
                Name = x.Name,
                GroupName = x.Group.Name,
                GroupId = x.GroupId
            }).ToListAsync();
        var result = attributes.GroupBy(c => c.GroupId).Select(c => new ProductAttributeGroupArrayResult
        {
            GroupId = c.Key,
            GroupName = attributes.FirstOrDefault(x => x.GroupId == c.Key)?.GroupName,
            ProductAttributes = attributes.Where(x => x.GroupId == c.Key).OrderBy(x => x.Name).ToList()
        }).OrderBy(c => c.GroupName).ToList();
        return Result.Ok(result);
    }

    /// <summary>
    /// Get the product attribute list by page, support advanced functions such as sorting.
    /// </summary>
    /// <param name="param">Object containing paging and sorting parameters. </param>
    /// <returns>Return the paginated product attribute list. </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<ProductAttributeResult>>> DataList([FromBody] StandardTableParam param)
    {
        var query = _productAttrRepository.Query();
        var result = await query.Include(c => c.Group)
            .ToStandardTableResult(param, x => new ProductAttributeResult
            {
                Id = x.Id,
                Name = x.Name,
                GroupName = x.Group.Name,
                GroupId = x.GroupId
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Get detailed information of the specified product attribute according to the product attribute ID.
    /// </summary>
    /// <param name="id">Product attribute ID. </param>
    /// <returns>Return detailed information of the specified product attribute. </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var productAttribute = await _productAttrRepository.Query()
            .Include(c => c.Group)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (productAttribute == null) return Result.Fail("The document does not exist");
        var model = new ProductAttributeResult
        {
            Id = productAttribute.Id,
            Name = productAttribute.Name,
            GroupId = productAttribute.GroupId,
            GroupName = productAttribute.Group?.Name
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Add new product attributes.
    /// </summary>
    /// <param name="model">Parameter object containing product attribute information. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] ProductAttributeParam model)
    {
        var productAttribute = new ProductAttribute
        {
            Name = model.Name,
            GroupId = model.GroupId
        };
        _productAttrRepository.Add(productAttribute);
        await _productAttrRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product attribute information of the specified ID.
    /// </summary>
    /// <param name="id">Product attribute ID. </param>
    /// <param name="model">Parameter object containing product attribute update information. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] ProductAttributeParam model)
    {
        var productAttribute = await _productAttrRepository.FirstOrDefaultAsync(id);
        if (productAttribute == null) return Result.Fail("The document does not exist");
        productAttribute.Name = model.Name;
        productAttribute.GroupId = model.GroupId;
        productAttribute.UpdatedOn = DateTime.Now;
        await _productAttrRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the product attribute of the specified ID.
    /// </summary>
    /// <param name="id">Product attribute ID. </param>
    /// <returns>Return the operation result. </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var productAttribute = await _productAttrRepository.FirstOrDefaultAsync(id);
        if (productAttribute == null) return Result.Fail("The document does not exist");

        var any = _productAttrDataRepository.Query().Any(c => c.AttributeId == id);
        if (any) return Result.Fail("Please make sure that the property is not referenced by value data");

        any = _productAttrValueRepository.Query().Any(c => c.AttributeId == id);
        if (any) return Result.Fail("Please make sure that the attribute is not referenced by a product");

        any = _productAttrTempRelaRepo.Query().Any(c => c.AttributeId == id);
        if (any) return Result.Fail("Please make sure the attribute is not referenced by the product template");

        productAttribute.IsDeleted = true;
        productAttribute.UpdatedOn = DateTime.Now;
        await _productAttrRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Get all values ​​of the attribute according to the product attribute ID.
    /// </summary>
    /// <param name="attributeId">Product attribute ID. </param>
    /// <returns>Returns a list of product attribute values. </returns>
    [HttpGet("data/{attributeId:int:min(1)}")]
    public async Task<Result<List<ProductAttributeDataQueryResult>>> DataList(int attributeId)
    {
        var query = _productAttrDataRepository.Query(c => c.AttributeId == attributeId);
        var list = await query.Include(c => c.Attribute).ToListAsync();
        var result = list.Select(c => new ProductAttributeDataQueryResult
        {
            Id = c.Id,
            Value = c.Value,
            Description = c.Description,
            AttributeId = c.AttributeId,
            AttributeName = c.Attribute.Name,
            CreatedOn = c.CreatedOn,
            UpdatedOn = c.UpdatedOn,
            IsPublished = c.IsPublished
        }).ToList();
        return Result.Ok(result);
    }

    /// <summary>
    /// Get the product attribute value list in pages, support advanced functions such as sorting.
    /// </summary>
    /// <param name="attributeId">Product attribute ID. </param>
    /// <param name="param">Object containing paging and sorting parameters. </param>
    /// <returns>Return the paging product attribute value list. </returns>
    [HttpPost("data/{attributeId:int:min(1)}/grid")]
    public async Task<Result<StandardTableResult<ProductAttributeDataQueryResult>>> DataList(int attributeId,
        [FromBody] StandardTableParam<ValueParam> param)
    {
        var query = _productAttrDataRepository.Query(c => c.AttributeId == attributeId);
        if (param.Search != null)
        {
            var value = param.Search.Value;
            if (!string.IsNullOrWhiteSpace(value)) query = query.Where(x => x.Value.Contains(value.Trim()));
        }

        var result = await query.Include(c => c.Attribute)
            .ToStandardTableResult(param, c => new ProductAttributeDataQueryResult
            {
                Id = c.Id,
                Value = c.Value,
                Description = c.Description,
                AttributeId = c.AttributeId,
                AttributeName = c.Attribute.Name,
                CreatedOn = c.CreatedOn,
                UpdatedOn = c.UpdatedOn,
                IsPublished = c.IsPublished
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Add a new attribute value to the specified product attribute.
    /// </summary>
    /// <param name="attributeId">Product attribute ID. </param>
    /// <param name="model">Parameter object containing product attribute value information. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPost("data/{attributeId:int:min(1)}")]
    public async Task<Result> AddData(int attributeId, [FromBody] ProductAttributeDataParam model)
    {
        var data = new ProductAttributeData
        {
            AttributeId = attributeId,
            IsPublished = model.IsPublished,
            Value = model.Value,
            Description = model.Description
        };
        _productAttrDataRepository.Add(data);
        await _productAttrDataRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product attribute value information of the specified ID.
    /// </summary>
    /// <param name="id">Product attribute value ID. </param>
    /// <param name="model">Parameter object containing product attribute value update information. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPut("data/{id:int:min(1)}")]
    public async Task<Result> EditData(int id, [FromBody] ProductAttributeDataParam model)
    {
        var data = await _productAttrDataRepository.FirstOrDefaultAsync(id);
        if (data == null) return Result.Fail("The document does not exist");
        data.IsPublished = model.IsPublished;
        data.Value = model.Value;
        data.Description = model.Description;
        data.UpdatedOn = DateTime.Now;
        await _productAttrDataRepository.SaveChangesAsync();
        return Result.Ok();
    }


    /// <summary>
    /// Delete the product attribute value of the specified ID.
    /// </summary>
    /// <param name="id">Product attribute value ID. </param>
    /// <returns>Return the operation result. </returns>
    [HttpDelete("data/{id:int:min(1)}")]
    public async Task<Result> DeleteData(int id)
    {
        var data = await _productAttrDataRepository.FirstOrDefaultAsync(id);
        if (data == null) return Result.Fail("The document does not exist");
        data.IsDeleted = true;
        data.UpdatedOn = DateTime.Now;
        await _productAttrDataRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
