namespace EfCoreLearning.models;

public sealed class Customer
{
    public int CustomerID { get; set; }
    public string EmailAddress { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int? ShipAddressID { get; set; }
    public int? BillingAddressID { get; set; }
}

public sealed class Address
{
    public int AddressID { get; set; }
    public int CustomerID { get; set; }
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public bool Disabled { get; set; }
}

public sealed class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = "";
}

public sealed class Product
{
    public int ProductID { get; set; }
    public int CategoryID { get; set; }
    public string ProductName { get; set; } = "";
    public decimal ListPrice { get; set; }
}

public sealed class Order
{
    public int OrderID { get; set; }
    public int CustomerID { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal ShipAmount { get; set; }
    public decimal TaxAmount { get; set; }
}

public sealed class OrderItem
{
    public int ItemID { get; set; }
    public int OrderID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    public decimal ItemPrice { get; set; }
}