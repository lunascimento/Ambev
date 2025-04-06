namespace Project.AvaliacaoDeveloper.Domain.Events
{
    public class SaleCreatedEvent
    {
        public Guid SaleId { get; }
        public string CustomerName { get; }
        public DateTime SaleDate { get; }

        public SaleCreatedEvent(Guid saleId, string customerName, DateTime saleDate)
        {
            SaleId = saleId;
            CustomerName = customerName;
            SaleDate = saleDate;
        }
    }
}
