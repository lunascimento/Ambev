using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales
{
    public interface ISaleRepository
    {
        Task AddAsync(Sale sale);
        Task<List<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(Guid id);
        Task<List<Sale>> GetSalesByFilterAsync(string? customerName, string? branch, bool? isCancelled, DateTime? startDate, DateTime? endDate);
        Task DeleteAsync(Sale sale);
        Task SaveAsync(Sale sale);

    }
}
