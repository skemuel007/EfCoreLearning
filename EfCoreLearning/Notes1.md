# The When, Why and When Not to Use

## ICollection<T>

When, why, and when not to use ICollection<T> in EF Core.

The current models work without Navigational properties `see models folder and classes, sample provided below`

```csharp
public sealed class Order
{
    public int OrderID { get; set; }
    public int CustomerID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal ShipAmount { get; set; }
    public decimal TaxAmount { get; set; }
}
```

This is perfect for
 - Dapper
 - RAW SQL
 - EF Core without Navigation properties

EF Core can still work because:
 - `CustomerID` is a <strong>foreign key scalar</strong>
 - You can write queries manually using `.Join()` or `SQL`

```csharp
var orders = await db.Orders
    .Where(o => o.CustomerID == 1)
    .ToListAsync();
```

The above is explicit, predictable, and performant.

### What ICollection<T> actually does

When you add something like this:

```csharp
public sealed class Customer
{
    public int CustomerID { get; set; }
    // other csharp properties
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
```
You’re telling EF Core:

> “This entity has a navigation relationship.”

EF now understands:

- `Customer → Orders` is **1-to-many**
- It can automatically:
  - Load related data (Include)
  - Track relationships 
  - Cascade deletes (if configured)

### With navigation properties (classic EF style)
```csharp
public sealed class Customer
{
    public int CustomerID { get; set; }
    public string FirstName { get; set; } = "";

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public sealed class Order
{
    public int OrderID { get; set; }
    public int CustomerID { get; set; }

    public Customer Customer { get; set; } = null!;
}

```

Now you can do:

```csharp
var customers = await db.Customers
    .Include(c => c.Orders)
    .ToListAsync();
```

EF automatically builds:
```sql
SELECT *
FROM Customers c
    LEFT JOIN Orders o 
    ON c.CustomerID = o.CustomerID
```

### Why you might NOT want ICollection<T>

This is very important — especially for clean architecture.

1. Hidden performance costs
`db.Customers.ToList();`

Looks innocent — but with lazy loading enabled:
- It may trigger N+1 queries 
- You lose control over SQL 
- Unexpected joins happen

2. Over-fetching data

If you have:
```
Customer → Orders → OrderItems → Products
```
One innocent query can load hundreds or thousands of rows and
This kills performance in APIs.

3. Poor boundaries (DDD concern)

Entities should model behavior, not database shape.

Having:
```
Customer → Orders → OrderItems → Products
```

Creates a deep object graph, which:
- Makes updates dangerous 
- Encourages fat aggregates 
- Breaks transactional boundaries

