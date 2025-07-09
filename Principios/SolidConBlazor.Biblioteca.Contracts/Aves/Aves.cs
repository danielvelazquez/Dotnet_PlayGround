using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Aves
{
    public abstract class Ave
    {
        public abstract string Nombre { get; set; }
        public abstract string Color { get; set; }
        public abstract string Especie { get; set; }
        public abstract string Volar();
    }
}
