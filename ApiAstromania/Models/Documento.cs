namespace FileApiCore.Models
{
    public class Documento
    {
        public int IdDocumento { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }

        public IFormFile Archivo { get; set; }
    }
}
