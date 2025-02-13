using System;

namespace Fibonacci
{
    class Program
    {
        //static bool TryFibonacci(object o, out int r)
        //{


        //}
        //static (int cur, int prev) Fib(int i)
        //{
        //    if (i == 0) return (1, 0);
        //    var (c, p) = Fib(i - 1);
        //    return (curr: c + p, prev: c);
        //}
        //static int Fibonacci(int n)
        //{
        //    var (r, _) = Fib(n);
        //    return r;
        //}
        public static int FibonacciIterativo(int n)
        {
            int a = 0;
            int b = 1;
            // In N steps compute Fibonacci sequence iteratively.
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Introdusca el numero del que quiere obtener la secuencia Fibonacci");
            int.TryParse(Console.ReadLine(), out int secde);
            if (secde != 0)
            {
                Console.WriteLine("Ésta es la secuencia Fibonacci del numero {0}", secde);
                for (int i = 0; i < secde; i++)
                {
                    Console.WriteLine(FibonacciIterativo(i));
                }
            }
            else
            {
                Console.WriteLine("El valor introducido no es un numero entero valido");
                return;
            }
            Console.ReadLine();
        }
    }
}
