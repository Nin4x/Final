using System.Net.Http.Headers;
using System.Net.Http.Json;
using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LoanApi.Api.IntegrationTests.Support;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly LoanApiWebApplicationFactory Factory = new();
    protected HttpClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        Client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        await Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        return Task.CompletedTask;
    }

    protected async Task<AuthResponse> RegisterAndLoginUserAsync(string username, string password)
    {
        var registerRequest = new RegisterRequest
        {
            Username = username,
            Email = $"{username}@example.com",
            Password = password
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        var registered = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Register response was null");

        return registered;
    }

    protected async Task<string> LoginAccountantAsync()
    {
        var loginRequest = new LoginRequest
        {
            UsernameOrEmail = "accountant",
            Password = "Accountant123!"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>()
            ?? throw new InvalidOperationException("Login response was null");

        return auth.AccessToken;
    }

    protected static void AddBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected static CreateLoanRequest CreateSampleLoanRequest(string borrower = "John Doe") => new()
    {
        BorrowerName = borrower,
        Amount = 1000,
        InterestRate = 5,
        PeriodMonths = 12,
        Currency = LoanCurrency.USD,
        Type = LoanType.Personal
    };
}
