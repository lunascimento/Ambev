using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById
{
    public class GetSaleByIdQuery : IRequest<GetSaleByIdResult>
    {
        public Guid Id { get; set; }

        public GetSaleByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
