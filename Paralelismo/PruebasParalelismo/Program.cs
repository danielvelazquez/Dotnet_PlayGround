using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PruebasParalelismo;

namespace PruebasParalelismo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Console.Clear();
            }
            var main = new Program();
            Console.WriteLine("Pruebas de Paralelismo");
            Console.WriteLine("Elije el tipo de prueba que quieras validar tecleando la letra que corresponda.");
            Console.WriteLine("A) Tareas paralelas.");
            var opcion = Console.ReadLine();
            switch (opcion.ToString().ToUpper())
            {
                case "A":
                    var tp = new TareasParalelas();
                    string[] procRes = tp.ProcesaParalelos();
                    if (procRes.Length > 0)
                    {
                        if (procRes[0] == "Restart")
                         Main(procRes);
                    }
                    Console.WriteLine("Finalizando Procesamiento de tareas. Presiona Cualquier Tecla para finalizar");
                    Console.ReadLine();
                    break;
                default:
                    Console.WriteLine("Opcion no valida");
                    string[] opcNoValida = new string[1];
                    opcNoValida[0] = "Restart";
                    Main(opcNoValida);
                    break;
            }
        }


    }
}
