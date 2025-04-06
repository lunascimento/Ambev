using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Services.Sales.Interface;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Project.AvaliacaoDeveloper.Domain.Events;


namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateSaleHandler> _logger;
        private readonly ISalesDomainService _salesDomainService;

        public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<CreateSaleHandler> logger, ISalesDomainService salesDomainService)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _logger = logger;
            _salesDomainService = salesDomainService;
        }

        public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = _mapper.Map<Sale>(request);

            _salesDomainService.ApplyBusinessRules(sale);

            foreach (var item in sale.Items)
            {
                item.Total = (item.Quantity * item.UnitPrice) - item.Discount;
            }

            sale.TotalAmount = sale.Items.Sum(i => i.Total);

            await _saleRepository.AddAsync(sale);

            var saleCreatedEvent = new SaleCreatedEvent(sale.Id, sale.CustomerName!, sale.SaleDate);
            _logger.LogInformation("Sale Created: {@SaleCreatedEvent}", saleCreatedEvent);

            var result = _mapper.Map<CreateSaleResult>(sale);

            return result;
        }
    }
}
