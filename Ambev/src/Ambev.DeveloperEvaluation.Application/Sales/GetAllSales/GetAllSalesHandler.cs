using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    public class GetAllSalesHandler : IRequestHandler<GetAllSalesQuery, GetAllSalesResult>
    {
        private readonly ISaleRepository _saleRepository;

        public GetAllSalesHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<GetAllSalesResult> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
        {
            var allSales = await _saleRepository.GetAllAsync();

            var pagedSales = allSales
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var result = new GetAllSalesResult
            {
                Sales = pagedSales.Select(s => new SaleDto
                {
                    Id = s.Id,
                    CustomerName = s.CustomerName,
                    SaleDate = s.SaleDate,
                    TotalAmount = s.TotalAmount,
                    IsCancelled = s.IsCancelled,
                    Items = s.Items.Select(i => new SaleItemDto
                    {
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Discount = i.Discount,
                        TotalAmount = i.Total
                    }).ToList()
                }).ToList()
            };

            return result;
        }

    }
}
