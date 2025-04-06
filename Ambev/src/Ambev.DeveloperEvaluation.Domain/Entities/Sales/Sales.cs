namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Sale
    {
        public Guid Id { get; set; }
        public string? SaleNumber { get; set; }
        public DateTime SaleDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Branch { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; } = false;

        public List<SaleItem> Items { get; set; } = new();

        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("A venda já está cancelada.");

            IsCancelled = true;
        }
    }
}
