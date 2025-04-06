using MediatR;

namespace Project.AvaliacaoDeveloper.Application.Sales.CancelSale
{
    public class CancelSaleCommand : IRequest<Unit>
    {
        public Guid SaleId { get; }

        public CancelSaleCommand(Guid saleId)
        {
            SaleId = saleId;
        }
    }
}
