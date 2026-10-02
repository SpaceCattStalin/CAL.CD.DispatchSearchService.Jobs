using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using SearchJobs.Api;
using SearchJobs.Api.Models;

namespace Application.UnitTests;

public class DispatchSearchServiceClientTests
{
    private static DispatchSearchServiceClient CreateSut(HttpResponseMessage response, out FakeHttpMessageHandler handler)
    {
        handler = new FakeHttpMessageHandler(response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://test.local/") };
        return new DispatchSearchServiceClient(httpClient, Mock.Of<ILogger<DispatchSearchServiceClient>>());
    }

    [Fact]
    public async Task IndexAsync_SuccessResponse_ReturnsId()
    {
        var id = Guid.NewGuid().ToString();
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent($$"""{"id":"{{id}}"}""", Encoding.UTF8, "application/json")
        };
        var sut = CreateSut(response, out _);

        var result = await sut.IndexAsync(new DispatchModel { DispatchId = Guid.NewGuid() }, CancellationToken.None);

        Assert.Equal(id, result);
    }

    [Fact]
    public async Task IndexAsync_NullResponseBody_ReturnsNull()
    {
        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };
        var sut = CreateSut(response, out _);

        var result = await sut.IndexAsync(new DispatchModel { DispatchId = Guid.NewGuid() }, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task IndexAsync_NonSuccessStatusCode_ReturnsEmptyStringInsteadOfThrowing()
    {
        // Documents current behavior: IndexAsync's generic catch swallows EnsureSuccessStatusCode's
        // HttpRequestException (and everything else except InvalidOperationException) and returns
        // an empty string rather than letting the failure propagate to the caller.
        var sut = CreateSut(new HttpResponseMessage(HttpStatusCode.InternalServerError), out _);

        var result = await sut.IndexAsync(new DispatchModel { DispatchId = Guid.NewGuid() }, CancellationToken.None);

        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public async Task DeleteAsync_NotFoundStatusCode_CompletesWithoutThrowing()
    {
        var sut = CreateSut(new HttpResponseMessage(HttpStatusCode.NotFound), out _);

        var exception = await Record.ExceptionAsync(() => sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task DeleteAsync_NonSuccessStatusCode_SwallowsExceptionAndCompletes()
    {
        var sut = CreateSut(new HttpResponseMessage(HttpStatusCode.InternalServerError), out _);

        var exception = await Record.ExceptionAsync(() => sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task UpdateAsync_NonSuccessStatusCode_SwallowsExceptionAndCompletes()
    {
        var sut = CreateSut(new HttpResponseMessage(HttpStatusCode.InternalServerError), out _);

        var exception = await Record.ExceptionAsync(
            () => sut.UpdateAsync(new DispatchModel { DispatchId = Guid.NewGuid() }, CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task BatchUpsertAsync_Success_SendsPutRequestToBatchUpdateEndpoint()
    {
        var sut = CreateSut(new HttpResponseMessage(HttpStatusCode.OK), out var handler);

        await sut.BatchUpsertAsync([new DispatchModel { DispatchId = Guid.NewGuid() }]);

        Assert.Equal(HttpMethod.Put, handler.Request.Method);
        Assert.Equal("/api/dispatch/batch-update", handler.Request.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task BatchUpsertAsync_NonSuccessStatusCode_ThrowsHttpRequestException()
    {
        // Unlike Index/Delete/UpdateAsync, this method has no try/catch, so a failure
        // propagates to the caller instead of being silently swallowed.
        var sut = CreateSut(new HttpResponseMessage(HttpStatusCode.InternalServerError), out _);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => sut.BatchUpsertAsync([new DispatchModel { DispatchId = Guid.NewGuid() }]));
    }
}
