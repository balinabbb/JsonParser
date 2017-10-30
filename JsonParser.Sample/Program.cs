using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonParser.StateMachineParser;
using JsonParser.ThreadedParser;

namespace JsonParser.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            new BenchMark("short", new StateMachineJsonParser()).Launch(Console.WriteLine);
            new BenchMark("short", new ThreadedJsonParser()).Launch(Console.WriteLine);
            new BenchMark("short", new NewtonSoftParser()).Launch(Console.WriteLine);

            new BenchMark("long", new StateMachineJsonParser()).Launch(Console.WriteLine);
            new BenchMark("long", new ThreadedJsonParser()).Launch(Console.WriteLine);
            new BenchMark("long", new NewtonSoftParser()).Launch(Console.WriteLine);

            new BenchMark("extralong", new StateMachineJsonParser()).Launch(Console.WriteLine);
            new BenchMark("extralong", new ThreadedJsonParser()).Launch(Console.WriteLine);
            new BenchMark("extralong", new NewtonSoftParser()).Launch(Console.WriteLine);


            Console.WriteLine("done");
            Console.ReadLine();
        }
    }
}
