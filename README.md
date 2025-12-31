
## Understanding the Relationships (Simple View)
```
Customer
├── Addresses (1-to-many)
├── Orders (1-to-many)
    ├── OrderItems (1-to-many)
        └── Products (many-to-1)
Products
└── Categories (many-to-1)
```

# When and Not to use Navigational Properties

