using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services.Sales.Interface;

namespace Ambev.DeveloperEvaluation.Domain.Services.Sales
{
    public class SalesDomainService : ISalesDomainService
    {
        public void ApplyBusinessRules(Sale sale)
        {
            ValidateItemQuantities(sale);
            ApplyDiscounts(sale);
        }
        private void ValidateItemQuantities(Sale sale)
        {
            var groupedByProduct = sale.Items
                .GroupBy(item => item.ProductName)
                .ToList();

            foreach (var group in groupedByProduct)
            {
                var totalQuantity = group.Sum(item => item.Quantity);

                if (totalQuantity > 20)
                    throw new DomainException($"Não é permitido vender mais de 20 unidades do produto '{group.Key}'.");
            }
        }

        private void ApplyDiscounts(Sale sale)
        {
            var groupedByProduct = sale.Items
                .GroupBy(item => item.ProductName)
                .ToList();

            foreach (var group in groupedByProduct)
            {
                var totalQuantity = group.Sum(item => item.Quantity);
                decimal discountPercentage = 0;

                if (totalQuantity >= 4 && totalQuantity < 10)
                    discountPercentage = 0.10m;
                else if (totalQuantity >= 10 && totalQuantity <= 20)
                    discountPercentage = 0.20m;

                foreach (var item in group)
                {
                    var itemTotal = item.UnitPrice * item.Quantity;
                    item.Discount = itemTotal * discountPercentage;
                    item.Total = itemTotal - item.Discount;
                }
            }

            sale.TotalAmount = sale.Items.Sum(item => item.Total);
        }

    }
}
