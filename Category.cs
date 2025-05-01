using System.ComponentModel.DataAnnotations;



public class Category

{
  public int CategoryId { get; set; }
  [Required]
    public string? CategoryName { get; set; }
       public string? Description { get; set; }
  public List<Product>? Products { get; set; }
}
