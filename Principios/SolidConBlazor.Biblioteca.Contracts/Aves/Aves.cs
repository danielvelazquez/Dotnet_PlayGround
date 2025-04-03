using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Aves
{
    public abstract class Aves
    {
        public abstract string Nombre { get; }
        public abstract string Volar();
    }
}
