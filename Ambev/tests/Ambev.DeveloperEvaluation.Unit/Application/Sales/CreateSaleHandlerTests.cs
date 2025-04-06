using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Domain.Services.Sales.Interface;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Unit.Application.Sales.CreateSale
{
    public class CreateSaleHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldApply10PercentDiscount_WhenMore4Products()
        {
            // Arrange
            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Sale>(It.IsAny<CreateSaleCommand>())).Returns(new Sale
            {
                CustomerName = "João",
                Branch = "Filial A",
                SaleDate = DateTime.UtcNow,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductName = "Produto Qualquer 1", Quantity = 5, UnitPrice = 10.0m },
                    new SaleItem { ProductName = "Produto Qualquer 2", Quantity = 1, UnitPrice = 20.0m }
                 }
            });

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();

            var domainServiceMock = new Mock<ISalesDomainService>();
            domainServiceMock.Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale =>
                {
                    var item = sale.Items.First(i => i.ProductName == "Produto Qualquer 1");
                    item.Discount = 0.10m;
                });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object,
                domainServiceMock.Object);

            var command = new CreateSaleCommand
            {
                CustomerName = "João",
                Branch = "Filial A",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand { ProductName = "Produto Qualquer 1", Quantity = 5, UnitPrice = 10.0m },
                    new CreateSaleItemCommand { ProductName = "Produto Qualquer 2", Quantity = 1, UnitPrice = 20.0m }
                }
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repositoryMock.Verify(r => r.AddAsync(It.Is<Sale>(sale =>
                sale.Items.Any(i => i.ProductName == "Produto Qualquer 1" && i.Discount == 0.10m)
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotApplyDiscount_WhenFewer4Products()
        {
            // ARRANGE
            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(m => m.Map<Sale>(It.IsAny<CreateSaleCommand>())).Returns(new Sale
            {
                CustomerName = "Maria",
                Branch = "Filial B",
                SaleDate = DateTime.UtcNow,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductName = "Produto Sem Desconto", Quantity = 4, UnitPrice = 10.0m },
                    new SaleItem { ProductName = "Outro Produto", Quantity = 2, UnitPrice = 20.0m }
                }
            });

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();

            var domainServiceMock = new Mock<ISalesDomainService>();
            domainServiceMock.Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale => { });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapperMock.Object,
                loggerMock.Object,
                domainServiceMock.Object
            );

            var command = new CreateSaleCommand
            {
                CustomerName = "Maria",
                Branch = "Filial B",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
                {
                    new CreateSaleItemCommand { ProductName = "Produto Sem Desconto", Quantity = 4, UnitPrice = 10.0m },
                    new CreateSaleItemCommand { ProductName = "Outro Produto", Quantity = 2, UnitPrice = 20.0m }
                }
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            repositoryMock.Verify(r => r.AddAsync(It.Is<Sale>(sale =>
                sale.Items.All(i => i.Discount == 0m)
            )), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldCalculateCorrectTotal_WhenProductsHaveAndDontHaveDiscount()
        {
            // ARRANGE
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreateSaleProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();
            var domainServiceMock = new Mock<ISalesDomainService>();

            domainServiceMock
                .Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale =>
                {
                    var groupedItems = sale.Items.GroupBy(i => i.ProductName);
                    foreach (var group in groupedItems)
                    {
                        var totalQuantity = group.Sum(i => i.Quantity);
                        if (totalQuantity > 4)
                        {
                            foreach (var item in group)
                            {
                                item.Discount = item.Quantity * item.UnitPrice * 0.10m;
                            }
                        }
                    }
                });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapper,
                loggerMock.Object,
                domainServiceMock.Object
            );

            var command = new CreateSaleCommand
            {
                CustomerName = "Maria",
                Branch = "Filial Teste",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 6, UnitPrice = 10.0m }, // desconto 10%
            new CreateSaleItemCommand { ProductName = "Produto B", Quantity = 2, UnitPrice = 20.0m }  // sem desconto
        }
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(94.0m, result.TotalAmount);
        }

        [Fact]
        public async Task Handle_ShouldNotApplyDiscount_WhenSameProductIsEqualOrLessThan4()
        {
            // ARRANGE
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreateSaleProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();
            var domainServiceMock = new Mock<ISalesDomainService>();

            domainServiceMock
                .Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale =>
                {
                    var groupedItems = sale.Items.GroupBy(i => i.ProductName);
                    foreach (var group in groupedItems)
                    {
                        var totalQuantity = group.Sum(i => i.Quantity);
                        if (totalQuantity > 4)
                        {
                            foreach (var item in group)
                            {
                                item.Discount = item.Quantity * item.UnitPrice * 0.10m;
                            }
                        }
                    }
                });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapper,
                loggerMock.Object,
                domainServiceMock.Object
            );

            var command = new CreateSaleCommand
            {
                CustomerName = "Lucas",
                Branch = "Filial Centro",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 2, UnitPrice = 10.0m },
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 2, UnitPrice = 10.0m },
            new CreateSaleItemCommand { ProductName = "Produto B", Quantity = 1, UnitPrice = 20.0m }
        }
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);

            // Total: (4x10) + (1x20) = 40 + 20 = 60
            Assert.Equal(60.0m, result.TotalAmount);

            // Verifica que nenhum item teve desconto aplicado
            Assert.All(result.Items, item => Assert.Equal(0, item.Discount));
        }

        [Fact]
        public async Task Handle_ShouldApplyDiscount_WhenSameProductIsSplitAcrossMultipleItems()
        {
            // ARRANGE
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreateSaleProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();
            var domainServiceMock = new Mock<ISalesDomainService>();

            domainServiceMock
                .Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale =>
                {
                    var groupedItems = sale.Items.GroupBy(i => i.ProductName);
                    foreach (var group in groupedItems)
                    {
                        var totalQuantity = group.Sum(i => i.Quantity);
                        if (totalQuantity > 4)
                        {
                            foreach (var item in group)
                            {
                                item.Discount = item.Quantity * item.UnitPrice * 0.10m;
                            }
                        }
                    }
                });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapper,
                loggerMock.Object,
                domainServiceMock.Object
            );

            var command = new CreateSaleCommand
            {
                CustomerName = "Camila",
                Branch = "Filial Leste",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 3, UnitPrice = 10.0m },
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 2, UnitPrice = 10.0m },
            new CreateSaleItemCommand { ProductName = "Produto B", Quantity = 1, UnitPrice = 20.0m }
        }
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);

            // Descontos esperados:
            // Produto A: (3+2)x10 = 50 - 10% = 45
            // Desconto: (3x10)x0.1 = 3.0 / (2x10)x0.1 = 2.0
            // Produto B: 1x20 = 20
            // Total esperado: 45 (Produto A) + 20 (Produto B) = 65

            var produtoADiscount = result.Items
                .Where(i => i.ProductName == "Produto A")
                .Sum(i => i.Discount);

            Assert.Equal(5.0m, produtoADiscount); // 10% de 50 = 5
            Assert.Equal(65.0m, result.TotalAmount);
        }

        [Fact]
        public async Task Handle_ShouldNotApplyDiscount_WhenDifferentProductsEvenIfTotalQuantityAbove4()
        {
            // ARRANGE
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreateSaleProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();
            var domainServiceMock = new Mock<ISalesDomainService>();

            domainServiceMock
                .Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale =>
                {
                    var groupedItems = sale.Items.GroupBy(i => i.ProductName);
                    foreach (var group in groupedItems)
                    {
                        var totalQuantity = group.Sum(i => i.Quantity);
                        if (totalQuantity > 4)
                        {
                            foreach (var item in group)
                            {
                                item.Discount = item.Quantity * item.UnitPrice * 0.10m;
                            }
                        }
                    }
                });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapper,
                loggerMock.Object,
                domainServiceMock.Object
            );

            var command = new CreateSaleCommand
            {
                CustomerName = "Luciana",
                Branch = "Filial Sul",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 3, UnitPrice = 10.0m },
            new CreateSaleItemCommand { ProductName = "Produto B", Quantity = 3, UnitPrice = 20.0m }
        }
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);

            // Verificamos se o desconto de ambos os produtos continua 0
            Assert.All(result.Items, item => Assert.Equal(0.0m, item.Discount));

            // Total esperado: (3 x 10) + (3 x 20) = 30 + 60 = 90
            Assert.Equal(90.0m, result.TotalAmount);
        }

        [Fact]
        public async Task Handle_ShouldCalculateTotalCorrectly_WhenMixedItemsWithAndWithoutDiscount()
        {
            // ARRANGE
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CreateSaleProfile>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repositoryMock = new Mock<ISaleRepository>();
            repositoryMock.Setup(r => r.AddAsync(It.IsAny<Sale>())).Returns(Task.CompletedTask);

            var loggerMock = new Mock<ILogger<CreateSaleHandler>>();
            var domainServiceMock = new Mock<ISalesDomainService>();

            domainServiceMock
                .Setup(s => s.ApplyBusinessRules(It.IsAny<Sale>()))
                .Callback<Sale>(sale =>
                {
                    var groupedItems = sale.Items.GroupBy(i => i.ProductName);
                    foreach (var group in groupedItems)
                    {
                        var totalQuantity = group.Sum(i => i.Quantity);
                        if (totalQuantity > 4)
                        {
                            foreach (var item in group)
                            {
                                item.Discount = item.Quantity * item.UnitPrice * 0.10m;
                            }
                        }
                    }
                });

            var handler = new CreateSaleHandler(
                repositoryMock.Object,
                mapper,
                loggerMock.Object,
                domainServiceMock.Object
            );

            var command = new CreateSaleCommand
            {
                CustomerName = "Cliente Misturado",
                Branch = "Filial Mista",
                SaleDate = DateTime.UtcNow,
                Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand { ProductName = "Produto A", Quantity = 6, UnitPrice = 10.0m }, // com desconto
            new CreateSaleItemCommand { ProductName = "Produto B", Quantity = 2, UnitPrice = 20.0m }  // sem desconto
        }
            };

            // ACT
            var result = await handler.Handle(command, CancellationToken.None);

            // ASSERT
            Assert.NotNull(result);

            var produtoADto = result.Items.First(i => i.ProductName == "Produto A");
            var produtoBDto = result.Items.First(i => i.ProductName == "Produto B");

            // Produto A → 6 x 10 = 60 - 10% = 54
            Assert.Equal(6, produtoADto.Quantity);
            Assert.Equal(10.0m, produtoADto.UnitPrice);
            Assert.Equal(6 * 10 * 0.10m, produtoADto.Discount);
            Assert.Equal((6 * 10) - (6 * 10 * 0.10m), produtoADto.Total);

            // Produto B → 2 x 20 = 40 (sem desconto)
            Assert.Equal(2, produtoBDto.Quantity);
            Assert.Equal(20.0m, produtoBDto.UnitPrice);
            Assert.Equal(0.0m, produtoBDto.Discount);
            Assert.Equal(2 * 20, produtoBDto.Total);

            // Total da venda esperado: 54 + 40 = 94
            Assert.Equal(94.0m, result.TotalAmount);
        }

    }
}
