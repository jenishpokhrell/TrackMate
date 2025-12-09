using AutoMapper;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services;
using backend.DataContext;
using backend.Dto.Budget;
using backend.Model;
using backend.Model.Dto.Shared;
using backend.Services.Helpers;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces.IBudget;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using Xunit;

namespace backend.Tests
{
    public class AddBudgetServiceTests
    {
        private readonly Mock<IBudgetRepository> _budgetRepositoryMock = new();
        private readonly Mock<IExpenseRepository> _expenseRepositoryMock = new();
        private readonly Mock<IBudgetCreationService> _budgetCreationServiceMock = new();
        private readonly Mock<IFindAccountGroupId> _findAccountGroupIdMock = new();
        private readonly Mock<IUserContextService> _userContextMock = new();
        private readonly Mock<INotificationService> _notificationServiceMock = new();
        private readonly Mock<ILogger<BudgetService>> _loggerMock = new();
        private readonly Mock<IMapper> _mapperMock = new();

        private readonly ApplicationDbContext _context;

        public AddBudgetServiceTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
        }

        private BudgetService CreateService()
        {
            return new BudgetService(_context,
                _mapperMock.Object,
                _budgetRepositoryMock.Object, 
                _expenseRepositoryMock.Object, 
                _budgetCreationServiceMock.Object,
                _findAccountGroupIdMock.Object,
                _userContextMock.Object, 
                _notificationServiceMock.Object, 
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Test1_AddBudgetAsync_ShouldReturn201_WhenSuccessful()
        {
            //Arrange
            var service = CreateService();

            var budgetDto = new AddBudgetDto
            {
                Amount = 1000
            };

            _budgetCreationServiceMock.Setup(x => x.AddBudgetAsync(It.IsAny<AddBudgetDto>())).Returns(Task.CompletedTask);
            _notificationServiceMock.Setup(x => x.NotificationAsync(It.IsAny<AddNotificationDto>(),
                It.IsAny<DbTransaction>())).ReturnsAsync(new Notification());

            //Act
            var result = await service.AddBudgetAsync(budgetDto);

            // Assert (Fluent Assertions)
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(201);
            result.Message.Should().Be("Budget Added Successfully.");

            _budgetCreationServiceMock.Verify(x => x.AddBudgetAsync(It.IsAny<AddBudgetDto>()), Times.Once);
            _notificationServiceMock.Verify(x => x.NotificationAsync(It.IsAny<AddNotificationDto>(), It.IsAny<DbTransaction>()), Times.Once);
        }

        [Fact]
        public async Task Test2_AddBudgetAsync_ShouldReturn400_WhenBudgetDtoIsNull()
        {
            var service = CreateService();

            var budget = await service.AddBudgetAsync(null);

            //Assert
            budget.Success.Should().BeFalse();
            budget.StatusCode.Should().Be(400);
            budget.Message.ToLower().Should().Contain("null");
        }

        [Fact]
        public async Task Test3_AddBudgetAsync_ShouldReturn500_WhenBudgetAddFails()
        {
            //Arrange
            var service = CreateService();

            var budgetDto = new AddBudgetDto
            {
                Amount = 1000
            };

            _budgetCreationServiceMock.Setup(x => x.AddBudgetAsync(It.IsAny<AddBudgetDto>())).ThrowsAsync(new Exception("Budget add failed"));

            //Act
            var result = await service.AddBudgetAsync(budgetDto);

            //Assert (Fluent Assertions)
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(500);
            result.Message.ToLower().Should().Contain("error");

            _notificationServiceMock.Verify(x => x.NotificationAsync(It.IsAny<AddNotificationDto>(), It.IsAny<DbTransaction>()), Times.Never);
        }
    }
}
