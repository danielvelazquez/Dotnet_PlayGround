using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace PruebasParalelismo
{
    public class TareasParalelas
    {
        public static void PrimeraTarea()
        {
            Console.WriteLine("Iniciando Tarea 1");
            Thread.Sleep(1000);
            Console.WriteLine("Finalizando Tarea 1");
        }
        public static void SegundaTarea()
        {
            Console.WriteLine("Iniciando Tarea 2");
            Thread.Sleep(1000);
            Console.WriteLine("Finalizando Tarea 2");
        }
        // Parallel.Invoke acepta cierto numero de "Acciones" de [Tipo delegado] y crea una "Tarea" por cada uno de ellos.
        // El metodo puede iniciar una gran cantidad de Tareas a la vez. 
        // No se puede controlar el orden en que las tareas son ejecutadas o a que proceso son asignadas.
        public string[] ProcesaParalelos()
        {
            string[] argumento = new string[1];
            Console.WriteLine("Iniciando Tareas Paralelas");
            Parallel.Invoke(() => TareasParalelas.PrimeraTarea(), () => TareasParalelas.SegundaTarea());
            Console.WriteLine("Teclee Si para continuar, No para finalizar");
            var seleccion = Console.ReadLine();
            Misc validaOpcion = new Misc();
            bool seleccionado = validaOpcion.OpcionContinuar(seleccion);
            if (seleccionado == true)
            {
                argumento[0] = "Restart";
                return argumento;
            }
            return argumento;
        }

        // Funcion que procesa un For Paralelo
        public string[] ProcesaForParalelo()
        {
            string[] argumento = new string[1];
            Console.WriteLine("Iniciando Tareas Paralelas");
            var items = Enumerable.Range(0, 500).ToArray(); // El Rango a utilizar
            ParallelLoopResult result = Parallel.For(0, items.Count(), (int i, ParallelLoopState loopState) =>
            {
                if (i == 200)
                {
                    loopState.Stop(); // El proceso de iteracion se detiene al llegar al ranto establecido. Las tareas pendientes no se ejecutan
                    //loopState.Break(); // El proceso se detiene, las tareas iniciadas con indice menor al establecido finalizaran.
                }
                //WorkOnItem(items[i]);
            });
            
            return argumento;
        }
    }
}
