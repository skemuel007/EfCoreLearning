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

// -- select * from products;
Console.WriteLine("EF core: get all products with all columns ");
var customersEFCore = await db.Customers.ToListAsync();
foreach (var c in customersEFCore) Console.WriteLine($"Id - {c.CustomerID}, firstName - {c.FirstName}, lastName - {c.LastName} ");

Console.WriteLine();
var customerDapper = await conn.QueryAsync<Customer>(
    @"select * from customers");
foreach (var c in customerDapper) 
    Console.WriteLine($"Id - {c.CustomerID}, firstName - {c.FirstName}, lastName - {c.LastName} ");

/*
 * select orders.*,
       orderItems.itemPrice
   from orders
   join orderItems
   on orders.orderID = orderItems.orderID
*/

var resultEfCore1 = await db.Orders
    .Join(db.OrderItems,
        o => o.OrderID,
        oi => oi.OrderID,
        (o, oi) => new OrderWithItemPriceRow()
        {
            OrderID =  o.OrderID,
            CustomerID = o.CustomerID,
            OrderDate =  o.OrderDate,
            ShipAmount = o.ShipAmount,
            TaxAmount = o.TaxAmount,
        }).ToListAsync();

/*
 EF Core (if you add relationships) — SelectMany
   
   If Order has ICollection<OrderItem> Items:
   
var rows = await db.Orders
   .SelectMany(o => o.Items.Select(oi => new
   {
       Order = o,
       oi.ItemPrice
   }))
   .ToListAsync();
*/

var sql = @"
SELECT o.*, oi.itemPrice
FROM dbo.orders o
JOIN dbo.orderItems oi ON o.orderID = oi.orderID;";

var rows = await conn.QueryAsync(sql);

foreach (var r in rows)
{
    Console.WriteLine($"{r.orderID} - {r.itemPrice}");
}

/*
var sql = @"
SELECT 
    o.orderID      AS OrderID,
    o.customerID   AS CustomerID,
    o.orderDate    AS OrderDate,
    o.shipAmount   AS ShipAmount,
    o.taxAmount    AS TaxAmount,
    oi.itemPrice   AS ItemPrice
FROM dbo.orders o
JOIN dbo.orderItems oi ON o.orderID = oi.orderID;";

var rows = await conn.QueryAsync<OrderWithItemPriceRow>(sql);
*/