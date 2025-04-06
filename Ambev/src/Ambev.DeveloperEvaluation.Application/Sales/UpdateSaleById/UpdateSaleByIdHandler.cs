using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Services.Sales.Interface;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleByIdCommand, UpdateSaleByIdResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<UpdateSaleHandler> _logger;
        private readonly ISalesDomainService _salesDomainService;

        public UpdateSaleHandler(ISaleRepository saleRepository, ILogger<UpdateSaleHandler> logger, ISalesDomainService salesDomainService)
        {
            _saleRepository = saleRepository;
            _logger = logger;
            _salesDomainService = salesDomainService;
        }

        public async Task<UpdateSaleByIdResult> Handle(UpdateSaleByIdCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.Id);
            if (sale == null)
                throw new KeyNotFoundException($"Sale with ID {request.Id} not found.");

            sale.CustomerName = request.CustomerName;
            sale.Branch = request.Branch;
            //sale.IsCancelled = request.IsCancelled;
            //sale.SaleDate = request.SaleDate;

            sale.Items.Clear();
            foreach (var item in request.Items)
            {
                sale.Items.Add(new SaleItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    //Discount = item.Discount
                });
            }

            _salesDomainService.ApplyBusinessRules(sale);

            await _saleRepository.SaveAsync(sale);
            var saleUpdatedEvent = new SaleUpdatedEvent(sale.Id, sale.CustomerName, sale.SaleDate, sale.TotalAmount);
            _logger.LogInformation("Sale Updated: {@SaleUpdatedEvent}", saleUpdatedEvent);

            return new UpdateSaleByIdResult
            {
                Id = sale.Id,
                CustomerName = sale.CustomerName,
                Branch = sale.Branch,
                SaleDate = sale.SaleDate,
                IsCancelled = sale.IsCancelled,
                TotalAmount = sale.TotalAmount,
                Items = sale.Items.Select(item => new SaleItemDto
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    TotalAmount = item.Total
                }).ToList()
            };
        }
    }
}
