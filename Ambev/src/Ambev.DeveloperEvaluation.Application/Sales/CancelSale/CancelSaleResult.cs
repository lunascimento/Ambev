namespace Project.AvaliacaoDeveloper.Application.Sales.CancelSale
{
    public class CancelSaleResult
    {
        public Guid SaleId { get; set; }
        public bool IsCancelled { get; set; }

        public CancelSaleResult(Guid saleId, bool IsCancelled)
        {
            SaleId = saleId;
            IsCancelled = IsCancelled;
        }
    }
}
