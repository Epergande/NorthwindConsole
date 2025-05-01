using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class DataContext : DbContext
{
  public DbSet<Category> Categories { get; set; }
  public DbSet<Product> Products{ get; set; }

  public void AddCategory(Category category)
  {
    this.Categories.Add(category);
    this.SaveChanges();
  }
  /*public void AddPost(Post post)
  {
    this.Posts.Add(post);
    this.SaveChanges();
  }
    public void EditBlog(Blog UpdatedBlog)
   {
     Blog blog = Blogs.Find(UpdatedBlog.BlogId)!;
     blog.Name = UpdatedBlog.Name;
     this.SaveChanges();
   }
    public void DeleteBlog(Blog blog)
   {
     this.Blogs.Remove(blog);
     this.SaveChanges();
   }*/
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

    var config = configuration.Build();
    optionsBuilder.UseSqlServer(@config["Northwind:ConnectionString"]);
  }
}