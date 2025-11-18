namespace frontend.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Seller { get; set; }
    public decimal Price { get; set; }
    public double Rating { get; set; }
    public string ImagePath { get; set; }
    public string Category { get; set; }
}