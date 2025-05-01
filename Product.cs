public class Product
{
  public int ProductId { get; set; }
  public string? ProductName { get; set; }
  
  public bool Discontinued {get; set;}

  public int CategoryID { get; set; }
  public Category? category { get; set; }
}
