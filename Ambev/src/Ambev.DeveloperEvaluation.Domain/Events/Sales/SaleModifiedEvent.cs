namespace Ambev.DeveloperEvaluation.Domain.Events
{
    public class SaleUpdatedEvent
    {
        public Guid SaleId { get; }
        public string CustomerName { get; }
        public DateTime SaleDate { get; }
        public decimal TotalAmount { get; }

        public SaleUpdatedEvent(Guid saleId, string customerName, DateTime saleDate, decimal totalAmount)
        {
            SaleId = saleId;
            CustomerName = customerName;
            SaleDate = saleDate;
            TotalAmount = totalAmount;
        }
    }
}
