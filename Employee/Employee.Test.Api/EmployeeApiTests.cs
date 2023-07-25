using EmployeeService.Data;

namespace Employee.Test.Api;

public class EmployeeApiTests
{
    [Fact]
    public async Task Should_Get_All_Contracts_Works_Test()
    {
        // Arange
        using var pw = await Playwright.CreateAsync();
        var apiContext = await pw.APIRequest.NewContextAsync(new()
        {
            BaseURL = "http://localhost:5226"
        });

        // Act
        var response = await apiContext.GetAsync("contracts");
        var result = await response.JsonAsync();
        //Console.WriteLine("Result = {0}", result.Value);
        var contracts = result?.Deserialize<List<Contract>>(new JsonSerializerOptions
        {
            // Bunu eklemezsek Playwright Deserialize operasyonu özellik adlarını küçük harf olarak ele alır 
            // ve çalışma zamanında Null Reference Exception'a düşeriz.
            PropertyNameCaseInsensitive = true
        });

        // Assertion
        using (new AssertionScope())
        {
            contracts.FirstOrDefault(c => c.Id == 1)?.Title.Should().Be("Kitaplık koruma sigortası");
            contracts.FirstOrDefault(c => c.Id == 1)?.Quantity.Should().Be(13);
        }
    }
}