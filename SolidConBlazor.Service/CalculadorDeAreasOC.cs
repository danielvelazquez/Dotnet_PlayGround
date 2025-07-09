using SolidConBlazor.Biblioteca.Contracts.Figuras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Service
{
    public class CalculadorDeAreasOC
    {
        // Open Close Principle
        // Este principio establece que las clases deben estar abiertas para su extensión, pero cerradas para su modificación.
        // En este código, la clase CalculadorDeAreasOC cumple con este principio, ya que no es necesario modificar la clase para agregar nuevas figuras.
        // En su lugar, se puede crear una nueva clase que implemente la interfaz IFigura y se puede pasar esa clase al método CalcularArea.
        public double CalcularArea(IFigura figura)
        {
            return figura.CalcularArea();
        }
    }
}
