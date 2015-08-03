using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace factors_and_caching
{
    class Program
    {
        static void Main(string[] args)
        {
            // Challenge Question
            Factors.Parse(new int[] { 10, 5, 2, 20 });            
            Factors.Print();

            // Test: Array in different order is loaded from cache
            Factors.Parse(new int[] { 20, 2, 5, 10 });
            Factors.Print();

            // Test: Same array twice is loaded from cache
            Factors.Parse(new int[] { 10, 5, 2, 20 });            
            Factors.Print();

            // Challenge Reversed
            Factors.ClearCache();
            Factors.Parse(new int[] { 10, 5, 2, 20 }, true);
            Factors.Print();

            // My additional tests
            Console.WriteLine("Big Array Test:");
            Factors.ClearCache();

            int[] bigArray = new int[10000];
            for (int i = 1; i <= 10000; i++)
                bigArray[i-1] = i;

            Factors.Parse(bigArray);
            Factors.Parse(bigArray); // Parse twice to show the cache / hashing works on bigger arrays
            Factors.ClearCache();

            // Cache flush/load test
            Factors.Parse(new int[] { 10, 5, 2, 20 });
            Factors.Parse(new int[] { 2, 4, 8, 16, 32, 64, 128, 256, 512 });
            Factors.Parse(new int[] { 3, 6, 9, 12, 15, 18, 21, 24 });
            Factors.Parse(new int[] { 4, 8, 12, 16, 20, 24, 28, 32 });
            Factors.Parse(new int[] { 5, 10, 15, 20, 25, 30, 35, 40 });

            Factors.FlushCache();
            Factors.ClearCache();
            Factors.LoadCache();
            Factors.PrintCache();

            // Printing the bigArray factors shuns the earlier prints
            //Factors.Print();

            // Hold the console until keypress
            Console.Read();
        }                
    }
}
