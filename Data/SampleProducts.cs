using ProductManagement.Models;

namespace ProductManagement.Data;

public static class SampleProducts
{
    public static List<Product> Products { get; } = new()
    {
       new()
    {
        Id = 1,
        Name = "Wireless Bluetooth Headphones",
        SkuCode = "AUD1001",
        Description = "Over-ear Bluetooth headphones with active noise cancellation and 30-hour battery life.",
        UnitPrice = 4999,
        Category = "Electronics",
        ImageUrl = "https://picsum.photos/seed/headphones/400/300"
    },
    new()
    {
        Id = 2,
        Name = "Mechanical Gaming Keyboard",
        SkuCode = "KEY1002",
        Description = "RGB mechanical keyboard featuring blue switches and durable aluminum construction.",
        UnitPrice = 3499,
        Category = "Electronics",
        ImageUrl = "https://picsum.photos/seed/keyboard/400/300"
    },
    new()
    {
        Id = 3,
        Name = "Wireless Optical Mouse",
        SkuCode = "MOU1003",
        Description = "Ergonomic wireless mouse with adjustable DPI and silent-click buttons.",
        UnitPrice = 1299,
        Category = "Accessories",
        ImageUrl = "https://picsum.photos/seed/mouse/400/300"
    },
    new()
    {
        Id = 4,
        Name = "Stainless Steel Water Bottle",
        SkuCode = "BOT1004",
        Description = "Double-wall insulated bottle that keeps drinks cold for 24 hours.",
        UnitPrice = 899,
        Category = "Home & Kitchen",
        ImageUrl = "https://picsum.photos/seed/bottle/400/300"
    },
    new()
    {
        Id = 5,
        Name = "Smart Fitness Watch",
        SkuCode = "WAT1005",
        Description = "Fitness smartwatch with heart-rate monitoring, GPS, and sleep tracking.",
        UnitPrice = 6999,
        Category = "Wearables",
        ImageUrl = "https://picsum.photos/seed/watch/400/300"
    },
    new()
    {
        Id = 6,
        Name = "Portable Bluetooth Speaker",
        SkuCode = "SPK1006",
        Description = "Compact waterproof Bluetooth speaker delivering rich sound and deep bass.",
        UnitPrice = 2799,
        Category = "Electronics",
        ImageUrl = "https://picsum.photos/seed/speaker/400/300"
    },
    new()
    {
        Id = 7,
        Name = "Laptop Backpack",
        SkuCode = "BAG1007",
        Description = "Water-resistant backpack with padded laptop compartment for up to 15.6-inch laptops.",
        UnitPrice = 1899,
        Category = "Bags",
        ImageUrl = "https://picsum.photos/seed/backpack/400/300"
    },
    new()
    {
        Id = 8,
        Name = "Ceramic Coffee Mug",
        SkuCode = "MUG1008",
        Description = "Premium ceramic coffee mug with a matte finish and 350 ml capacity.",
        UnitPrice = 399,
        Category = "Kitchen",
        ImageUrl = "https://picsum.photos/seed/mug/400/300"
    },
    new()
    {
        Id = 9,
        Name = "USB-C Fast Charger",
        SkuCode = "CHR1009",
        Description = "45W USB-C wall charger supporting fast charging for phones, tablets, and laptops.",
        UnitPrice = 1499,
        Category = "Accessories",
        ImageUrl = "https://picsum.photos/seed/charger/400/300"
    },
    new()
    {
        Id = 10,
        Name = "LED Desk Lamp",
        SkuCode = "LMP1010",
        Description = "Adjustable LED desk lamp with touch controls and multiple brightness levels.",
        UnitPrice = 2199,
        Category = "Home",
        ImageUrl = "https://picsum.photos/seed/desklamp/400/300"
    }
    };
}