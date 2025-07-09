using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Service
{
    public  class ReporteDeFigurasNoSRP
    {
        // Single Responsibility Principle
        // Este principio establece que una clase debe tener una única responsabilidad o razón para cambiar.
        // En este código, la clase ReporteDeFigurasNoSRP no cumple con este principio, ya que tiene múltiples responsabilidades:
        // 1. Generar un reporte de figuras.
        // 2. Calcular el área de las figuras.
        // Esto hace que la clase sea difícil de mantener y modificar, ya que cualquier cambio en una de las responsabilidades puede afectar a la otra.
        public void GenerarReporte()
        {
            // Lógica para generar el reporte
            Console.WriteLine("Generando reporte de figuras...");
        }
        public double CalcularArea(object figura)
        {
            // Lógica para calcular el área de la figura
            return 0;
        }
    }
}
