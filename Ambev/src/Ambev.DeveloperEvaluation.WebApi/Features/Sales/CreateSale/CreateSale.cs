namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale
{
    public class CreateSaleRequest
    {
        public string? Client { get; set; }
        public DateTime SaleDate { get; set; }
        public string? Branch { get; set; }
        public List<CreateSaleItemRequest>? Items { get; set; }
    }

    public class CreateSaleItemRequest
    {
        public string? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
