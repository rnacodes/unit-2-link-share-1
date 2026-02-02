using System;
using System.IO;
using System.Linq;
using System.Globalization;


/// <summary>
/// SalesAnalyzer2 provides methods to load sales data from a CSV file, calculate total sales, and find the top-selling product.
/// </summary>
public class SalesAnalyzer2
{
    private string _filename;
    private string[] _dataLines;

    /// <summary>
    /// Initializes a new instance of the SalesAnalyzer2 class with the specified filename.
    /// </summary>
    /// <param name="filename">The path to the CSV file containing sales data.</param>
    public SalesAnalyzer2(string filename)
    {
        _filename = filename;
        _dataLines = null;
    }

    /// <summary>
    /// Loads sales data from the CSV file specified during initialization.
    /// Reads all data lines after skipping the header row.
    /// </summary>
    /// <returns>True if the data was successfully loaded; false otherwise.</returns>
    public bool load_data()
    {
        try
        {
            _dataLines = File.ReadLines(_filename).Skip(1).ToArray();
            return true;
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file '{_filename}' was not found.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while loading data: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Calculates the total sales by summing the product of price and quantity for all items.
    /// </summary>
    /// <returns>The total sales amount as a decimal value.</returns>
    public decimal calculate_total_sales()
    {
        decimal total = 0.0m;
        if (_dataLines == null)
        {
            Console.WriteLine("Error: Data has not been loaded. Call load_data() first.");
            return total;
        }
        try
        {
            foreach (string line in _dataLines)
            {
                string[] columns = line.Split(',');
                if (columns.Length == 3)
                {
                    if (decimal.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) &&
                        int.TryParse(columns[2], out int quantity))
                    {
                        total += price * quantity;
                    }
                    else
                    {
                        Console.WriteLine($"Skipping malformed data in row: {line}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while calculating total sales: {ex.Message}");
        }
        return total;
    }

    /// <summary>
    /// Finds the top-selling product by total revenue (price * quantity).
    /// </summary>
    /// <returns>A tuple containing the product name and its total revenue. Returns (null, 0) if no valid data is found.</returns>
    public (string ProductName, decimal Revenue) find_top_product()
    {
        string topProduct = null;
        decimal maxRevenue = 0.0m;
        if (_dataLines == null)
        {
            Console.WriteLine("Error: Data has not been loaded. Call load_data() first.");
            return (null, 0.0m);
        }
        try
        {
            foreach (string line in _dataLines)
            {
                string[] columns = line.Split(',');
                if (columns.Length == 3)
                {
                    if (decimal.TryParse(columns[1], NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price) &&
                        int.TryParse(columns[2], out int quantity))
                    {
                        decimal revenue = price * quantity;
                        if (revenue > maxRevenue)
                        {
                            maxRevenue = revenue;
                            topProduct = columns[0];
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred while finding top product: {ex.Message}");
        }
        return (topProduct, maxRevenue);
    }
}

/// <summary>
/// Main entry point of the application. Demonstrates the usage of the SalesAnalyzer2 class by loading data, calculating totals, and finding the top product.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        string salesDataFile = "sales_data.csv";
        if (!File.Exists(salesDataFile))
        {
            File.WriteAllText(salesDataFile, "product_name,price,quantity\nLaptop,1200.00,5\nMouse,25.50,10\n");
        }
        SalesAnalyzer2 analyzer = new SalesAnalyzer2(salesDataFile);
        if (!analyzer.load_data())
        {
            Console.WriteLine("Failed to load data. Exiting.");
            return;
        }
        decimal totalSales = analyzer.calculate_total_sales();
        Console.WriteLine($"Total sales from {salesDataFile}: {totalSales.ToString("C", CultureInfo.CurrentCulture)}");
        var (topProduct, topRevenue) = analyzer.find_top_product();
        if (topProduct != null)
        {
            Console.WriteLine($"Top-selling product: {topProduct}\nTotal revenue: {topRevenue.ToString("C", CultureInfo.CurrentCulture)}");
        }
        else
        {
            Console.WriteLine("No valid product data found to determine the top-selling product.");
        }
    }
}
