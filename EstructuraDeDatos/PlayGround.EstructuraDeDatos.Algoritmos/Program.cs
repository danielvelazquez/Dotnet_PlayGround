using PlayGround.EstructuraDeDatos.Algoritmos.Fabrica;

namespace PlayGround.EstructuraDeDatos.Algoritmos
{
    internal class Program
    {
        // TODO: Add Timer to measure the time it takes to sort the array
        // TODO: Implement method foreach sorting algorithm
        static void Main(string[] args)
        {
            // BubbleSort
            IBaseFabricaOrdenamiento baseFabricaOrdenamiento = FabricaOrdenamiento.CrearOrdenamiento(TiposFabricaOrdenamiento.BubbleSort);
            int[] arreglo = { 1, 2, 3, 4, 5 };
            
            baseFabricaOrdenamiento.Sort(arreglo);

            Console.WriteLine(string.Join(", ", arreglo));
        }
    }
}
