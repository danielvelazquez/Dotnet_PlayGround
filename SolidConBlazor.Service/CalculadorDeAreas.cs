using SolidConBlazor.Biblioteca.Contracts.Figuras;
using System.Drawing;

namespace SolidConBlazor.Service
{
    public class CalculadorDeAreas
    {
        // Open Close Principle
        // Este principio establece que las clases deben estar abiertas para su extensión, pero cerradas para su modificación.
        // En éste codigo podemos ver que la clase CalculadorDeAreas no está cumpliendo con este principio, ya que si se desea agregar una nueva figura,
        // se debe modificar la clase CalculadorDeAreas para agregar la nueva figura y su respectivo cálculo de área.
        public double CalcularArea(object figura)
        {
            if (figura is Circulo)
            {
                Circulo c = (Circulo)figura;
                return Math.PI * c.Radio * c.Radio;
            }
            else if (figura is Rectangulo)
            {
                Rectangulo r = (Rectangulo)figura;
                return r.Base * r.Altura;
            }
            return 0;
        }
    }
}
