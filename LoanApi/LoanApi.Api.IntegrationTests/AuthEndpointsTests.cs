using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LoanApi.Api.IntegrationTests.Support;
using LoanApi.Application.DTOs;
using Xunit;

namespace LoanApi.Api.IntegrationTests;

public class AuthEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task Register_and_login_should_issue_tokens()
    {
        var username = $"user_{Guid.NewGuid():N}";
        var password = "Password123!";

        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = $"{username}@example.com",
            Password = password
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var registerBody = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
        registerBody.Should().NotBeNull();
        registerBody!.AccessToken.Should().NotBeNullOrWhiteSpace();

        var loginRequest = new LoginRequest
        {
            UsernameOrEmail = username,
            Password = password
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        loginBody.Should().NotBeNull();
        loginBody!.AccessToken.Should().NotBeNullOrWhiteSpace();
        loginBody.User.Username.Should().Be(username);
    }
}
