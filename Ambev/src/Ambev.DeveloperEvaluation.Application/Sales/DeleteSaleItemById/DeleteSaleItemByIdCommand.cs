using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSaleItem
{
    public class DeleteSaleItemByIdCommand : IRequest<DeleteSaleItemResult>
    {
        public Guid SaleId { get; set; }
        public Guid ItemId { get; set; }

        public DeleteSaleItemByIdCommand(Guid saleId, Guid itemId)
        {
            SaleId = saleId;
            ItemId = itemId;
        }
    }
}
