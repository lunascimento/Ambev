using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales
{
    public class GetAllSalesQuery : IRequest<GetAllSalesResult>
    {
        public int PageNumber { get; }
        public int PageSize { get; }

        public GetAllSalesQuery(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
