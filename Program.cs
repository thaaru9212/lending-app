using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static string filePath = "lenders.txt";

    static void Main(string[] args)
    {
        Console.WriteLine(@" 
  _                           _             
 | |                         | |            
 | |     ___  __ _ _ __ _ __ | | _____ _ __ 
 | |    / _ \/ _` | '__| '_ \| |/ / _ \ '__|
 | |___|  __/ (_| | |  | | | |   <  __/ |   
 |______\___|\__,_|_|  |_| |_|_|\_\___|_|   
                                            
");
        List<Lender> lenders = LoadLenders();

        while (true)
        {
            Console.WriteLine("*****************************************");
            Console.WriteLine("*            LENDER TRACKER             *");
            Console.WriteLine("*****************************************");
            Console.WriteLine("1. Add Lender");
            Console.WriteLine("2. View All Lenders");
            Console.WriteLine("3. Save & Exit");
            Console.Write("Choose an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    AddLender(lenders);
                    break;
                case "2":
                    ViewLenders(lenders);
                    break;
                case "3":
                    SaveLenders(lenders);
                    Console.WriteLine("Data saved. Exiting program.");
                    return;
                default:
                    Console.WriteLine("Invalid option, please try again.\n");
                    break;
            }
        }
    }

    static void AddLender(List<Lender> lenders)
    {
        Console.Write("Enter lender's name: ");
        string name = Console.ReadLine();
        Console.Write("Enter amount owed: ");
        decimal amountOwed = decimal.Parse(Console.ReadLine());

        lenders.Add(new Lender(name, amountOwed));
        Console.WriteLine("Lender added successfully!\n");
    }

    static void ViewLenders(List<Lender> lenders)
    {
        Console.WriteLine("\nList of All Lenders:");
        foreach (var lender in lenders)
        {
            Console.WriteLine(lender);
        }
        Console.WriteLine();
    }

    static void SaveLenders(List<Lender> lenders)
    {
        using (StreamWriter file = new StreamWriter(filePath))
        {
            foreach (var lender in lenders)
            {
                file.WriteLine($"{lender.Name},{lender.AmountOwed}");
            }
        }
    }

    static List<Lender> LoadLenders()
    {
        List<Lender> lenders = new List<Lender>();
        if (File.Exists(filePath))
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 2)
                    {
                        string name = parts[0];
                        decimal amountOwed = decimal.Parse(parts[1]);
                        lenders.Add(new Lender(name, amountOwed));
                    }
                }
            }
        }
        return lenders;
    }
}

public class Lender
{
    public string Name { get; set; }
    public decimal AmountOwed { get; set; }

    public Lender(string name, decimal amountOwed)
    {
        Name = name;
        AmountOwed = amountOwed;
    }

    public override string ToString()
    {
        return $"Lender: {Name}, Amount Owed: ${AmountOwed}";
    }
}