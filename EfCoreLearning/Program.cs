// See https://aka.ms/new-console-template for more information

using System.Data;
using Dapper;
using EfCoreLearning.db;
using EfCoreLearning.models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(cfg =>
    {
        cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((ctx, services) =>
    {
        var cs = ctx.Configuration.GetConnectionString("Eshop")
                 ?? throw new InvalidOperationException("Missing ConnectionStrings:Eshop");

        services.AddDbContext<EshopDbContext>(opt => opt.UseSqlServer(cs));
        services.AddScoped<IDbConnection>(_ => new SqlConnection(cs));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<EshopDbContext>();
var conn = scope.ServiceProvider.GetRequiredService<IDbConnection>();

Console.WriteLine("--Selecting with conditions");
Console.WriteLine("SQL: select * from dbo.Customers where emailAddress = 'barryz@gmail.com'");

Console.WriteLine();
Console.WriteLine("EfCore implementation...");
Console.WriteLine("Basic EfCore Where condition");

var customer = await db.Customers.AsNoTracking()
    .FirstOrDefaultAsync(c => c.EmailAddress == "barryz@gmail.com");

if (customer is not null)
{
    Console.WriteLine($"Customer full name: {customer.FirstName} {customer.LastName}");
}
 
customer = await db.Customers.AsNoTracking()
    .Where(c => c.EmailAddress == "barryz@gmail.com")
    .Select(c => new Customer()
    {
        BillingAddressID = c.BillingAddressID,
        EmailAddress = c.EmailAddress,
        CustomerID = c.CustomerID,
        FirstName = c.FirstName,
        LastName = c.LastName,
        ShipAddressID = c.ShipAddressID,
    }).FirstOrDefaultAsync();

if (customer is not null)
{
    Console.WriteLine($"Customer full name: {customer.FirstName} {customer.LastName}");
}

var customers = await db.Customers.AsNoTracking()
    .Where(c => c.EmailAddress == "barryz@gmail.com")
    .ToListAsync();

if (customers.Any())
{
    customer = customers[0];
}

if (customer is not null)
{
    Console.WriteLine($"Customer full name: {customer.FirstName} {customer.LastName}");
}

// where condition with dapper
var sql = @"
SELECT *
FROM dbo.customers
WHERE emailAddress = @Email;
";

customer = await conn.QueryFirstOrDefaultAsync(sql,
    new { Email = "barryz@gmail.com" });
    
if (customer is not null)
{
    Console.WriteLine($"Customer full name: {customer.FirstName} {customer.LastName}");
}

// conditions with multiple fields (and/or)
// using ef core
customers = await db.Customers
    .Where(c => c.FirstName == "Barry" && c.LastName == "Zimmer")
    .ToListAsync();

if (customers.Any())
{
    customer = customers[0];
}

if (customer is not null)
{
    Console.WriteLine($"Customer full name: {customer.FirstName} {customer.LastName}");
}
    
customers = await db.Customers
    .Where(c => c.FirstName == "Barry" || c.LastName == "Zimmer")
    .ToListAsync();

if (customers.Any())
{
    customer = customers[0];
}

if (customer is not null)
{
    Console.WriteLine($"Customer full name: {customer.FirstName} {customer.LastName}");
}
    
sql = @"
SELECT *
FROM dbo.customers
WHERE firstName = @FirstName
  AND lastName = @LastName;
";

var results = await conn.QueryAsync(sql, new
{
    FirstName = "Barry",
    LastName = "Zimmer"
});

foreach (var result in results)
{
    Console.WriteLine($"Customer full name: {result.FirstName} {result.LastName}");
}
// In query
var ids = new[] { 1, 3, 5 };

customers = await db.Customers
    .Where(c => ids.Contains(c.CustomerID))
    .ToListAsync();

foreach (var result in customers)
{
    Console.WriteLine($"Customer full name: {result.FirstName} {result.LastName}");
}

sql = @"
SELECT *
FROM dbo.customers
WHERE customerID IN @Ids;
";

var dapperResultSets = await conn.QueryAsync(sql, new { Ids = new[] { 1, 3, 5 } });

foreach (var result in dapperResultSets)
{
    Console.WriteLine($"Customer full name: {result.FirstName} {result.LastName}");
}

// like query in ef core and dapper
customers = await db.Customers
    .Where(c => c.EmailAddress.Contains("gmail"))
    .ToListAsync();

foreach (var result in customers)
{
    Console.WriteLine($"Customer full name: {result.FirstName} {result.LastName}");
}

sql = @"
SELECT *
FROM dbo.customers
WHERE emailAddress LIKE @Pattern;
";

var likeDapperResultSet = await conn.QueryAsync(sql,
    new { Pattern = "%gmail%" });

foreach (var result in likeDapperResultSet)
{
    Console.WriteLine($"Customer full name: {result.FirstName} {result.LastName}");
}

/*
 * Conditional with optional filters dynamic filter
 * var query = db.Orders.AsQueryable();
   
   if (minAmount.HasValue)
       query = query.Where(o => o.ShipAmount >= minAmount.Value);
   
   if (maxAmount.HasValue)
       query = query.Where(o => o.ShipAmount <= maxAmount.Value);
   
   var orders = await query.ToListAsync();
   
   // dapper
   
   var sql = @"SELECT * FROM dbo.orders WHERE 1 = 1";
   var parameters = new DynamicParameters();
   
   if (minAmount.HasValue)
   {
       sql += " AND shipAmount >= @MinAmount";
       parameters.Add("MinAmount", minAmount);
   }
   
   if (maxAmount.HasValue)
   {
       sql += " AND shipAmount <= @MaxAmount";
       parameters.Add("MaxAmount", maxAmount);
   }
   
   var orders = await connection.QueryAsync(sql, parameters);
 */

