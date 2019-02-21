using Artifact.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Meta meta = new Meta();
            meta["name"] = "fuck you";
            Console.WriteLine(meta["name"]);
            Console.ReadKey();
        }
    }
}
