using Microsoft.AspNetCore.Authorization;
using RealEstate.Domain.DTOs.Image;
using RealEstate.Shared.Entities;
using RealEstate.Shared.Entities.Response;

namespace RealEstate.Api.Controllers;

[Authorize]
[ApiController]
public class PropertyBuildingController : BaseController
{
    /// <summary>Creates a new property.</summary>
    /// <param name="useCase">Use case to create property.</param>
    /// <param name="request">Data for new property.</param>
    /// <response code="200">Property created successfully.</response>
    /// <response code="400">Bad request — validation error.</response>
    /// <response code="500">Internal server error.</response>
    [ProducesResponseType(typeof(Result<Unit>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromServices] CreatePropertyUseCase useCase,
        [FromBody] CreatePropertyRequest request
    )
    {
        return await useCase.Execute(request)
            .UseSuccessHttpStatusCode(HttpStatusCode.OK)
            .ToActionResult();
    }
    
    /// <summary>Uploads an image for a property.</summary>
    /// <param name="useCase">Use case to upload image.</param>
    /// <param name="propertyId">Property identifier.</param>
    /// <param name="request">Image upload request.</param>
    /// <response code="200">Image uploaded successfully.</response>
    /// <response code="400">Bad request (e.g. invalid extension).</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("{PropertyId:guid}/image")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Result<Unit>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UploadImage(
        [FromServices] AddImageToPropertyUseCase useCase,
        [FromRoute] Guid propertyId,
        [FromForm] AddImageToPropertyRequest request
    )
    {
        request.PropertyId = propertyId;
        request.FileName = request.File.FileName;
        return await useCase.Execute(request)
            .UseSuccessHttpStatusCode(HttpStatusCode.OK)
            .ToActionResult();
    }
    
    /// <summary>Updates the price of a property.</summary>
    /// <param name="useCase">Use case to change price.</param>
    /// <param name="propertyId">Property identifier.</param>
    /// <param name="request">Request containing new price.</param>
    /// <response code="200">Price updated.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{propertyId:guid}/price")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Result<Unit>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePrice(
        [FromServices] ChangePropertyPriceUseCase useCase,
        [FromRoute] Guid propertyId,
        [FromBody] ChangePropertyPriceRequest request
    )
    {
        request.PropertyId = propertyId;
        return await useCase.Execute(request)
            .UseSuccessHttpStatusCode(HttpStatusCode.OK)
            .ToActionResult();
    }
    
    /// <summary>Updates property details and image status.</summary>
    /// <param name="useCase">Use case to update property.</param>
    /// <param name="propertyId">Property identifier.</param>
    /// <param name="request">Update request payload.</param>
    /// <response code="200">Entity updated successfully.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPut("{propertyId:guid}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Result<Unit>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(
        [FromServices] UpdatePropertyUseCase useCase,
        [FromRoute] Guid propertyId,
        [FromBody] UpdatePropertyRequest request
    )
    {
        request.Id = propertyId;
        return await useCase.Execute(request)
            .UseSuccessHttpStatusCode(HttpStatusCode.OK)
            .ToActionResult();
    }
    
    /// <summary>Lists properties using filters (search, pagination).</summary>
    /// <param name="useCase">Use case to list properties.</param>
    /// <param name="request">Filter and paging parameters.</param>
    /// <response code="200">Returns filtered and paged list of properties.</response>
    /// <response code="400">Bad request.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost("list")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Result<PagedResult<PropertyResponse>>),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> List(
        [FromServices] ListPropertyWithFiltersUseCase useCase,
        [FromBody] ListPropertyFiltersRequest request
    )
    {
        return await useCase.Execute(request)
            .UseSuccessHttpStatusCode(HttpStatusCode.OK)
            .ToActionResult();
    }
}