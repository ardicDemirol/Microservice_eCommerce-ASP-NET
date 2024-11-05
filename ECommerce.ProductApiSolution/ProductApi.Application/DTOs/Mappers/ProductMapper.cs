using ProductApi.Domain.Entities;
namespace ProductApi.Application.DTOs.Mappers;

public static class ProductMapper
{
    public static Product ToEntity(this ProductDto product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Quantity = product.Quantity
    };

    public static (ProductDto?, IEnumerable<ProductDto>?) ToDto(Product product, IEnumerable<Product>? products)
    {
        if (product is not null || products is null)
        {
            var singleProduct = new ProductDto(
                product!.Id,
                product.Name,
                product.Quantity,
                product.Price);

            return (singleProduct, null);
        }

        if (product is null || products is not null)
        {
            var multipleProducts = products!.Select(p => new ProductDto(
                p.Id, p.Name, p.Quantity, p.Price)).ToList();

            return (null, multipleProducts);
        }

        return (null, null);
    }
}