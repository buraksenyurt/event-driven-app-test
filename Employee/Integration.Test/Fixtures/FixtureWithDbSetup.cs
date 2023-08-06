using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using EmployeeService.Data;

namespace Integration.Test.Fixtures;

public class FixtureWithDbSetup<T> : WebApplicationFactory<T>
    where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        /*
            Test çalışma zamanı için bazı ayarlar yapılır.
            Örneğin DbContext nesnesi yakalanır ve InMemory versiyonu çalışma zamanına eklenir.
            Dolayısıyla gerçek veritabanına olan bağımlılık ortadan kalkar.
        */
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(descr => descr.ServiceType == typeof(DbContextOptions<EmployeeDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddDbContext<EmployeeDbContext>(options =>
            {
                options.UseInMemoryDatabase("EmployeeDb");
            });
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            using var appContext = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
            appContext.Database.EnsureDeleted();
            appContext.Database.EnsureCreated();

            var contract = new Contract
            {
                Id = 1,
                Title = "Klavyeme kahve döküldü sigortası",
                Quantity = 56,
                ContractId = Guid.NewGuid()
            };
            appContext.Contracts.Add(contract);
            appContext.SaveChanges();

        });
    }

}