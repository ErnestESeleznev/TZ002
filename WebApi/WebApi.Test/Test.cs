using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using Xunit;
using Assert = Xunit.Assert;

namespace WebApi.Test
{
    public class HomeControllerTests
    {
        [Fact]
        public void IndexViewResultNotNull()
        {
            // Arrange
            HomeController controller = new HomeController();
            // Act
            var result = controller.Index();
            // Assert
            Assert.NotNull(result);
        }
    }
}
