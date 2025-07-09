using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Biblioteca.Contracts.Figuras
{
    public class Rectangulo : IFigura
    {
        public double Base { get; set; }
        public double Altura { get; set; }
        public double CalcularArea()
        {
            return Base * Altura;
        }
    }
}