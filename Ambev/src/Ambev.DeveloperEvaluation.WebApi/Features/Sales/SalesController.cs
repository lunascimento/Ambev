using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Project.AvaliacaoDeveloper.Application.Sales.CancelSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SalesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponseWithData<CreateSaleResult>>> Create([FromBody] CreateSaleCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponseWithData<CreateSaleResult>.SuccessResponse(result));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseWithData<GetAllSalesResult>>> GetAllSales([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = new GetAllSalesQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(ApiResponseWithData<GetAllSalesResult>.SuccessResponse(result));
        }  

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseWithData<GetSaleByIdResult>>> GetById(Guid id)
        {
            try
            {
                var query = new GetSaleByIdQuery(id);
                var result = await _mediator.Send(query);

                return Ok(ApiResponseWithData<GetSaleByIdResult>.SuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse.FailResponse(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailResponse("An error occurred while processing your request"));
            }
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetSalesByFilter([FromQuery] string? customerName, [FromQuery] string? branch, [FromQuery] bool? isCancelled, CancellationToken cancellationToken)
        {
            var query = new GetSalesByFilterQuery(customerName, branch, isCancelled, null, null);

            var result = await _mediator.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse>> Delete(Guid id)
        {
            await _mediator.Send(new DeleteSaleByIdCommand(id));

            return Ok(ApiResponse.SuccessResponse($"Sale {id} deleted successfully"));
        }

        [HttpDelete("{saleId}/items/{itemId}")]
        public async Task<ActionResult<ApiResponse>> DeleteSaleItem(Guid saleId, Guid itemId)
        {
            var result = await _mediator.Send(new DeleteSaleItemByIdCommand(saleId, itemId));

            if (!result.Success)
                return NotFound(ApiResponse.FailResponse(result.Message));

            return Ok(ApiResponse.SuccessResponse(result.Message));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseWithData<UpdateSaleByIdResult>>> Update(Guid id, [FromBody] UpdateSaleByIdCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);

            return Ok(ApiResponseWithData<UpdateSaleByIdResult>.SuccessResponse(result));
        }
    }
}
