using MediatR;

public class GetSalesByFilterQuery : IRequest<GetSalesByFilterResult>
{
    public string? CustomerName { get; }
    public string? Branch { get; }
    public bool? IsCancelled { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public GetSalesByFilterQuery(
        string? customerName,
        string? branch,
        bool? isCancelled,
        DateTime? startDate,
        DateTime? endDate)
    {
        CustomerName = customerName;
        Branch = branch;
        IsCancelled = isCancelled;
        StartDate = startDate;
        EndDate = endDate;
    }
}
