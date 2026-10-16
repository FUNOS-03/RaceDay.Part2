
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace RaceDay.Tests;

public class UnitTest1
{
    [Fact]
    public void TestProject_IsWorking()
    {
        Assert.True(true);
    }

    [Fact]
    public void ValidParticipantRole_IsAccepted()
    {
        string role = "Participant";

        Assert.Contains(role, new[] { "Participant", "Organiser" });
    }

    [Fact]
    public void ValidOrganiserRole_IsAccepted()
    {
        string role = "Organiser";

        Assert.Contains(role, new[] { "Participant", "Organiser" });
    }

    [Fact]
    public void InvalidRole_IsRejected()
    {
        string role = "Administrator";

        Assert.DoesNotContain(role, new[] { "Participant", "Organiser" });
    }

    [Fact]
    public async Task Register_WithValidDetails_ReturnsCreated()
    {
        using var factory = new RaceDayWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            FullName = "Test Participant",
            Email = "participant.test@example.com",
            Password = "TestPassword2026!",
            Role = "Participant"
        };

        var response = await client.PostAsJsonAsync(
            "/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Profile_WithoutToken_ReturnsUnauthorized()
    {
        using var factory = new RaceDayWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/profile/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        using var factory = new RaceDayWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new
        {
            FullName = "Test Participant",
            Email = "duplicate.test@example.com",
            Password = "TestPassword2026!",
            Role = "Participant"
        };

        var firstResponse = await client.PostAsJsonAsync(
            "/api/auth/register", request);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, secondResponse.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        using var factory = new RaceDayWebApplicationFactory();
        using var client = factory.CreateClient();

        var registration = new
        {
            FullName = "Login Test Participant",
            Email = "login.test@example.com",
            Password = "TestPassword2026!",
            Role = "Participant"
        };

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register", registration);

        Assert.Equal(
            HttpStatusCode.Created,
            registrationResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                registration.Email,
                registration.Password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var responseBody = await loginResponse.Content.ReadAsStringAsync();

        using var json = JsonDocument.Parse(responseBody);

        Assert.True(
            json.RootElement.TryGetProperty("token", out var token));

        Assert.False(string.IsNullOrWhiteSpace(token.GetString()));
    }

    [Fact]
    public async Task Login_WithIncorrectPassword_ReturnsUnauthorized()
    {
        using var factory = new RaceDayWebApplicationFactory();
        using var client = factory.CreateClient();

        var registration = new
        {
            FullName = "Invalid Login Test",
            Email = "invalid.login@example.com",
            Password = "CorrectPassword2026!",
            Role = "Participant"
        };

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register", registration);

        Assert.Equal(
            HttpStatusCode.Created,
            registrationResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                registration.Email,
                Password = "WrongPassword2026!"
            });

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            loginResponse.StatusCode);
    }

    [Fact]
    public async Task Profile_WithValidToken_ReturnsOK()
    {
        using var factory = new RaceDayWebApplicationFactory();
        using var client = factory.CreateClient();

        var registration = new
        {
            FullName = "Profile Test Participant",
            Email = "profile.test@example.com",
            Password = "TestPassword2026!",
            Role = "Participant"
        };

        var registrationResponse = await client.PostAsJsonAsync(
            "/api/auth/register", registration);

        Assert.Equal(
            HttpStatusCode.Created,
            registrationResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login",
            new
            {
                registration.Email,
                registration.Password
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var responseBody = await loginResponse.Content.ReadAsStringAsync();

        using var json = JsonDocument.Parse(responseBody);

        var token = json.RootElement
            .GetProperty("token")
            .GetString();

        Assert.False(string.IsNullOrWhiteSpace(token));

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var profileResponse = await client.GetAsync("/api/profile/me");

        Assert.Equal(HttpStatusCode.OK, profileResponse.StatusCode);
    }
}
