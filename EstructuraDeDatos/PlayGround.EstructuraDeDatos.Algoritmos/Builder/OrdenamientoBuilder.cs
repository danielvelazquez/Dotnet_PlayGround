using PlayGround.EstructuraDeDatos.Algoritmos.Fabrica;
using PlayGround.EstructuraDeDatos.Algoritmos.Ordenamiento;

namespace PlayGround.EstructuraDeDatos.Algoritmos.Builder
{
    // TODO: Mover este patron a otra solución mas efectiva y clara
    public class OrdenamientoBuilder //: IOrdenamientoBuilder
    {
        private TiposFabricaOrdenamiento _tipo;
        private int[] _datos;
        //public IOrdenamientoBuilder ConTipo(TiposFabricaOrdenamiento tipoOrdenamiento)
        //{
        //    _tipo = tipoOrdenamiento;
        //    return this;
        //}
        //public IOrdenamientoBuilder ConDatos(int[] datos)
        //{
        //    _datos = datos;
        //    return this;
        //}

        //public TiposFabricaOrdenamiento Construir()
        //{
        //    IBaseFabricaOrdenamiento fabrica = FabricaOrdenamiento.CrearOrdenamiento(_tipo);

        //}

        
    }
}
