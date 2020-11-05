using System;

namespace StringCalculator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var result = StringCalculator.Add(null);
            Console.WriteLine(result);
         }
    }
}