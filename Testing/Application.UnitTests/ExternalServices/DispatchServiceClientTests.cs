using System.Net;
using System.Net.Http.Json;
using SearchJobs.Api;
using SearchJobs.Api.Models;
using SearchJobs.Api.Models.Enums;

namespace Application.UnitTests;

public class DispatchServiceClientTests
{

    [Fact]
    public async Task GetAsync_ValidResponse_ReturnDeserializeBody()
    {
        var expectedBody = new PageResponseWithCursor<DispatchWriterDto>(
            Items: new List<DispatchWriterDto>{
                new DispatchWriterDto(Guid.NewGuid(), 100.00m, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), DispatchStatus.PendingPickup, []),
            },
            Cursor: "def"
        );

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedBody)
        };

        Console.WriteLine(response.ToString());
        Console.WriteLine(await response.Content.ReadAsStringAsync());

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.local/")
        };

        var serviceUnderTest = new DispatchServiceClient(httpClient);

        var result = await serviceUnderTest.GetAsync(cursor: "abc", auth: "test_token");

        Assert.Equal(expectedBody.Cursor, result.Cursor);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetAsync_NullResponseBody_ThrowsInvalidOperationException()
    {
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("null")
        };

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.local/")
        };

        var serviceUnderTest = new DispatchServiceClient(httpClient);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await serviceUnderTest.GetAsync(cursor: "abc", auth: "test_token"));
    }

    [Theory]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.NotFound)]
    public async Task GetAsync_NonSuccessStatusCode_ThrowsHttpRequestException(HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage
        {
            StatusCode = statusCode
        };

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.local/")
        };

        var serviceUnderTest = new DispatchServiceClient(httpClient);

        await Assert.ThrowsAsync<HttpRequestException>(async () => await serviceUnderTest.GetAsync(cursor: "abc", auth: "test_token"));
    }

    [Fact]
    public async Task GetAsync_DefaultLimit_SendLimitOf500()
    {
        var expectedBody = new PageResponseWithCursor<DispatchWriterDto>(
            Items: new List<DispatchWriterDto>
            {
                new DispatchWriterDto(Guid.NewGuid(), 100.00m, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), DispatchStatus.PendingPickup, [])
            },

            Cursor: "def"
        );

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedBody)
        };

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.local/")
        };

        var serviceUnderTest = new DispatchServiceClient(httpClient);
        await serviceUnderTest.GetAsync(cursor: "abc", auth: "Bearer auth_key");

        Console.WriteLine(handler.Request.RequestUri.Query);

        Assert.Contains("limit=500", handler.Request.RequestUri.Query);
    }

    [Fact]
    public async Task GetAsync_CustomLimit_SendLimitOf50()
    {
        var expectedBody = new PageResponseWithCursor<DispatchWriterDto>(
            Items: new List<DispatchWriterDto>
            {
                new DispatchWriterDto(Guid.NewGuid(), 100.00m, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), DispatchStatus.PendingPickup, [])
            },
            Cursor: "def"
        );


        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedBody)
        };

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.local/")
        };

        var serviceUnderTest = new DispatchServiceClient(httpClient);
        await serviceUnderTest.GetAsync(cursor: "abc", auth: "Bearer auth_key", limit: 50);

        Console.WriteLine(handler.Request.RequestUri.Query);

        Assert.Contains("limit=50", handler.Request.RequestUri.Query);
    }

    [Fact]
    public async Task GetAsync_ValidAuth_SendAuthorizationHeader()
    {
        var expectedBody = new PageResponseWithCursor<DispatchWriterDto>(
            Items: new List<DispatchWriterDto>
            {
                new DispatchWriterDto(Guid.NewGuid(), 100.00m, DateTime.UtcNow, DateTime.UtcNow.AddDays(1), DispatchStatus.PendingPickup, [])
            },
            Cursor: "def"
        );


        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = JsonContent.Create(expectedBody)
        };

        var handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://test.local/")
        };

        var serviceUnderTest = new DispatchServiceClient(httpClient);
        await serviceUnderTest.GetAsync(cursor: "abc", auth: "Bearer auth_key", limit: 50);

        Console.WriteLine(handler.Request.RequestUri.Query);

        Assert.Contains("auth_key", handler.Request.Headers.Authorization.Parameter);
        Assert.Contains("Bearer", handler.Request.Headers.Authorization.Scheme);
    }
}
