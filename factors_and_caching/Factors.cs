using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace factors_and_caching
{
    internal static class Factors
    {
        // Key: hash code, Value: factor pairs table
        private static Dictionary<int, Dictionary<int, int[]>> inputCache = new Dictionary<int, Dictionary<int, int[]>>();

        // Key: integer, Value: array of factors
        private static Dictionary<int, int[]> factorPairs = new Dictionary<int, int[]>();
                
        // Empty the cache
        public static void ClearCache()
        {
            inputCache.Clear(); // clear all hash <-> factor pair tables in the cache

            Console.WriteLine("Cache cleared.");
        }

        // Load cache from disk (cache.bin)
        public static void LoadCache()
        {
            // Load the JSON cache from disk
            string json = System.IO.File.ReadAllText("cache.json");

            // Deserialize the JSON
            JObject jsonObject = (JObject)JsonConvert.DeserializeObject(@json);

            // Clear the existing cache
            inputCache.Clear();
            
            foreach (JProperty cacheEntry in jsonObject.Children()) // Get hash code <-> factor pair entries
            {
                // Get the hash code of the current entry
                int hashCode = Convert.ToInt32(cacheEntry.Name);

                JObject fPTable = (JObject)cacheEntry.First; // Get the factor pairs table

                Dictionary<int, int[]> fP = new Dictionary<int, int[]>();
                foreach (JProperty pair in fPTable.Children()) // Each pair
                {                    
                    // Add the integer and the array of factors
                    fP.Add(Convert.ToInt32(pair.Name), pair.First.Select(jv => (int)jv).ToArray());                   
                }

                // Add the hash code and it's deserialized factor pairs table to the cache
                inputCache.Add(Convert.ToInt32(cacheEntry.Name), fP);
            }

            Console.WriteLine("Loaded cache from disk.");
        }

        // Flush cache to disk (cache.bin)
        public static void FlushCache()
        {
            // Serialize the cache to JSON
            string json = JsonConvert.SerializeObject(inputCache);

            // Write the JSON to disk
            System.IO.File.WriteAllText("cache.json", @json);

            Console.WriteLine("Flushed cache to disk.");
        }

        // Initial challenge: parse the input of arrays, such that for each integer i, determine if each other integer in array is a factor of i
        public static void Parse(int[] input, bool reverse = false) // reverse, Challenge Question 3: check if integer i is factor of array members x
        {            
            if (input == null || input.Length <= 0)
                Console.WriteLine("Invalid input to calculate factors. Empty/null array.");

            // Clear any factor pairs from the previous parse.
            factorPairs.Clear();

            // Hash the input array
            int hashCode = GetArrayHashCode(input);

            // Check if input array exists with results in cache
            if (inputCache.ContainsKey(hashCode))
            {
                // Calculation exists, return results from cache
                factorPairs = new Dictionary<int,int[]>(inputCache[hashCode]);

                Console.WriteLine("Loaded values from cache.");
            }
            else // Calculate input array factor pairs, then add to cache
            {
                int[] factors;

                // Iterate through each integer in the array
                foreach (int i in input)
                {
                    // Return array of all integers in input array that are factors of the given integer, i (division modulus remainder of 0)
                    factors = reverse ? input.Where(x => x % i == 0 && i != x).ToArray() : input.Where(x => i % x == 0 && i != x).ToArray();

                    // Add each integer and array of factors to the factorPairs dictionary (to be printed / cached)
                    factorPairs.Add(i, factors);
                }

                // Add to cache
                inputCache.Add(hashCode, new Dictionary<int, int[]>(factorPairs));

                Console.WriteLine("Computed values and added to cache.");
            }
        }
                
        public static void Print()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{ ");

            foreach (int key in factorPairs.Keys)
            {
                sb.Append(string.Format("{0}:", key));

                sb.Append(" [");

                sb.Append(string.Join(", ", factorPairs[key]));

                sb.Append("] ");
            }

            sb.Append(" }");

            Console.WriteLine(sb.ToString());
        }

        public static void PrintCache()
        {
            foreach (int key in inputCache.Keys)
            {
                factorPairs = inputCache[key];

                Console.WriteLine(string.Format("Hash Code: {0}", key));
                    Print();
            }
        }

        private static int GetArrayHashCode(int[] input)
        {
            if (input == null || input.Length == 0)
                return 17; // base prime number

            // Sort integer array so that identical arrays in different orders are treated as the same array in cache
            Array.Sort(input);

            // Based on Josh Bloch's "Effective Java" hash code generation
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;

                foreach (int i in input)
                    hash = hash * 23 + i.GetHashCode();
                
                return hash;
            }
        }
    }
}
