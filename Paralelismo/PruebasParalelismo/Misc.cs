namespace PruebasParalelismo
{
    public class Misc
    {
        public bool OpcionContinuar(string opcion)
        {
            if (opcion.ToUpper() == "SI")
            {
                return true;
            }
            return false;
        }
    }
}
