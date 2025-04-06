namespace Project.AvaliacaoDeveloper.Domain.Events
{
    public class SaleCanceledEvent
    {
        public Guid SaleId { get; }
        public string CustomerName { get; }
        public DateTime SaleDate { get; }

        public SaleCanceledEvent(Guid saleId, string customerName, DateTime saleDate)
        {
            SaleId = saleId;
            CustomerName = customerName;
            SaleDate = saleDate;
        }
    }
}
