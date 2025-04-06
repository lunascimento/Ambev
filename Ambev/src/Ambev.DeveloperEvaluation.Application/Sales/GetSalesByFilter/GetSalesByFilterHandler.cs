using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using MediatR;

public class GetSalesByFilterHandler : IRequestHandler<GetSalesByFilterQuery, GetSalesByFilterResult>
{
    private readonly ISaleRepository _saleRepository;

    public GetSalesByFilterHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<GetSalesByFilterResult> Handle(GetSalesByFilterQuery request, CancellationToken cancellationToken)
    {
        var sales = await _saleRepository.GetSalesByFilterAsync(
            request.CustomerName,
            request.Branch,
            request.IsCancelled,
            request.StartDate,
            request.EndDate
        );

        var salesDtos = sales.Select(s => new SaleDto
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
        }).ToList();

        return new GetSalesByFilterResult(salesDtos);
    }
}
