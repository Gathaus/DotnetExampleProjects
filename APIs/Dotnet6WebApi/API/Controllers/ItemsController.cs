using Dotnet7WebApi.Entities;
using Dotnet7WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet7WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IItemsRepository _repository;

    public ItemsController(IItemsRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IEnumerable<Dtos.ItemDto>> GetItems(string nameToMatch = null)
    {
        var items = (await _repository.GetItemsAsync())
            .Select(item => item.ToDto());

        
        if (!string.IsNullOrWhiteSpace(nameToMatch))
        {
            items = items.Where(x => x.Name.Contains(nameToMatch, StringComparison.OrdinalIgnoreCase));
        }
        return items;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Dtos.ItemDto>> GetItem(Guid id)
    {
        var item = await _repository.GetItemAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return item.ToDto();
    }

    [HttpPost]
    public async Task<ActionResult<Dtos.ItemDto>> CreateItem(Dtos.CreateItemDto itemDto)
    {
        Item item = new()
        {
            Id = Guid.NewGuid(),
            Name = itemDto.Name,
            Description = itemDto.Description,
            Price = itemDto.Price,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await _repository.CreateItemAsync(item);

        return CreatedAtAction(nameof(GetItem), new {id = item.Id}, item.ToDto());
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateItem(Guid id, Dtos.UpdateItemDto itemDto)
    {
        var existingItem = await _repository.GetItemAsync(id);

        if (existingItem is null)
            return NotFound();

        existingItem.Name = itemDto.Name;
        existingItem.Price = itemDto.Price;

        await _repository.UpdateItemAsync(existingItem);

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteItem(Guid id)
    {
        var existingItem = await _repository.GetItemAsync(id);

        if (existingItem is null)
            return NotFound();

        await _repository.DeleteItemAsync(id);

        return NoContent();
    }
}