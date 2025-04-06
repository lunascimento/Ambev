using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.AvaliacaoDeveloper.Domain.Events;

namespace Project.AvaliacaoDeveloper.Application.Sales.CancelSale
{
    public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, Unit>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<CancelSaleHandler> _logger;

        public CancelSaleHandler(ISaleRepository saleRepository, ILogger<CancelSaleHandler> logger)
        {
            _saleRepository = saleRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId);

            if (sale == null)
                throw new KeyNotFoundException("Venda não encontrada.");

            if (sale.IsCancelled)
                throw new InvalidOperationException("A venda já está cancelada.");

            sale.Cancel();

            await _saleRepository.SaveAsync(sale);

            var saleCanceledEvent = new SaleCanceledEvent(sale.Id, sale.CustomerName!, sale.SaleDate);
            _logger.LogInformation("Sale Cancelled: {@SaleCanceledEvent}", saleCanceledEvent);

            return Unit.Value;
        }
    }
}
