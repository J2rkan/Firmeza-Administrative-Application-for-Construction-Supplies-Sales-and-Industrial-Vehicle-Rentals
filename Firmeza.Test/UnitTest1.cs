using Firmeza.Core.Entities;
using Xunit;

namespace Firmeza.Test
{
    public class ProductTests
    {
        [Fact]
        public void AddStock_ShouldIncreaseStock_WhenQuantityIsPositive()
        {
            // Arrange
            var product = new Product { Stock = 10 };

            // Act
            product.AddStock(5);

            // Assert
            Assert.Equal(15, product.Stock);
        }

        [Fact]
        public void RemoveStock_ShouldDecreaseStock_WhenQuantityIsValid()
        {
            // Arrange
            var product = new Product { Stock = 10 };

            // Act
            product.RemoveStock(5);

            // Assert
            Assert.Equal(5, product.Stock);
        }

        [Fact]
        public void RemoveStock_ShouldThrowException_WhenStockIsInsufficient()
        {
            // Arrange
            var product = new Product { Stock = 10 };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => product.RemoveStock(15));
        }
    }
}