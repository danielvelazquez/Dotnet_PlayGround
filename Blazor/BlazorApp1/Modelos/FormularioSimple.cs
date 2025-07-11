namespace BlazorApp1.Modelos
{
    public class FormularioSimple
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        //public string Telefono { get; set; } = string.Empty;
        //public DateTime FechaNacimiento { get; set; } = DateTime.Now;
        //public bool AceptaTerminos { get; set; } = false;
        public FormularioSimple() { }

        public FormularioSimple(string nombre, string apellido, string correoElectronico)//, string telefono, DateTime fechaNacimiento, bool aceptaTerminos)
        {
            Nombre = nombre;
            Apellido = apellido;
            CorreoElectronico = correoElectronico;
            //Telefono = telefono;
            //FechaNacimiento = fechaNacimiento;
            //AceptaTerminos = aceptaTerminos;
        }
    }
}
