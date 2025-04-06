using MediatR;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleByIdCommand : IRequest<UpdateSaleByIdResult>
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string Branch { get; set; }
        public List<UpdateSaleItemCommand> Items { get; set; }
    }

    public class UpdateSaleItemCommand
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
