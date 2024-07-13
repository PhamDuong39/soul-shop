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
/// The API controller of the product attribute template is responsible for managing the related operations of the product attribute template.
/// </summary>
[Authorize(Roles = "admin")]
[Route("/api/product-attribute-templates")]
public class ProductAttributeTemplateApiController : ControllerBase
{
    private readonly IRepository<ProductAttributeTemplate> _productAttrTempRepo;
    private readonly IRepository<ProductAttribute> _productAttrRepo;
    private readonly IRepository<ProductAttributeTemplateRelation> _productAttrTempRelaRepo;

    public ProductAttributeTemplateApiController(
        IRepository<ProductAttributeTemplate> productAttrTemplate,
        IRepository<ProductAttribute> productAttrRepository,
        IRepository<ProductAttributeTemplateRelation> productAttrTempRelaRepo)
    {
        _productAttrTempRepo = productAttrTemplate;
        _productAttrRepo = productAttrRepository;
        _productAttrTempRelaRepo = productAttrTempRelaRepo;
    }

    /// <summary>
    /// Get a list of all product attribute templates.
    /// </summary>
    /// <returns> Returns a list of product attribute templates. </returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var result = await _productAttrTempRepo.Query().Select(c => new
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
        return Result.Ok(result);
    }

    /// <summary>
    /// Get the paginated list of product attribute templates according to the given parameters.
    /// </summary>
    /// <param name="param">Pagination and filtering parameters. </param>
    /// <returns>Return the paginated results of product attribute templates that meet the conditions. </returns>
    [HttpPost("grid")]
    public async Task<Result<StandardTableResult<ProductAttributeTemplateResult>>> DataList(
        [FromBody] StandardTableParam param)
    {
        var query = _productAttrTempRepo.Query()
            .Include(c => c.ProductAttributes)
            .ThenInclude(c => c.Attribute)
            .ThenInclude(c => c.Group);
        var result = await query
            .ToStandardTableResult(param, x => new ProductAttributeTemplateResult
            {
                Id = x.Id,
                Name = x.Name,
                Attributes = x.ProductAttributes.Select(c => new ProductAttributeResult
                {
                    Id = c.AttributeId,
                    Name = c.Attribute.Name,
                    GroupId = c.Attribute.GroupId,
                    GroupName = c.Attribute.Group.Name
                }).ToList()
            });
        return Result.Ok(result);
    }

    /// <summary>
    /// Get a product attribute template based on the specified ID.
    /// </summary>
    /// <param name="id">The ID of the product attribute template. </param>
    /// <returns>Return the product attribute template with the specified ID. </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var first = await _productAttrTempRepo.Query()
            .Include(c => c.ProductAttributes)
            .ThenInclude(c => c.Attribute)
            .ThenInclude(c => c.Group)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (first == null)
            return Result.Fail("The document does not exist");
        var model = new ProductAttributeTemplateResult
        {
            Id = first.Id,
            Name = first.Name,
            Attributes = first.ProductAttributes.Select(c => new ProductAttributeResult
            {
                Id = c.AttributeId,
                Name = c.Attribute.Name,
                GroupId = c.Attribute.GroupId,
                GroupName = c.Attribute.Group.Name
            }).ToList()
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Add a new product attribute template.
    /// </summary>
    /// <param name="model">Parameter object containing product attribute template information. </param>
    /// <returns>Return the result of the add operation. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] ProductAttributeTemplateParam model)
    {
        var template = new ProductAttributeTemplate
        {
            Name = model.Name
        };
        var attributeIds = model.AttributeIds.Distinct();
        if (attributeIds.Count() > 0)
        {
            var attrIds = await _productAttrRepo
                .Query(c => attributeIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();
            foreach (var attrId in attrIds) template.AddAttribute(attrId);
        }

        _productAttrTempRepo.Add(template);
        await _productAttrTempRepo.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product attribute template of the specified ID.
    /// </summary>
    /// <param name="id">The ID of the product attribute template that needs to be updated. </param>
    /// <param name="model">The product attribute template parameter object containing the update information. </param>
    /// <returns>Return the result of the update operation. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] ProductAttributeTemplateParam model)
    {
        var productTemplate = await _productAttrTempRepo
            .Query()
            .Include(x => x.ProductAttributes)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (productTemplate == null)
            return Result.Fail("The document does not exist");

        productTemplate.Name = model.Name;
        productTemplate.UpdatedOn = DateTime.Now;

        var attributeIds = model.AttributeIds.Distinct();
        var attrIds = new List<int>();
        if (attributeIds.Count() > 0)
        {
            attrIds = await _productAttrRepo
                .Query(c => attributeIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();
            foreach (var attrId in attrIds)
            {
                if (productTemplate.ProductAttributes.Any(x => x.AttributeId == attrId)) continue;
                productTemplate.AddAttribute(attrId);
            }
        }

        var deletedAttributes = productTemplate.ProductAttributes.Where(attr => !attrIds.Contains(attr.AttributeId));

        foreach (var deletedAttribute in deletedAttributes)
        {
            deletedAttribute.IsDeleted = true;
            deletedAttribute.UpdatedOn = DateTime.Now;
        }

        _productAttrTempRepo.SaveChanges();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the product attribute template with the specified ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var first = await _productAttrTempRepo.Query().Include(c => c.ProductAttributes)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (first == null)
            return Result.Fail("The document does not exist");

        foreach (var item in first.ProductAttributes)
        {
            item.IsDeleted = true;
            item.UpdatedOn = DateTime.Now;
        }

        //var any = await _productAttrTempRelaRepo.Query().AnyAsync(c => c.TemplateId == first.Id);
        //if (any)
        //    return Result.Fail("Please make sure this template not used");

        first.IsDeleted = true;
        first.UpdatedOn = DateTime.Now;
        await _productAttrTempRepo.SaveChangesAsync();
        return Result.Ok();
    }
}
