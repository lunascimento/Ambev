namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleByIdResult
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Branch { get; set; }
        public DateTime SaleDate { get; set; }
        public bool IsCancelled { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemDto> Items { get; set; }
    }

    public class SaleItemDto
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
