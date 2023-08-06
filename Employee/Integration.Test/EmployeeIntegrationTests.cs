using Integration.Test.Fixtures;
using EmployeeService;
using Xunit.Abstractions;

namespace Integration.Test;

/*
    Test çalışma zamanı ayarlanırken EmployeeService uygulamasındaki Program sınıfı baz alınır.
    Buna göre client nesnesi oluşturulduğunda EmployeeService içindeki Program sınıfının Main
    metoduna yapılan çağrı ile ayağa kalkan Controller ele alınır.
*/
public class EmployeeIntegrationTests
    : IClassFixture<FixtureWithDbSetup<Program>>
{
    private readonly FixtureWithDbSetup<Program> _fixtureWithDbSetup;
    private readonly ITestOutputHelper _testOutputHelper;
    public EmployeeIntegrationTests(FixtureWithDbSetup<Program> fixtureWithDbSetup, ITestOutputHelper testOutputHelper)
    {
        _fixtureWithDbSetup = fixtureWithDbSetup;
        _testOutputHelper = testOutputHelper;
    }
    [Fact]
    public async void Should_Get_All_Contracts_From_InMemory_Works_Test()
    {
        using var client = _fixtureWithDbSetup.CreateDefaultClient();
        var response = client.GetAsync("/contracts").GetAwaiter().GetResult().Content;
        var result = await response.ReadAsStringAsync();
        _testOutputHelper.WriteLine(result);
    }
}