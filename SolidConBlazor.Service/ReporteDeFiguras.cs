using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidConBlazor.Service
{
    public class ReporteDeFiguras
    {
        // Single Responsibility Principle
        // Este principio establece que una clase debe tener una única responsabilidad o razón para cambiar.
        // En este código, la clase ReporteDeFiguras cumple con este principio, ya que su única responsabilidad es generar un reporte de figuras.
        public void GenerarReporte()
        {
            // Lógica para generar el reporte
            Console.WriteLine("Generando reporte de figuras...");
        }
    }
}
