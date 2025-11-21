using Firmeza.Api.Controllers;
using Firmeza.Api.DTOs;
using Firmeza.Core.Entities;
using Firmeza.Core.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Firmeza.Test;

public class ProductsControllerTests
{
    private readonly Mock<IGenericRepository<Product>> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockRepository = new Mock<IGenericRepository<Product>>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 100, Stock = 10 },
            new Product { Id = 2, Name = "Product 2", Description = "Description 2", Price = 200, Stock = 20 }
        };

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = 1, Name = "Product 1", Description = "Description 1", Price = 100, Stock = 10 },
            new ProductDto { Id = 2, Name = "Product 2", Description = "Description 2", Price = 200, Stock = 20 }
        };

        _mockRepository.Setup(repo => repo.GetAllAsync(null))
            .ReturnsAsync(products);

        _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(products))
            .Returns(productDtos);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count());
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product 1", Description = "Description 1", Price = 100, Stock = 10 };
        var productDto = new ProductDto { Id = 1, Name = "Product 1", Description = "Description 1", Price = 100, Stock = 10 };

        _mockRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(product);

        _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("Product 1", returnedProduct.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(999))
            .ReturnsAsync((Product)null!);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithNewProduct()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 150,
            Stock = 15
        };

        var product = new Product
        {
            Id = 3,
            Name = "New Product",
            Description = "New Description",
            Price = 150,
            Stock = 15
        };

        var productDto = new ProductDto
        {
            Id = 3,
            Name = "New Product",
            Description = "New Description",
            Price = 150,
            Stock = 15
        };

        _mockMapper.Setup(mapper => mapper.Map<Product>(createProductDto))
            .Returns(product);

        _mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product))
            .Returns(productDto);

        // Act
        var result = await _controller.Create(createProductDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProduct = Assert.IsType<ProductDto>(createdResult.Value);
        Assert.Equal("New Product", returnedProduct.Name);
    }
}
