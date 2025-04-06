using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services.Sales.Interface
{
    public interface ISalesDomainService
    {
        void ApplyBusinessRules(Sale sale);
    }
}
