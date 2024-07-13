using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure;
using Shop.Infrastructure.Data;
using Shop.Module.Catalog.Entities;
using Shop.Module.Catalog.ViewModels;

namespace Shop.Module.Catalog.Controllers;

/// <summary>
/// Product attribute group API controller, responsible for the management operations of product attribute groups, such as query, create, update and delete.
/// </summary>
[Authorize(Roles = "admin")]
[Route("/api/product-attribute-groups")]
public class ProductAttributeGroupApiController : ControllerBase
{
    private readonly IRepository<ProductAttributeGroup> _productAttrGroupRepository;
    private readonly IRepository<ProductAttribute> _productAttrRepository;

    public ProductAttributeGroupApiController(IRepository<ProductAttributeGroup> productAttrGroupRepository,
        IRepository<ProductAttribute> productAttrRepository)
    {
        _productAttrGroupRepository = productAttrGroupRepository;
        _productAttrRepository = productAttrRepository;
    }

    /// <summary>
    /// Get a list of all product attribute groups.
    /// </summary>
    /// <returns> Return a list of product attribute groups. </returns>
    [HttpGet]
    public async Task<Result> Get()
    {
        var options = await _productAttrGroupRepository.Query().Select(c => new ProductAttributeGroupParam
        {
            Id = c.Id,
            Name = c.Name
        }).ToListAsync();
        return Result.Ok(options);
    }


    /// <summary>
    /// Get detailed information of the specified product attribute group according to the product attribute group ID.
    /// </summary>
    /// <param name="id">Product attribute group ID. </param>
    /// <returns>Return detailed information of the specified product attribute group. </returns>
    [HttpGet("{id:int:min(1)}")]
    public async Task<Result> Get(int id)
    {
        var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
        if (group == null)
            return Result.Fail("The document does not exist");
        var model = new ProductAttributeGroupParam
        {
            Id = group.Id,
            Name = group.Name
        };
        return Result.Ok(model);
    }

    /// <summary>
    /// Add a new product attribute group.
    /// </summary>
    /// <param name="model">Parameter object containing product attribute group information. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPost]
    public async Task<Result> Post([FromBody] ProductAttributeGroupParam model)
    {
        var group = new ProductAttributeGroup
        {
            Name = model.Name
        };
        _productAttrGroupRepository.Add(group);
        await _productAttrGroupRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Update the product attribute group information of the specified ID.
    /// </summary>
    /// <param name="id">Product attribute group ID. </param>
    /// <param name="model">Parameter object containing the updated information of the product attribute group. </param>
    /// <returns>Return the operation result. </returns>
    [HttpPut("{id:int:min(1)}")]
    public async Task<Result> Put(int id, [FromBody] ProductAttributeGroupParam model)
    {
        var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
        if (group == null)
            return Result.Fail("The document does not exist");
        group.Name = model.Name;
        group.UpdatedOn = DateTime.Now;
        await _productAttrGroupRepository.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Delete the product attribute group with the specified ID.
    /// </summary>
    /// <param name="id">Product attribute group ID. </param>
    /// <returns>Return the operation result. </returns>
    [HttpDelete("{id:int:min(1)}")]
    public async Task<Result> Delete(int id)
    {
        var group = await _productAttrGroupRepository.FirstOrDefaultAsync(id);
        if (group == null)
            return Result.Fail("The document does not exist");

        // Verify that the group is used by the attribute
        var any = await _productAttrRepository.Query().AnyAsync(c => c.GroupId == group.Id);
        if (any)
            return Result.Fail("Please make sure this group not used");

        group.IsDeleted = true;
        group.UpdatedOn = DateTime.Now;
        await _productAttrGroupRepository.SaveChangesAsync();
        return Result.Ok();
    }
}
