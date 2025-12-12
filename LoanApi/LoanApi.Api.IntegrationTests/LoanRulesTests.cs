using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using FluentAssertions;
using LoanApi.Api.IntegrationTests.Support;
using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;
using Xunit;

namespace LoanApi.Api.IntegrationTests;

public class LoanRulesTests : IntegrationTestBase
{
    [Fact]
    public async Task Blocked_user_cannot_create_loan()
    {
        var userAuth = await RegisterAndLoginUserAsync($"user_{Guid.NewGuid():N}", "Password123!");
        var accountantToken = await LoginAccountantAsync();

        AddBearerToken(Client, accountantToken);
        var blockRequest = new BlockUserRequest { IsBlocked = true };
        var blockResponse = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, $"/api/accountant/users/{userAuth.User.Id}/block")
        {
            Content = JsonContent.Create(blockRequest)
        });
        blockResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        AddBearerToken(Client, userAuth.AccessToken);
        var createResponse = await Client.PostAsJsonAsync("/api/loans", CreateSampleLoanRequest());

        createResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task User_cannot_update_non_processing_loan()
    {
        var userAuth = await RegisterAndLoginUserAsync($"user_{Guid.NewGuid():N}", "Password123!");
        AddBearerToken(Client, userAuth.AccessToken);

        var createdResponse = await Client.PostAsJsonAsync("/api/loans", CreateSampleLoanRequest());
        createdResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdLoan = await createdResponse.Content.ReadFromJsonAsync<LoanResponse>();
        createdLoan.Should().NotBeNull();

        var accountantToken = await LoginAccountantAsync();
        AddBearerToken(Client, accountantToken);
        var statusRequest = new UpdateLoanStatusRequest { Status = LoanStatus.Approved };
        var statusResponse = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, $"/api/accountant/loans/{createdLoan!.Id}/status")
        {
            Content = JsonContent.Create(statusRequest)
        });
        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        AddBearerToken(Client, userAuth.AccessToken);
        var updateRequest = new UpdateLoanRequest
        {
            BorrowerName = "Updated Borrower",
            Amount = 2000,
            InterestRate = 7,
            PeriodMonths = 18,
            Currency = LoanCurrency.EUR,
            Type = LoanType.Business
        };

        var updateResponse = await Client.PutAsJsonAsync($"/api/loans/{createdLoan.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task User_cannot_access_another_users_loan()
    {
        var firstUser = await RegisterAndLoginUserAsync($"user_{Guid.NewGuid():N}", "Password123!");
        AddBearerToken(Client, firstUser.AccessToken);
        var createResponse = await Client.PostAsJsonAsync("/api/loans", CreateSampleLoanRequest());
        createResponse.EnsureSuccessStatusCode();
        var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();

        var secondUser = await RegisterAndLoginUserAsync($"user_{Guid.NewGuid():N}", "Password123!");
        AddBearerToken(Client, secondUser.AccessToken);

        var getResponse = await Client.GetAsync($"/api/loans/{createdLoan!.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
