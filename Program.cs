﻿using NLog;
 using System.Linq;
 using Microsoft.Extensions.Configuration;
 using Microsoft.EntityFrameworkCore;
 using NorthwindConsole.Model;
 using System.ComponentModel.DataAnnotations;


using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Microsoft.Identity.Client;

using NLog.LayoutRenderers;
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
  Console.WriteLine("5) ");
  Console.WriteLine("6) ");
  Console.WriteLine("7) Display records in product table");
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
    foreach (Product p in category.Products)
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
      foreach (Product p in item.Products)
      {
        Console.WriteLine($"\t{p.ProductName}");
      }
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
    Console.WriteLine("0) to Main Menue");
    string? c5choice = Console.ReadLine();
  if (c5choice == "1")
  {
   foreach(var item in query)
   {
     Console.WriteLine($"{item.ProductId} - {item.ProductName} Discontinued = {item.Discontinued} ");
   }
  }
  else if (c5choice == "2")
  {
 foreach(var item in query) if (item.Discontinued == true )
 {
  Console.WriteLine($"{item.ProductId} - {item.ProductName} Discontinued = {item.Discontinued} ");
 }

  }

  else if (c5choice == "3")
  {
     foreach(var item in query) if (item.Discontinued == false )
 {
  Console.WriteLine($"{item.ProductId} - {item.ProductName} Discontinued = {item.Discontinued} ");
 }
  }
  else if (c5choice == "0")
  {
    break;
  }
  else if (String.IsNullOrEmpty(c5choice))
  {
    logger.Error("Invalid input");
  }
      } while (true);
   }
  else if (String.IsNullOrEmpty(choice))
  {
    break;
  }
  Console.WriteLine();





} while (true);

logger.Info("Program ended");

