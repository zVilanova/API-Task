using APITask.Data;
using APITask.Services;
using APITask.DTOs;
using APITask.Models.Enums;
using APITask.Results;
using Microsoft.EntityFrameworkCore;
using APITask.Models;

namespace APITask.Tests.Services;

public class TaskServiceTests
{
    private AppDbContext CreateContext()
    {
        // Cada teste tem um banco isolado
        var options = new DbContextOptionsBuilder<AppDbContext>()
                       .UseInMemoryDatabase(Guid.NewGuid().ToString())
                       .Options;

        // Cria o contexto que será passado para TaskService
        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateTask()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        var createTaskDto = new CreateTaskDto
        {
            Title = "Teste",
            Description = "Descrição teste",
            Priority = TaskItemPriority.High
        };


        // Act => Executa a ação testada
        var result = await service.CreateTaskAsync(createTaskDto);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);

        // Como Assert.NotNull garante que Data não é null, usamos "!" para avisar ao compilador
        var data = result.Data!;

        Assert.Equal("Teste", data.Title);
        Assert.Equal("Descrição teste", data.Description);
        Assert.Equal("High", data.Priority);
        Assert.Equal(1, await context.Tasks.CountAsync());
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNotFound_WhenTaskDoesntExist()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        // Act => Executa a ação testada
        var result = await service.GetTaskByIdAsync(999);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Null(result.Data);
        Assert.Equal("Task não encontrada...", result.Message);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);
        var task = new TaskItem
        {
            Title = "Teste",
            Description = "Descrição teste",
            Priority = TaskItemPriority.High
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act => Executa a ação testada
        var result = await service.GetTaskByIdAsync(task.TaskItemId);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);

        var data = result.Data!;

        Assert.Equal("Teste", data.Title);
        Assert.Equal("Descrição teste", data.Description);
        Assert.Equal("High", data.Priority);
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnTasks()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Teste1",
                Description = "Descrição teste1",
                Priority = TaskItemPriority.High
            },

            new TaskItem
            {
                Title = "Teste2",
                Description = "Descrição teste2",
                Priority = TaskItemPriority.High
            }
        );

        await context.SaveChangesAsync();

        // Act => Executa a ação testada
        var result = await service.GetTasksAsync(null, null);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Count());
    }

    [Fact]
    public async Task GetTasksAsync_ShouldReturnEmptyList_WhenNoTasksExist()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        // Act => Executa a ação testada
        var result = await service.GetTasksAsync(null, null);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data!);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnValidationError_WhenStatusIsInvalid()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        var task = new TaskItem()
        {
            Title = "Teste",
            Description = "Descrição teste",
            Priority = TaskItemPriority.High
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var updateTaskDto = new UpdateTaskDto
        {
            Status = (TaskItemStatus)99
        };

        // Act => Executa a ação testada
        var result = await service.UpdateTaskAsync(task.TaskItemId, updateTaskDto);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.ValidationError, result.Status);
        Assert.Equal("Status inválido...", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldReturnValidationError_WhenPriorityIsInvalid()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        var task = new TaskItem()
        {
            Title = "Teste",
            Description = "Descrição teste",
            Priority = TaskItemPriority.High
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var updateTaskDto = new UpdateTaskDto
        {
            Priority = (TaskItemPriority)99
        };

        // Act => Executa a ação testada
        var result = await service.UpdateTaskAsync(task.TaskItemId, updateTaskDto);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.ValidationError, result.Status);
        Assert.Equal("Prioridade inválida...", result.Message);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateTaskAsync_ShouldUpdateTask_WhenDataIsValid()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        var task = new TaskItem()
        {
            Title = "Teste",
            Description = "Descrição teste",
            Status = TaskItemStatus.InProgress,
            Priority = TaskItemPriority.High
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var updateTaskDto = new UpdateTaskDto
        {
            Status = TaskItemStatus.Pending,
            Priority = TaskItemPriority.Low
        };

        // Act => Executa a ação testada
        var result = await service.UpdateTaskAsync(task.TaskItemId, updateTaskDto);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.NotNull(result.Data);

        var data = result.Data!;

        Assert.Equal("Teste", data.Title);
        Assert.Equal("Descrição teste", data.Description);
        Assert.Equal("Pending", data.Status);
        Assert.Equal("Low", data.Priority);
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldDeleteTask()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        var task = new TaskItem()
        {
            Title = "Teste",
            Description = "Descrição teste",
            Priority = TaskItemPriority.High
        };

        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        // Act => Executa a ação testada
        var result = await service.DeleteTaskAsync(task.TaskItemId);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.Success, result.Status);
        Assert.True(result.Data);
        Assert.Equal(0, await context.Tasks.CountAsync());
    }

    [Fact]
    public async Task DeleteTaskAsync_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange => Prepara o cenário
        using var context = CreateContext();
        var service = new TaskService(context);

        // Act => Executa a ação testada
        var result = await service.DeleteTaskAsync(999);

        // Assert => Verifica o resultado
        Assert.Equal(ServiceResultStatus.NotFound, result.Status);
        Assert.Equal("Task não encontrada...", result.Message);
        Assert.False(result.Data);
    }
}