using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Figuras
{
    public class Circulo : IFigura
    {
        public double Radio { get; set; }
        public double CalcularArea()
        {
            return Math.PI * Radio * Radio;
        }
    }
}
