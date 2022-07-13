using Dotnet7WebApi.Entities;
using Dotnet7WebApi;

namespace Dotnet7WebApi;

public static class Extensions
{
        public static Dtos.ItemDto ToDto(this Item item)
        {
            return new Dtos.ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
}