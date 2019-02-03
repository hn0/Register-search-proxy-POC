using System;

namespace oibregistarhack
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistarSearch search = new RegistarSearch();
            

            // Console.WriteLine("Enter search term");
            // string inp = Console.ReadLine();

            string inp = "test";
            Console.WriteLine( search.Results( inp ) );
        }
    }
}
