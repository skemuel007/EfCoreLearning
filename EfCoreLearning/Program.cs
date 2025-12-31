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
   
   var resultEfCoreAnonymousJoinProjection = await db.Orders
   .Join(db.OrderItems,
       o => o.OrderID,
       oi => oi.OrderID,
       (o, oi) => new
       {
           // orders.*
           o.OrderID,
           o.CustomerID,
           o.OrderDate,
           o.ShipAmount,
           o.TaxAmount,
           o.ShipDate,
           o.ShipAddressID,
           o.BillingAddressID,
           o.CardType,
           o.CardNumber,
           o.CardExpires,

           // orderItems.itemPrice
           ItemPrice = oi.ItemPrice
       })
   .ToListAsync();
*/

var resultEfCoreJoinProjection = await db.Orders
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

foreach (var item in resultEfCoreJoinProjection)
    Console.WriteLine($"Order Id: {item.OrderID}");
/*
 EF Core (if you add relationships) — SelectMany
   
   If Order has ICollection<OrderItem> Items:
   
var resultEfCoreRelationshipSelectMany = await db.Orders
   .SelectMany(o => o.Items.Select(oi => new
   {
       Order = o,
       oi.ItemPrice
   }))
   .ToListAsync();
*/

var dapperDynamicRowsSql = @"
SELECT o.*, oi.itemPrice
FROM dbo.orders o
JOIN dbo.orderItems oi ON o.orderID = oi.orderID;";

var dapperDynamicRowsResultSet = await conn.QueryAsync(dapperDynamicRowsSql);

foreach (var r in dapperDynamicRowsResultSet)
{
    Console.WriteLine($"{r.orderID} - {r.itemPrice}");
}


var dapperStronglyTypedSql = @"
SELECT 
    o.orderID      AS OrderID,
    o.customerID   AS CustomerID,
    o.orderDate    AS OrderDate,
    o.shipAmount   AS ShipAmount,
    o.taxAmount    AS TaxAmount,
    oi.itemPrice   AS ItemPrice
FROM dbo.orders o
JOIN dbo.orderItems oi ON o.orderID = oi.orderID;";

var dapperStronglyTypedResultSet = await conn.QueryAsync<OrderWithItemPriceRow>(dapperStronglyTypedSql);

foreach (var r in dapperDynamicRowsResultSet)
{
    Console.WriteLine($"{r.orderID} - {r.itemPrice}");
}

// Ef core nested relationship assumming the following all commented
/*
public sealed class Order
   {
       public int OrderID { get; set; }
       public int CustomerID { get; set; }
       public DateTime OrderDate { get; set; }
       public decimal ShipAmount { get; set; }
       public decimal TaxAmount { get; set; }
   
       public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
   }
   
   public sealed class OrderItem
   {
       public int ItemID { get; set; }
       public int OrderID { get; set; }
       public int ProductID { get; set; }
       public decimal ItemPrice { get; set; }
       public decimal DiscountAmount { get; set; }
       public int Quantity { get; set; }
   
       public Order Order { get; set; } = null!;
   }
   
   // fluent api
   modelBuilder.Entity<Order>()
       .HasMany(o => o.Items)
       .WithOne(i => i.Order)
       .HasForeignKey(i => i.OrderID);
   
   modelBuilder.Entity<OrderItem>()
       .HasOne(i => i.Order)
       .WithMany(o => o.Items)
       .HasForeignKey(i => i.OrderID);
       
  var orders = await db.Orders
   .AsNoTracking()
   .Select(o => new OrderDto
   {
       OrderID = o.OrderID,
       CustomerID = o.CustomerID,
       OrderDate = o.OrderDate,
       ShipAmount = o.ShipAmount,
       TaxAmount = o.TaxAmount,
       Items = db.OrderItems
           .Where(oi => oi.OrderID == o.OrderID)
           .Select(oi => new OrderItemDto
           {
               ItemID = oi.ItemID,
               ProductID = oi.ProductID,
               ItemPrice = oi.ItemPrice,
               Quantity = oi.Quantity
           })
           .ToList()
   })
   .ToListAsync();
   
   public sealed class OrderDto
   {
       public int OrderID { get; set; }
       public int CustomerID { get; set; }
       public DateTime OrderDate { get; set; }
       public decimal ShipAmount { get; set; }
       public decimal TaxAmount { get; set; }
       public List<OrderItemDto> Items { get; set; } = new();
   }
   
   public sealed class OrderItemDto
   {
       public int ItemID { get; set; }
       public int ProductID { get; set; }
       public decimal ItemPrice { get; set; }
       public int Quantity { get; set; }
   }
   
      // dapper nested relationship
    public sealed class Order
   {
       public int OrderID { get; set; }
       public int CustomerID { get; set; }
       public DateTime OrderDate { get; set; }
       public decimal ShipAmount { get; set; }
       public decimal TaxAmount { get; set; }
       public List<OrderItem> Items { get; set; } = new();
   }
   
   public sealed class OrderItem
   {
       public int ItemID { get; set; }
       public int OrderID { get; set; }
       public int ProductID { get; set; }
       public decimal ItemPrice { get; set; }
       public decimal DiscountAmount { get; set; }
       public int Quantity { get; set; }
   }
   
   var sql = @"
   SELECT
       o.orderID      AS OrderID,
       o.customerID   AS CustomerID,
       o.orderDate    AS OrderDate,
       o.shipAmount   AS ShipAmount,
       o.taxAmount    AS TaxAmount,
   
       oi.itemID          AS ItemID,
       oi.orderID         AS OrderID,
       oi.productID       AS ProductID,
       oi.itemPrice       AS ItemPrice,
       oi.discountAmount  AS DiscountAmount,
       oi.quantity        AS Quantity
   FROM dbo.orders o
   JOIN dbo.orderItems oi ON o.orderID = oi.orderID
   ORDER BY o.orderID;
   ";
   
   var lookup = new Dictionary<int, Order>();
   
   var result = await connection.QueryAsync<Order, OrderItem, Order>(
       sql,
       (order, item) =>
       {
           if (!lookup.TryGetValue(order.OrderID, out var existing))
           {
               existing = order;
               existing.Items = new List<OrderItem>();
               lookup.Add(existing.OrderID, existing);
           }
   
           existing.Items.Add(item);
           return existing;
       },
       splitOn: "ItemID"
   );
   
   var orders = lookup.Values.ToList();
*/

// -- Select specified number of records
/*
 * var customers = await db.Customers
   .AsNoTracking()
   .OrderBy(c => c.CustomerID)   // IMPORTANT: stable ordering
   .Take(n)
   .ToListAsync();
 */
Console.WriteLine();
int n = 5;

var productsNRecordsEfCore = await db.Products.AsNoTracking()
    .OrderBy(p => p.ProductID)
    .Select(p => new
    {
        ProductID = p.ProductID,
        CategoryID = p.CategoryID,
        Name = p.ProductName
    })
    .Take(n)
    .ToListAsync();

foreach (var item in productsNRecordsEfCore)
{
    Console.WriteLine($"Product Id: {item.ProductID}");
}



var expensiveProductsWithEfCore = await db.Products
    .AsNoTracking()
    .Where(p => p.ListPrice > 100m)
    .OrderByDescending(p => p.ListPrice)
    .Take(n)
    .Select(p => new { p.ProductID, p.ProductName, p.ListPrice })
    .ToListAsync();

foreach (var item in expensiveProductsWithEfCore)
{
    Console.WriteLine($"Product Id: {item.ProductID}, ProductName: {item.ProductName}, ListPrice: {item.ListPrice}");
}

/* -- Strongly typed dapper
public sealed class CustomerRow
{
    public int CustomerID { get; set; }
    public string EmailAddress { get; set; } = "";
}

int n = 5;

var sql = @"
SELECT TOP (@N)
    customerID AS CustomerID,
    emailAddress AS EmailAddress
FROM dbo.customers
ORDER BY customerID;";

var customers = await conn.QueryAsync<CustomerRow>(sql, new { N = n });
 */

var dapperTopNResultsSql = @"
SELECT TOP (@N)
    customerID,
    emailAddress
FROM dbo.customers
ORDER BY customerID;";

var rows = await conn.QueryAsync(dapperTopNResultsSql, new { N = n });

/*
int page = 2;
int pageSize = 10;
   
   var customers = await db.Customers
       .AsNoTracking()
       .OrderBy(c => c.CustomerID)
       .Skip((page - 1) * pageSize)
       .Take(pageSize)
       .ToListAsync();
       
int page = 2;
int pageSize = 10;
   
   var sql = @"
   SELECT customerID, emailAddress
   FROM dbo.customers
   ORDER BY customerID
   OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;";
   
   var customers = await connection.QueryAsync(sql,
       new { Offset = (page - 1) * pageSize, PageSize = pageSize });
*/