using PlayGround.EstructuraDeDatos.Algoritmos.Fabrica;
using PlayGround.EstructuraDeDatos.Algoritmos.Ordenamiento;
namespace PlayGround.EstructuraDeDatos.Algoritmos.Builder
{
    public interface IOrdenamientoBuilder
    {
        IOrdenamientoBuilder ConTipo(TiposFabricaOrdenamiento tipoOrdenamiento);
        IOrdenamientoBuilder ConDatos(int[] datos);
        TiposFabricaOrdenamiento Construir();
    }
}
