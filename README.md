# Passing tokens to a server-side Blazor application

Authenticate your application as you would do with a regular mvc/razor pages application.

Provision and save the tokens to the authentication cookie.

Define a class to pass in the initial settings for the application:

```csharp
namespace BlazorServerAuthWithAzureActiveDirectory.Data
{
    public class InitialApplicationState
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
```

Define a **scoped** service that can be used within the Blazor application to resolve the settings from DI:

```csharp
namespace BlazorServerAuthWithAzureActiveDirectory
{
    public class TokenProvider
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
```

On startup:

```csharp
services.AddScoped<TokenProvider>();
```

In *_Host cshtml*, create and instance of `InitialApplicationState` and pass that as a parameter to the app:

```cshtml
@{
    var tokens = new InitialApplicationState
    {
        AccessToken = await HttpContext.GetTokenAsync("access_token"),
        RefreshToken = await HttpContext.GetTokenAsync("refresh_token")
    };
}

<app>
    <component type="typeof(App)" param-InitialState="tokens" render-mode="ServerPrerendered" />
</app>
```

In the app component, resolve the service and initialize it with the data from the parameter:

```razor
@using BlazorServerAuthWithAzureActiveDirectory.Data
@inject TokenProvider TokenProvider

...

@code{
    [Parameter]
    public InitialApplicationState InitialState { get; set; }

    protected override Task OnInitializedAsync()
    {
        TokenProvider.AccessToken = InitialState.AccessToken;
        TokenProvider.RefreshToken = InitialState.RefreshToken;

        return base.OnInitializedAsync();
    }
}
```

On your service, inject the token provider and retrieve the token to call the API:

```csharp
public class WeatherForecastService
{
    private readonly TokenProvider _store;

    public WeatherForecastService(IHttpClientFactory clientFactory, TokenProvider tokenProvider)
    {
        Client = clientFactory.CreateClient();
        _store = tokenProvider;
    }

    public HttpClient Client { get; }

    public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
    {
        var token = _store.AccessToken;
        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5003/WeatherForecast");
        request.Headers.Add("Authorization", $"Bearer {token}");
        var response = await Client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsAsync<WeatherForecast[]>();
    }
}
```
