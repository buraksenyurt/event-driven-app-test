namespace Employee.Test.Api;

public class InsuranceApiTests
{
    [Fact]
    public async Task Should_Post_A_New_Contract_Works_Test()
    {
        // Arange
        using var pw = await Playwright.CreateAsync();
        var apiContext = await pw.APIRequest.NewContextAsync(new()
        {
            BaseURL = "http://localhost:5011"
        });

        var payload = new Dictionary<string, object>()
        {
            { "title", "Klavyeme Kahve Döküldü Sigortası" },
            { "quantity", 17 }
        };

        // Act
        var request = await apiContext.PostAsync("contract", new() { DataObject = payload });

        // Assertion
        request.Ok.Should().Be(true);
    }
}