using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using FluentAssertions;
using LoanApi.Api.IntegrationTests.Support;
using LoanApi.Application.DTOs;
using LoanApi.Domain.Enums;
using Xunit;

namespace LoanApi.Api.IntegrationTests;

public class AccountantPrivilegesTests : IntegrationTestBase
{
    [Fact]
    public async Task Accountant_can_view_and_update_loans()
    {
        var userAuth = await RegisterAndLoginUserAsync($"user_{Guid.NewGuid():N}", "Password123!");
        AddBearerToken(Client, userAuth.AccessToken);
        var createResponse = await Client.PostAsJsonAsync("/api/loans", CreateSampleLoanRequest());
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanResponse>();
        createdLoan.Should().NotBeNull();

        var accountantToken = await LoginAccountantAsync();
        AddBearerToken(Client, accountantToken);

        var getAllResponse = await Client.GetAsync("/api/loans");
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loans = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<LoanResponse>>();
        loans.Should().NotBeNull();
        loans!.Should().ContainSingle(loan => loan.Id == createdLoan!.Id && loan.User.Id == userAuth.User.Id);

        var updateStatusRequest = new UpdateLoanStatusRequest { Status = LoanStatus.Approved };
        var statusResponse = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Patch, $"/api/accountant/loans/{createdLoan.Id}/status")
        {
            Content = JsonContent.Create(updateStatusRequest)
        });

        statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await statusResponse.Content.ReadFromJsonAsync<LoanResponse>();
        updated.Should().NotBeNull();
        updated!.Status.Should().Be(LoanStatus.Approved);
    }
}
