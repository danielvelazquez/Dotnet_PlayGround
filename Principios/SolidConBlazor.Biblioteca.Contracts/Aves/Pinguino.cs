using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Aves
{
    public class Pinguino : Ave
    {
        public override string Nombre { get; set;} = string.Empty;
        public override string Color { get; set; } = string.Empty;
        public override string Especie { get; set; } = string.Empty;


        // Esto viola el principio de Liskov ya que no se puede implementar el método Volar
        // Los pingüinos no vuelan
        public override string Volar()
        {
            throw new NotImplementedException();
        }
    }
}
