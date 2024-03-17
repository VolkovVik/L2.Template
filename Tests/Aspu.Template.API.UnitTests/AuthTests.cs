using Aspu.L2.API.Controllers;
using FluentAssertions;

namespace Aspu.Template.API.UnitTests;

public class AuthTests
{
    [Test]
    public void PingTest()
    {
        var controller = new AuthController();

        var response = controller.Ping();

        response.Should().NotBeNull();
        response.IsSuccess.Should().BeTrue();
    }
}