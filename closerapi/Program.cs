using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using closerapi;

// Configure the client.
string urlBase = "https://closer.sandbox.colectica.org/";
using var client = new HttpClient();


// Request an API token, using our username and password.
var tokenRequest = new 
{
    username = "workshop@colectica.com",
    password = "CLOSERworkshop2022"
};
string requestJson = JsonSerializer.Serialize(tokenRequest);
var content = new StringContent(requestJson,
    Encoding.UTF8, 
    "application/json");
var response = await client.PostAsync(urlBase + "/token/CreateToken", content);
string responseStr = await response.Content.ReadAsStringAsync();


// Grab the token from the response.
var json = JsonDocument.Parse(responseStr);
string token = json.RootElement.GetProperty("access_token").GetString();
Console.WriteLine("Token:" + token);


// Set the token on our client, for use in all future calls.
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


// Instantiate a client, which was created based on the OpenAPI definition (swagger.json).
var service = new swaggerClient(urlBase, client);


// Call the /info/ API and output some values.
var info = await service.InfoAsync();

Console.WriteLine("Admin? " + info.CanAdministrator);
Console.WriteLine("Write? " + info.CanWrite);
Console.WriteLine("Read? " + info.CanRead);
Console.WriteLine("Title: " + info.Citation.Title.ToString());
