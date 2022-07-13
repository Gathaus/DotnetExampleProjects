using Dotnet7WebApi.Entities;

namespace Dotnet7WebApi.Repositories;

public class InMemItemsRepository : IItemsRepository
{
    private readonly List<Item> items = new()
    {
        new Item {Id = Guid.NewGuid(), Name = "Potion", Price = 10, CreatedDate = DateTimeOffset.UtcNow},
        new Item {Id = Guid.NewGuid(), Name = "Iron Sword", Price = 22, CreatedDate = DateTimeOffset.UtcNow},
        new Item {Id = Guid.NewGuid(), Name = "Bronze Shield", Price = 14, CreatedDate = DateTimeOffset.UtcNow},
    };

    public async Task<IEnumerable<Item>> GetItemsAsync()
    {
        return await Task.FromResult(items);
    }

    public async Task<Item> GetItemAsync(Guid id)
    {
        return await Task.FromResult(items.FirstOrDefault(i => i.Id == id));
    }

    public async Task CreateItemAsync(Item item)
    {
        items.Add(item);
        await Task.CompletedTask;
    }

    public async Task UpdateItemAsync(Item item)
    {
        var index = items.FindIndex(x => x.Id == item.Id);

        items[index] = item;
        await Task.CompletedTask;
    }

    public async Task DeleteItemAsync(Guid id)
    {
        var index = items.FindIndex(x => x.Id == id);
        items.RemoveAt(index);
        await Task.CompletedTask;
    }
}