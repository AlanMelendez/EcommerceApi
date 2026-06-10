using AutoMapper;
using Ecommerce.Application.DTOs.Categories;
using Ecommerce.Application.DTOs.Products;
using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryResponse>();

        CreateMap<Product, ProductResponse>()
            .ForMember( //If the product has a category loaded, use the category name. If not, return null or message.
                destination => destination.CategoryName,
                options => options.MapFrom(source => source.Category != null
                    ? source.Category.Name
                    : "Not found category name"));
    }
}