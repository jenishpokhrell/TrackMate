using FluentAssertions;
using Xunit;

namespace backend.Tests
{
    public class MathTests
    {
        [Fact]
        public void Add_ShouldReturnCorrectSum()
        {
            // Arrange

            int a = 5;
            int b = 3;

            // Act
            int result = a + b;

            //Assert
            result.Should().Be(8);
        }
    }
}