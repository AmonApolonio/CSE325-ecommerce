using System.Net.Http.Headers;
using Microsoft.JSInterop;

public class JwtHandler : DelegatingHandler
{
    private readonly IJSRuntime _jsRuntime;

    public JwtHandler(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Pega o token do LocalStorage via JSInterop
        var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        if (!string.IsNullOrEmpty(token))
        {
            // 2. Anexa o token ao cabeçalho Authorization
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}