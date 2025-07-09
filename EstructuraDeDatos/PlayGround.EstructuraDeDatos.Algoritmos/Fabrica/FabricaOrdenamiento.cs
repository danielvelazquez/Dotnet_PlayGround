using PlayGround.EstructuraDeDatos.Algoritmos.Ordenamiento;

namespace PlayGround.EstructuraDeDatos.Algoritmos.Fabrica
{
    public static class FabricaOrdenamiento
    {
        public static IBaseFabricaOrdenamiento CrearOrdenamiento(TiposFabricaOrdenamiento tipo)
        {
            //switch (tipo)
            //{
            //    case TiposFabricaOrdenamiento.SelectionSort:
            //        return new SelectionSort();
            //    case TiposFabricaOrdenamiento.ShellSort:
            //        return new ShellSort();
            //    case TiposFabricaOrdenamiento.NaturalMergeSort:
            //        return new NaturalMergeSort();
            //    case TiposFabricaOrdenamiento.BubbleSort:
            //        return new BubbleSort();
            //    default:
            //        throw new ArgumentOutOfRangeException();
            //};

            return tipo switch
            {
                TiposFabricaOrdenamiento.BubbleSort => new BubbleSort(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
