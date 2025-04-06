using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
    {
        public CreateSaleValidator()
        {
            //  Camada para as validações, não apliquei nenhum pois não há este requisito.

            /*
            RuleFor(x => x.SaleDate)
                .NotEmpty().WithMessage("A data da venda é obrigatória.");

            RuleFor(x => x.Client)
                .NotEmpty().WithMessage("O cliente é obrigatório.");

            RuleFor(x => x.Branch)
                .NotEmpty().WithMessage("A filial é obrigatória.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("A venda deve conter pelo menos um item.");

            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.Product)
                    .NotEmpty().WithMessage("O nome do produto é obrigatório.");

                items.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("A quantidade deve ser maior que 0.");

                items.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0).WithMessage("O preço unitário deve ser maior que 0.");

                items.RuleFor(i => i.Discount)
                    .GreaterThanOrEqualTo(0).WithMessage("O desconto não pode ser negativo.");
            });
            */
        }
    }
}
