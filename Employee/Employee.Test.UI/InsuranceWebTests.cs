using Microsoft.Playwright;
using FluentAssertions;
using AutoFixture;

namespace Employee.Test.UI;

public class InsuranceWebTests
{
    // [Fact]
    // public async Task Should_New_Contract_Added_Works_Test()
    // {
    //     // Act
    //     using var pw = await Playwright.CreateAsync();
    //     await using var browser = await pw.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
    //     {
    //         Headless = false
    //     });
    //     var page = await browser.NewPageAsync();
    //     await page.GotoAsync("http://localhost:5131/");
    //     await page.ClickAsync("text=Poliçe Oluştur");
    //     await page.FillAsync("input[id=Title]", "Kafama saksı düştü sigortası");
    //     await page.FillAsync("input[id=Quantity]", "13");
    //     await page.ClickAsync("input[type=submit]");

    //     // Assertion
    //     await page.ClickAsync("text=Poliçeler");
    //     var result = await page.TextContentAsync(".table");
    //     result.Should().Contain("Kafama saksı düştü sigortası");
    // }

    // [Theory]
    // [InlineData("Telefonumun camı çatladı sigortası", "58")]
    // [InlineData("Cüzdanımı kaybettim sigortası", "17")]
    // [InlineData("Fazla oyun oynamaktan ekran kartım yandı sigortası", "4")]
    // public async Task Should_Contracts_Data_Set_Added_Works_Test(string title, string quantity)
    // {
    //     // Act
    //     using var pw = await Playwright.CreateAsync();
    //     await using var browser = await pw.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
    //     {
    //         Headless = false
    //     });
    //     var page = await browser.NewPageAsync();
    //     await page.GotoAsync("http://localhost:5131/");
    //     await page.ClickAsync("text=Poliçe Oluştur");
    //     await page.FillAsync("input[id=Title]", title);
    //     await page.FillAsync("input[id=Quantity]", quantity);
    //     await page.ClickAsync("input[type=submit]");

    //     // Assertion
    //     await page.ClickAsync("text=Poliçeler");
    //     var result = await page.TextContentAsync(".table");
    //     result.Should().Contain(title);
    // }

    [Fact]
    public async Task Should_Contracts_Data_Set_Added_Works_Test()
    {
        // Arange
        using var pw = await Playwright.CreateAsync();
        await using var browser = await pw.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var title = new Fixture().Create<string>();
        var quantity = new Fixture().Create<int>();

        var page = await browser.NewPageAsync();

        // Act
        await page.GotoAsync("http://localhost:5131/");
        await page.ClickAsync("text=Poliçe Oluştur");
        await page.FillAsync("input[id=Title]", title);
        await page.FillAsync("input[id=Quantity]", quantity.ToString());
        await page.ClickAsync("input[type=submit]");

        // Assertion
        await page.ClickAsync("text=Poliçeler");
        var result = await page.TextContentAsync(".table");
        result.Should().Contain(title);
    }
}