using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductApi.Controllers.Repositories;

public class ProductRepositoryTest
{
    private readonly ProductDbContext dbContext;
    private readonly ProductRepository productRepository;
    public ProductRepositoryTest()
    {
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        dbContext = new ProductDbContext(options);
        productRepository = new ProductRepository(dbContext);
    }

    // ------------------- Create Product -------------------

    [Fact]
    public async Task CreateAsync_WhenProductAlreadyExist_ReturnErrorResponse()
    {
        // Arrange
        var existProduct = new Product { Name = "ExistProduct" };
        dbContext.Products.Add(existProduct);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await productRepository.CreateAsync(existProduct);

        // Assert
        result.Should().NotBeNull();
        result.Flag.Should().BeFalse();
        result.Message.Should().Be($"{existProduct.Name} already exist");
    }

    [Fact]
    public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnSuccessResponse()
    {
        // Arrange
        var newProduct = new Product { Name = "New Product" };

        // Act
        var result = await productRepository.CreateAsync(newProduct);

        // Assert
        result.Should().NotBeNull();
        result.Flag.Should().BeTrue();
        result.Message.Should().Be($"{newProduct.Name} added successfully");
    }

    // ------------------- Delete Product ------------------------

    [Fact]
    public async Task DeleteAsync_WhenProductIsFound_ReturnSuccessResponse()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "ExistProduct", Price = 62, Quantity = 4 };
        dbContext.Products.Add(product);

        // Act
        var result = await productRepository.DeleteAsync(product);

        // Assert

        result.Should().NotBeNull();
        result.Flag.Should().BeTrue();
        result.Message.Should().Be("Product deleted successfully");

    }


    [Fact]
    public async Task DeleteAsync_WhenProductIsNotFound_ReturnNotFoundResponse()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "NonExistProduct", Price = 62, Quantity = 4 };

        // Act
        var result = await productRepository.DeleteAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Flag.Should().BeFalse();
        result.Message.Should().Be($"{product.Name} not found");
    }


    // ------------------- Get Product By Id ------------------------

    [Fact]
    public async Task FindByIdAsync_WhenProductIsFound_ReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 2, Name = "Product", Price = 62, Quantity = 4 };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await productRepository.FindByIdAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
        result.Price.Should().Be(product.Price);
        result.Quantity.Should().Be(product.Quantity);
    }

    [Fact]
    public async Task FindByIdAsync_WhenProductIsNotFound_ReturnNull()
    {
        // Arrange
        var product = new Product { Id = -1, Name = "Product", Price = 62, Quantity = 4 };
        // Act
        var result = await productRepository.FindByIdAsync(product.Id);

        // Assert
        result.Should().BeNull();
    }


    // --------------------- Get All Products ------------------------

    [Fact]
    public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
    {
        // Arrange
        List<Product> products =
        [
            new() { Id = 1, Name = "Product 1", Price = 100 ,Quantity = 10 },
            new() { Id = 2, Name = "Product 2", Price = 200 ,Quantity = 20 }
        ];

        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await productRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(products.Count);
        result.Should().Contain(p => p.Name!.Equals(products[0].Name));
        result.Should().Contain(p => p.Name!.Equals(products[1].Name));

    }

    [Fact]
    public async Task GetAllAsync_WhenProductsAreNotFound_ReturnNull()
    {
        // Arrange

        // Act
        var result = await productRepository.GetAllAsync();

        // Assert
        result.Should().BeNullOrEmpty();
    }


    // ------------ Get By Any Type (int,bool,string, etc .....) ------------

    [Fact]
    public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product", Price = 62, Quantity = 4 };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();
        Expression<Func<Product, bool>> expression = p => p.Name!.Equals(product.Name);

        // Act
        var result = await productRepository.GetByAsync(expression);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
    {
        // Arrange
        Expression<Func<Product, bool>> expression = p => p.Name!.Equals("");

        // Act
        var result = await productRepository.GetByAsync(expression);

        // Assert
        result.Should().BeNull();
    }


    // ------------------- Update Product -------------------

    [Fact]
    public async Task UpdateAsync_WhenProductIsUpdatedSuccessfully_ReturnSuccessResponse()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product", Price = 62, Quantity = 4 };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await productRepository.UpdateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Flag.Should().BeTrue();
        result.Message.Should().Be($"{product.Name} is updated successfully");
    }


    [Fact]
    public async Task UpdateAsync_WhenProductIsNotFound_ReturnErrorResponse()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Product", Price = 62, Quantity = 4 };

        // Act
        var result = await productRepository.UpdateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Flag.Should().BeFalse();
        result.Message.Should().Be($"{product.Name} not found");
    }


}
