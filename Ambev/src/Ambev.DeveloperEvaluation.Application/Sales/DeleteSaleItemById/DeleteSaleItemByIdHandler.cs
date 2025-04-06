using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSaleItem
{
    public class DeleteSaleItemByIdHandler : IRequestHandler<DeleteSaleItemByIdCommand, DeleteSaleItemResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<DeleteSaleItemByIdHandler> _logger;

        public DeleteSaleItemByIdHandler(ISaleRepository saleRepository, ILogger<DeleteSaleItemByIdHandler> logger)
        {
            _saleRepository = saleRepository;
            _logger = logger;
        }

        public async Task<DeleteSaleItemResult> Handle(DeleteSaleItemByIdCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId);

            if (sale == null)
                return new DeleteSaleItemResult { Success = false, Message = "Sale not found" };

            var item = sale.Items.FirstOrDefault(i => i.Id == request.ItemId);

            if (item == null)
                return new DeleteSaleItemResult { Success = false, Message = "Item not found" };

            sale.Items.Remove(item);

            var groupedItems = sale.Items.GroupBy(i => i.ProductName);

            foreach (var group in groupedItems)
            {
                int totalQuantity = group.Sum(i => i.Quantity);
                decimal discount = totalQuantity >= 5 ? 0.10m : 0m;

                foreach (var i in group)
                {
                    i.Discount = discount;
                }
            }

            // Atualiza o valor total da venda
            sale.TotalAmount = sale.Items.Sum(i =>
                (i.UnitPrice * i.Quantity) * (1 - i.Discount) - i.Discount);

            await _saleRepository.SaveAsync(sale);

            _logger.LogInformation("Item {ItemId} removed from sale {SaleId}", request.ItemId, request.SaleId);

            return new DeleteSaleItemResult { Success = true, Message = "Item removed successfully" };
        }
    }
}
