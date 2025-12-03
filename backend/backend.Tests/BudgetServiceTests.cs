using AutoMapper;
using backend.Core.Interfaces.IRepositories;
using backend.Core.Interfaces.IServices;
using backend.Core.Services;
using backend.DataContext;
using backend.Dto.Budget;
using backend.Services.Helpers;
using backend.Services.Interfaces;
using backend.Services.Shared.Interfaces.IBudget;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace backend.Tests
{
    public class BudgetServiceTests
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

        public BudgetServiceTests()
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
        public void Test1_AddBudgetAsync_ShouldReturn201_WhenSuccessful()
        {
            //Arrange 
            var service = CreateService();

            var budgetDto = new AddBudgetDto
            {
                Amount = 1000
            };


        }
    }
}
