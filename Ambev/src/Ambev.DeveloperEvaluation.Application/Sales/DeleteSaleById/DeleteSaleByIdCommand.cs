using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleByIdCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public DeleteSaleByIdCommand(Guid id)
        {
            Id = id;
        }
    }
}
