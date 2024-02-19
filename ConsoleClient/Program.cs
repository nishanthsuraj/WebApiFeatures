using IdentityModel.Client;


HttpClient client = new HttpClient();
DiscoveryDocumentResponse discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discoveryDocumentResponse.TokenEndpoint,
    ClientId = "m2m.client",
    ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
    Scope = "scope1"
});

Console.WriteLine($"Token: {tokenResponse.AccessToken}");
Console.ReadLine();
