using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Aves.Aves
{
    public class Aguila : Ave
    {
        public override string Nombre { get; set; } = string.Empty;
        public override string Color { get; set; } = string.Empty;
        public override string Especie { get; set; } = string.Empty;

        public override string Volar()
        {
            return "Vuela alto";
        }
    }
}
