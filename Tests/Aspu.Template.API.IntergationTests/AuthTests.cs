using Aspu.Template.Application.Application.Auth.Queries;
using Aspu.Template.Domain.Common;
using FluentAssertions;

namespace Aspu.Template.API.IntergationTests;

public class AuthTests : ApiIntegrationTests
{
    [Test]
    public async Task PingTest()
    {
        // Arrange

        // Act
        var response = await GetRequest<Result>("/api/ping");

        // Assert
        response.Should().NotBeNull();
        response!.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task LoginTest()
    {
        // Arrange
        var body = new GetTokenQuery { UserName = "admin", Password = "admin" };

        // Act
        var response = await PostRequest<Result<GetTokenQueryResponse?>>("/api/login", body);

        // Assert
        response.Should().NotBeNull();
        response!.IsSuccess.Should().BeTrue();
    }
}
