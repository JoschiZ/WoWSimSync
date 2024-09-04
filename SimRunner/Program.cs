using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Polly;
using Polly.Retry;
using SimRunner;
using SimRunner.Configs;


var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var pipeline = new ResiliencePipelineBuilder()
    .AddRetry(new RetryStrategyOptions()
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        Delay = TimeSpan.FromSeconds(20),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        MaxRetryAttempts = 4
    })
    .Build();

var auditConfig = config.GetRequiredSection("Audit").Get<AuditConfig>();

if (auditConfig is null)
{
    throw new Exception("Audit configuration is missing");
}
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://wowaudit.com/");
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auditConfig.ApiKey);


var roster = await httpClient.GetFromJsonAsync<List<GetCharacterResponse>>("v1/characters");
if (roster is null)
{
    throw new Exception("Roster is missing");
}

var file = File.OpenRead("input.json");
var entriesDictionary = await JsonSerializer.DeserializeAsync<Dictionary<string, AddonOutputEntry>>(file);
var entries = entriesDictionary.Select(x => x.Value).ToList();
using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
var context = await browser.NewContextAsync();

var loginPage = await context.NewPageAsync();

await loginPage.GotoAsync($"https://www.raidbots.com/auth");
await loginPage.WaitForURLAsync("https://www.raidbots.com/simbot", new PageWaitForURLOptions(){Timeout = 0});
List<Task<(AddonOutputEntry, string)>> getUrlTasks = [];
foreach (var character in entries)
{
    if (roster
        .Where(x => x.Name == character.CharacterName)
        .All(x => x.Realm != character.Realm))
    {
        continue;
    }
    
    getUrlTasks.Add(GetReportPage(context, character));
}

List<Task> uploadTasks = [];
while (getUrlTasks.Count > 0)
{
    var task = await Task.WhenAny(getUrlTasks);
    getUrlTasks.Remove(task);
    var (addonOutputEntry, reportId) = await task;
    var character = roster
        .Where(x => x.Name == addonOutputEntry.CharacterName)
        .First(x => x.Realm == addonOutputEntry.Realm); 
    uploadTasks.Add(UploadReport(httpClient, reportId, addonOutputEntry, character.Id));
}

await Task.WhenAll(uploadTasks);
return;



async Task<(AddonOutputEntry Character, string reportId)> GetReportPage(IBrowserContext browserContext, AddonOutputEntry character)
{
    var page = await browserContext.NewPageAsync();
    await page.GotoAsync($"https://www.raidbots.com/simbot/droptimizer");
    await page.Locator("#SimcUserInput-input").FillAsync(character.SimCString);
    await page.GetByText("Nerub-ar Palace", new PageGetByTextOptions { Exact = true }).ClickAsync();
    await Task.Delay(TimeSpan.FromSeconds(2)); // Raidbots is sometimes a bit finnicky with this button so we wait a bit
    await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions() {Name = "RUN DROPTIMIZER"}).ClickAsync();
    try
    {
        await page.WaitForURLAsync("**/simbot/report/*");
    }
    catch (TimeoutException)
    {
        await page.GetByRole(AriaRole.Button, new PageGetByRoleOptions() {Name = "RUN DROPTIMIZER"}).ClickAsync();
        await page.WaitForURLAsync("**/simbot/report/*");
    }
    
    var uri = new Uri(page.Url);
    try
    {
        await page.Locator("#Report-backLink").ClickAsync(new LocatorClickOptions(){Timeout = TimeSpan.FromMinutes(10).Milliseconds});
    }
    catch (TimeoutException)
    {
        
    }
    
    
    return (character, uri.Segments[^1]);
}

async Task UploadReport(HttpClient client, string reportId, AddonOutputEntry character, int characterId)
{
    var request = new UploadDroptimizerReportRequest(reportId, characterId, character.CharacterName);
    try
    {
        var response = await pipeline.ExecuteAsync(async ct =>
        {
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Try to upload report for: {character.CharacterName}");
            var response = await client.PostAsJsonAsync("v1/wishlists", request, cancellationToken: ct);
            if (response.StatusCode == HttpStatusCode.NotAcceptable)
            {
                throw new Exception($"{DateTime.Now.ToShortTimeString()}: Can't upload report for: {character.CharacterName}");
            }
            return response;
        });
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"{DateTime.Now.ToShortTimeString()}: Updated " + character.CharacterName);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Failed to upload report for: {character.CharacterName}");
        Console.WriteLine(e);
    }



}