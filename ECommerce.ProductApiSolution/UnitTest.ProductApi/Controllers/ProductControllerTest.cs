using AutoFixture;
using ECommerce.SharedLibrary.Response;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers;

public class ProductControllerTest
{
    private readonly IProduct productInterface;
    private readonly ProductsController productsController;

    public ProductControllerTest()
    {
        productInterface = A.Fake<IProduct>();
        productsController = new ProductsController(productInterface);
    }

    // ------------------- Get All Product -------------------
    [Fact]
    public async Task GetProduct_WhenProductExists_ReturnOkResponseWithProduct()
    {
        // Arrange
        //List<Product> products =
        //[
        //    new() { Id = 1, Name = "Product 1", Price = 100 ,Quantity = 10 },
        //    new() { Id = 2, Name = "Product 2", Price = 200 ,Quantity = 20 }
        //];

        var fixture = new Fixture();
        List<Product> products = fixture.CreateMany<Product>(2).ToList();


        //  Set up fake response for GetAllAsync method
        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        // Act 
        var result = await productsController.GetProducts();

        // Assert
        var okResult = result.Result as OkObjectResult;

        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);


        var returnedProducts = okResult.Value as IEnumerable<ProductDto>;
        returnedProducts.Should().NotBeNull();
        returnedProducts.Should().HaveCount(products.Count);
        returnedProducts.First().Id.Should().Be(products[0].Id);
        returnedProducts.Last().Id.Should().Be(products[^1].Id);
    }


    [Fact]
    public async Task GetProduct_WhenNoProductExists_ReturnNotFoundResponse()
    {
        // Arrange
        List<Product> products = [];

        //  Set up fake response for GetAllAsync method
        A.CallTo(() => productInterface.GetAllAsync()).Returns(products);

        // Act 
        var result = await productsController.GetProducts();

        // Assert
        var notFoundResult = result.Result as NotFoundObjectResult;

        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var messsage = notFoundResult.Value as string;
        messsage.Should().Be("No products detected in the database");
    }


    // ------------------- Create Product -------------------
    [Fact]
    public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
    {
        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        var fixture = new Fixture();
        ProductDto productDto = fixture.Create<ProductDto>();

        productsController.ModelState.AddModelError("Name", "Name is required");

        // Act
        var result = await productsController.CreateProduct(productDto);

        // Assert

        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

    }

    [Fact]
    public async Task CreateProduct_WhenCreateIsSuccessfull_ReturnOkResponse()
    {
        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        //var response = new Response(true, "Product added successfully");
        var fixture = new Fixture();

        ProductDto productDto = fixture.Create<ProductDto>();

        var response = fixture.Build<Response>()
         .With(r => r.Flag, true)
         .With(r => r.Message, "Product added successfully")
         .Create();


        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);

        //Act
        var result = await productsController.CreateProduct(productDto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseResult = okResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult.Message.Should().Be("Product added successfully");
        responseResult.Flag.Should().BeTrue();
    }


    [Fact]
    public async Task CreateProduct_WhenCreateFailed_ReturnBadRequestResponse()
    {
        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        //var response = new Response(false, "Product can not added successfully");

        var fixture = new Fixture();
        ProductDto productDto = fixture.Create<ProductDto>();
        var response = fixture.Build<Response>()
        .With(r => r.Flag, false)
        .With(r => r.Message, "Product can not added successfully")
        .Create();

        A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored)).Returns(response);

        //Act
        var result = await productsController.CreateProduct(productDto);


        // Assert
        var okResult = result.Result as BadRequestObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseResult = okResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult.Message.Should().Be("Product can not added successfully");
        responseResult.Flag.Should().BeFalse();
    }


    // ------------------- Update Product -------------------

    [Fact]
    public async Task UpdateProduct_WhenUpdateIsSuccessfull_ReturnOkResponse()
    {
        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        //var response = new Response(true, "Product updated successfully");

        var fixture = new Fixture();
        ProductDto productDto = fixture.Create<ProductDto>();
        var response = fixture.Build<Response>()
       .With(r => r.Flag, true)
       .With(r => r.Message, "Product updated successfully")
       .Create();

        A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

        //Act
        var result = await productsController.UpdateProduct(1, productDto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseResult = okResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult.Message.Should().Be("Product updated successfully");
        responseResult.Flag.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestResponse()
    {
        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        //var response = new Response(false, "Product updated failed");

        var fixture = new Fixture();
        ProductDto productDto = fixture.Create<ProductDto>();
        var response = fixture.Build<Response>()
        .With(r => r.Flag, false)
        .With(r => r.Message, "Product updated failed")
         .Create();

        A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored)).Returns(response);

        //Act
        var result = await productsController.UpdateProduct(1, productDto);


        // Assert
        var okResult = result.Result as BadRequestObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseResult = okResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult.Message.Should().Be("Product updated failed");
        responseResult.Flag.Should().BeFalse();
    }


    // ------------------- Delete Product -------------------

    [Fact]
    public async Task DeleteProduct_WhenDeleteIsSuccessful_ReturnOkResponse()
    {
        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        //var response = new Response(true, "Product deleted");

        var fixture = new Fixture();
        ProductDto productDto = fixture.Create<ProductDto>();
        var response = fixture.Build<Response>()
       .With(r => r.Flag, true)
       .With(r => r.Message, "Product deleted")
        .Create();


        A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

        //Act
        var result = await productsController.DeleteProduct(productDto);


        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseResult = okResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult.Message.Should().Be("Product deleted");
        responseResult.Flag.Should().BeTrue();
    }


    [Fact]
    public async Task DeleteProduct_WhenDeleteFails_ReturnBadRequestResponse()
    {

        // Arrange
        //var productDto = new ProductDto(1, "Product 1", 10, 100);
        //var response = new Response(false, "Product could not be deleted");

        var fixture = new Fixture();
        ProductDto productDto = fixture.Create<ProductDto>();
        var response = fixture.Build<Response>()
         .With(r => r.Flag, false)
         .With(r => r.Message, "Product could not be deleted")
          .Create();

        A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored)).Returns(response);

        //Act
        var result = await productsController.DeleteProduct(productDto);


        // Assert
        var okResult = result.Result as BadRequestObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        var responseResult = okResult.Value as Response;
        responseResult.Should().NotBeNull();
        responseResult.Message.Should().Be("Product could not be deleted");
        responseResult.Flag.Should().BeFalse();
    }


}
