﻿using NLog;
 using System.Linq;
 using Microsoft.Extensions.Configuration;
 using Microsoft.EntityFrameworkCore;

 using System.ComponentModel.DataAnnotations;


using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Microsoft.Identity.Client;

using NLog.LayoutRenderers;
using Microsoft.EntityFrameworkCore.Metadata;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

do
{

  Console.WriteLine("1) Display categories");
  Console.WriteLine("2) Add category");
  Console.WriteLine("3) Display Category and related products");
  Console.WriteLine("4) Display all Categories and their related products");
  Console.WriteLine("5) Add new records to the Products table");
  Console.WriteLine("6) Edit a specified record from the Products table");
  Console.WriteLine("7) Display records in product table");
  Console.WriteLine("8) Display a specific product");
  Console.WriteLine("9) Display all Categories and their related active product data ");
  Console.WriteLine("10) Display a specific Category and its related active product data ");
  Console.WriteLine("Press Enter to End Session");
  string? choice = Console.ReadLine();
  Console.Clear();
  logger.Info("Option {choice} selected", choice);

  if (choice == "1")
  {
    // display categories
    var configuration = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json");

    var config = configuration.Build();

    var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryName);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Magenta;
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryName} - {item.Description}");
    }
    Console.ForegroundColor = ConsoleColor.White;
  }
  else if (choice == "2")
  {
    // Add category
    Category category = new();
    Console.WriteLine("Enter Category Name:");
    category.CategoryName = Console.ReadLine()!;
    Console.WriteLine("Enter the Category Description:");
    category.Description = Console.ReadLine();
    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
      var db = new DataContext();
      // check for unique name
      if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
      {
        // generate validation error
        isValid = false;
        results.Add(new ValidationResult("Name exists", ["CategoryName"]));
      }
      else
      {
        logger.Info("Validation passed");
        db.AddCategory(category);
      }
    }
    if (!isValid)
    {
      foreach (var result in results)
      {
        logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
      }
    }
  }
  else if (choice == "3")
  {
    var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryId);

    Console.WriteLine("Select the category whose products you want to display:");
    Console.ForegroundColor = ConsoleColor.DarkRed;
   
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }
    Console.ForegroundColor = ConsoleColor.White;
    int id = int.Parse(Console.ReadLine()!);


    Console.Clear();
    logger.Info($"CategoryId {id} selected");
    Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id)!;
    Console.WriteLine($"{category.CategoryName} - {category.Description}");
    #pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (var p in category.Products)
    {
      Console.WriteLine($"\t{p.ProductName}");
    }
}

    
  else if (choice == "4")
  {
    var db = new DataContext();
    var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryName}");

            foreach (var p in item.Products)
      {
        Console.WriteLine($"\t{p.ProductName}");
      }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        }
  }
  else if (choice =="5")
  {
     var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryName);
     Console.WriteLine("Which Category do you want to add a product to?");
  
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryId} ({item.CategoryName})");
    }

    
    string? inputID = Console.ReadLine();
    
    if (int.TryParse(inputID, out int chosenId) && query.Any(b => b.CategoryId == chosenId))
    {
        Console.Write("Enter a Name for the product: ");
        var name = Console.ReadLine();
        if (string.IsNullOrEmpty(name))
        {
            logger.Error("Product Name cannot be empty");
            return;
        }
          var product = new Product
        {
            ProductName = name,            
            CategoryID = chosenId
        };


        db.AddProduct(product);
        logger.Info($"Product '{name}' successfully created in Category ID {chosenId}");
    }
    else
    {
        logger.Error("Invalid Category ID selected.");
    }
  }
  
  else if (choice == "6")
  {

    var db = new DataContext();

    var categories = db.Categories.OrderBy(p => p.CategoryName).ToList();
    Console.WriteLine("Which Category do you want to edit a product in?");
    foreach (var item in categories)
    {
        Console.WriteLine($"{item.CategoryId} ({item.CategoryName})");
      }

    if (!int.TryParse(Console.ReadLine(), out int chosenCategoryId) || !categories.Any(c => c.CategoryId == chosenCategoryId))
    {
       Console.WriteLine("Invalid category selection.");
       break;
    }

    var productsInCategory = db.Products.Where(p => p.CategoryID == chosenCategoryId).OrderBy(p => p.ProductId).ToList();
    if (!productsInCategory.Any())
    {
        Console.WriteLine("No products found in this category.");
        break;
    }

    Console.WriteLine("Which Product do you want to edit?");
    foreach (var item in productsInCategory)
    {
        Console.WriteLine($"{item.ProductId}) {item.ProductName}");
    }

    if (!int.TryParse(Console.ReadLine(), out int chosenProductId) || !productsInCategory.Any(p => p.ProductId == chosenProductId))
    {
        Console.WriteLine("Invalid product selection.");
        return;
    }

 
    Product? updatedProduct = InputProduct(db, logger);
    if (updatedProduct != null)
    {
        updatedProduct.ProductId = chosenProductId;
        db.EditProduct(updatedProduct);
        logger.Info($"Product (id: {chosenProductId}) updated");
    }
  }
  else if (choice == "7")
  {
    var db = new DataContext();
    var query = db.Products.OrderBy(p => p.ProductId);
  
    do{
    Console.WriteLine("What do you want to view?");
    Console.WriteLine("1) all Products");
    Console.WriteLine("2) Discontinued Products");
    Console.WriteLine("3) Active Products");
    Console.WriteLine("0) to Main Menu");
    string? c5choice = Console.ReadLine();
  if (c5choice == "1")
  {
   foreach(var item in query)
   {
     Console.WriteLine($"{item.ProductId} - {item.ProductName} (Discontinued = {item.Discontinued}) ");
   }
  }
  else if (c5choice == "2")
  {
 foreach(var item in query) if (item.Discontinued == true )
 {
  Console.WriteLine($"{item.ProductId} - {item.ProductName} ");
 }

  }

  else if (c5choice == "3")
  {
     foreach(var item in query) if (item.Discontinued == false )
 {
  Console.WriteLine($"{item.ProductId} - {item.ProductName} ");
 }
  }
  
  else if (c5choice == "0")
  {
    break;
  }

  else
   {
   logger.Error("Invalid input");
  }
      } while (true);
   }
else if (choice == "8")
{
  
 var db = new DataContext();
    var query = db.Categories.OrderBy(p => p.CategoryId);

   Console.WriteLine("Which category do you want to view a product from? ");

    foreach (var item in query)
    {
      Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }

    int id = int.Parse(Console.ReadLine()!);
    var query2 = db.Products.OrderBy(p => p.ProductId);
    Console.WriteLine("Which Product do you want to view?");
    foreach (var item in query2) if (item.CategoryID == id)
    {
     Console.WriteLine($"{item.ProductId}) {item.ProductName}");
    }
     
    else {
      logger.Error("No category With that id");
    }
    int idp = int.Parse(Console.ReadLine()!);
}
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  else {
    logger.Error("Invalid input");
  }
  Console.WriteLine();





} while (true);

logger.Info("Program ended");


 static Product? InputProduct(DataContext db, NLog.Logger logger)
 {
   Product product = new();
   Console.WriteLine("Enter the Product name");
   product.ProductName = Console.ReadLine();
 
   ValidationContext context = new(product, null, null);
   List<ValidationResult> results = [];
 
   var isValid = Validator.TryValidateObject(product, context, results, true);
   if (isValid)
   {
     // check for unique name
     if (db.Products.Any(b => b.ProductName == product.ProductName))
     {
       // generate validation error
       isValid = false;
       results.Add(new ValidationResult("Product name exists", ["Name"]));
     }
     else
     {
       logger.Info("Validation passed");
     }
   }
   if (!isValid)
   {
     foreach (var result in results)
     {
       logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
     }
     return null;
   }
   return product;
 }