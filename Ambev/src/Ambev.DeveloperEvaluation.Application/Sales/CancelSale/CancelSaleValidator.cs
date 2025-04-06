using FluentValidation;

namespace Project.AvaliacaoDeveloper.Application.Sales.CancelSale
{
    public class CancelSaleValidator : AbstractValidator<CancelSaleCommand>
    {
        public CancelSaleValidator()
        {
            //RuleFor(x => x.SaleId)
            //    .NotEmpty().WithMessage("O ID da venda é obrigatório.");
        }
    }
}
