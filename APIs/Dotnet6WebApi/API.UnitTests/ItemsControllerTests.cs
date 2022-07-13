using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotnet7WebApi;
using Dotnet7WebApi.Controllers;
using Dotnet7WebApi.Entities;
using Dotnet7WebApi.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace API.UnitTests;

public class ItemControllerTests
{
    private readonly Mock<IItemsRepository> repositoryStub = new();
    private readonly Random rand = new();

    [Fact]
    public async Task GetItem_WithUnexistingItem_ReturnsNotFound()
    {
        //Arrage
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Item) null!);

        var controller = new ItemsController(repositoryStub.Object);

        //Act

        var result = await controller.GetItem(Guid.NewGuid());

        //Assert

        result.Result.Should().BeOfType<NotFoundResult>();

        // Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetItem_WithexistingItem_ReturnExpectedItem()
    {
        //Arrage
        var expectedItem = CreateRandomItem();

        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);
        var controller = new ItemsController(repositoryStub.Object);

        //Act

        var result = await controller.GetItem(Guid.NewGuid());

        //Assert

        //With FluentAssertions


        result.Value.Should().BeEquivalentTo(expectedItem);

        /* Without FluentAssertions
        Assert.IsType<ItemDto>(result.Value);
        var dto = (result as ActionResult<ItemDto>).Value;
        Assert.Equal(expectedItem.Id, dto.Id);
        Assert.Equal(expectedItem.Name, dto.Name);
        Assert.Equal(expectedItem.Price, dto.Price);
        */
    }

    [Fact]
    public async Task GetItems_WithExistingItems_ReturnsAllItems()
    {
        //Arrange
        var expectedItems = new[] {CreateRandomItem(), CreateRandomItem(), CreateRandomItem()};

        repositoryStub.Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(expectedItems);
        //Act
        var controller = new ItemsController(repositoryStub.Object);

        var actualItems = await controller.GetItems();

        //Assert

        actualItems.Should().BeEquivalentTo(expectedItems);
    }

    [Fact]
    public async Task GetItems_WithMatchingItems_ReturnsMatchingItems()
    {
        //Arrange
        var allItems = new[]
        {
            new Item() {Name = "Potion"},
            new Item() {Name = "Axe"},
            new Item() {Name = "BigAxe"}
        };

        var nameToMatch = "Potion";

        repositoryStub.Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(allItems);

        var controller = new ItemsController(repositoryStub.Object);

        //Act
        IEnumerable<Dtos.ItemDto> foundItems = await controller.GetItems(nameToMatch);

        //Assert

        foundItems.Should().OnlyContain(
            item => item.Name == allItems[0].Name || item.Name == allItems[2].Name);
    }

    [Fact]
    public async Task CreateItem_WithItemToCreate_ReturnsCreatedItem()
    {
        //Arrange
        var itemToCreate =
            new Dtos.CreateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), rand.Next(1000));

        var controller = new ItemsController(repositoryStub.Object);
        //Act

        var result = await controller.CreateItem(itemToCreate);


        //Assert

        var createdItem = (result.Result as CreatedAtActionResult).Value as Dtos.ItemDto;
        itemToCreate.Should().BeEquivalentTo(
            createdItem,
            options => options.ComparingByMembers<Dtos.ItemDto>()
                .ExcludingMissingMembers());
        createdItem.Id.Should().NotBeEmpty();
        createdItem.CreatedDate.Should().NotBeCloseTo(DateTimeOffset.UtcNow, new TimeSpan(5000));
    }

    [Fact]
    public async Task UpdateItem_WithExistingItem_NoContent()
    {
        //Arrange

        Item existingItem = CreateRandomItem();
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);

        var itemId = existingItem.Id;
        var itemToUpdate =
            new Dtos.UpdateItemDto(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), existingItem.Price + 3);

        var controller = new ItemsController(repositoryStub.Object);

        //Act

        var result = await controller.UpdateItem(itemId, itemToUpdate);

        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task DeleteItem_WithExistingItem_ReturnNoContent()
    {
        //Arrange
        Item existingItem = CreateRandomItem();
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);

        var controller = new ItemsController(repositoryStub.Object);

        //Act

        var result = await controller.DeleteItem(existingItem.Id);


        //Assert
        result.Should().BeOfType<NoContentResult>();
    }

    private Item CreateRandomItem()
    {
        return new Item
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = rand.Next(1000),
            CreatedDate = DateTimeOffset.UtcNow
        };
    }
}