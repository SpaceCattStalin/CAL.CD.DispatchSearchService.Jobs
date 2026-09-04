namespace Application.UnitTests;
/// <summary>
/// Fake class for <see cref="HttpMessageHandler"/> that always returns the same
/// response, regardless of the incoming request. Inject it into an
/// <see cref="HttpClient"/> to fake HTTP calls in tests without calling the real network.
/// </summary>
/// <param name="response">The response every request through the client will receive.</param>
public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public HttpRequestMessage Request { get; private set; }
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Request = request;
        return Task.FromResult<HttpResponseMessage>(response);
    }
}
