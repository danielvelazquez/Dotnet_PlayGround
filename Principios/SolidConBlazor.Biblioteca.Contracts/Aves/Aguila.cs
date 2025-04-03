using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Aves.Aves
{
    public class Aguila : Aves
    {
        public override string Nombre => throw new NotImplementedException();

        public override string Volar()
        {
            return "Vuela alto";
        }
    }
}
