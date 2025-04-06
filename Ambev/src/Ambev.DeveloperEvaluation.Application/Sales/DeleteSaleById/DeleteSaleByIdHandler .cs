using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleByIdHandler : IRequestHandler<DeleteSaleByIdCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;

        public DeleteSaleByIdHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<bool> Handle(DeleteSaleByIdCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id);
            if (sale == null)
            {
                return false;
            }

            await _saleRepository.DeleteAsync(sale);
            return true;
        }
    }
}
