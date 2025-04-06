using Ambev.DeveloperEvaluation.Application.Sales.GetSales;

public class GetSalesByFilterResult
{
    public List<SaleDto> Sales { get; set; }

    public GetSalesByFilterResult(List<SaleDto> sales)
    {
        Sales = sales;
    }
}
