using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Aves
{
    public class Pinguino : Aves
    {
        public override string Nombre => throw new NotImplementedException();

        // Esto viola el principio de Liskov ya que no se puede implementar el método Volar
        // Los pingüinos no vuelan
        public override string Volar()
        {
            throw new NotImplementedException();
        }
    }
}
