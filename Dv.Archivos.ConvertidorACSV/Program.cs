namespace Dv.Archivos.ConvertidorACSV
{
    class Program
    {
        static void Main(string[] args)
        {
            // Verificar argumentos
            if (args.Length == 0)
            {
                Console.WriteLine("Uso: ConvertirNumeros.exe <archivo_entrada> [archivo_salida]");
                Console.WriteLine("Ejemplo: ConvertirNumeros.exe numeros.txt numeros_separados_por_comas.txt");
                return;
            }

            string archivoEntrada = args[0];
            string archivoSalida = args.Length > 1 ? args[1] : "numeros_separados_por_comas.txt";

            try
            {
                // Verificar si el archivo de entrada existe
                if (!File.Exists(archivoEntrada))
                {
                    Console.WriteLine($"Error: El archivo '{archivoEntrada}' no existe.");
                    return;
                }

                Console.WriteLine($"Leyendo archivo: {archivoEntrada}");

                // Leer todas las líneas del archivo
                string[] lineas = File.ReadAllLines(archivoEntrada);

                // Filtrar líneas vacías y espacios en blanco
                var numeros = lineas
                    .Where(linea => !string.IsNullOrWhiteSpace(linea))
                    .Select(linea => linea.Trim())
                    .ToList();

                Console.WriteLine($"Se encontraron {numeros.Count} números");

                // Unir todos los números con comas
                string resultado = string.Join(",", numeros);

                // Escribir el resultado al archivo de salida
                File.WriteAllText(archivoSalida, resultado);

                Console.WriteLine($"Archivo creado exitosamente: {archivoSalida}");
                Console.WriteLine($"Total de números procesados: {numeros.Count}");

                // Mostrar una vista previa del resultado
                string preview = resultado.Length > 100
                    ? resultado.Substring(0, 100) + "..."
                    : resultado;
                Console.WriteLine($"Vista previa: {preview}");

                Console.WriteLine("Proceso completado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar el archivo: {ex.Message}");
            }

            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
