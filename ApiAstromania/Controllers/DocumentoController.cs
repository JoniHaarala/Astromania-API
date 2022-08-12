using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FileApiCore.Models;
using System.Data.SqlClient;
using System.Data;

namespace FileApiCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly string _rutaServidor;
        private readonly string _cadenaSql;
        public DocumentoController(IConfiguration config) {

            _rutaServidor = config.GetSection("Configuracion").GetSection("RutaServidor").Value;
            _cadenaSql = config.GetConnectionString("CadenaSQL");
        }


        //REFERENCIAS 
        //MODELS
        [HttpPost]
        [Route("Subir")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit = int.MaxValue, ValueLengthLimit = int.MaxValue)] //OMITIR ESTO
        public IActionResult Subir([FromForm]Documento objeto) {

            string rutaDocumento = Path.Combine(_rutaServidor, objeto.Archivo.FileName);

            try
            {
                //System.IO
                using (FileStream newFile = System.IO.File.Create(rutaDocumento))
                {
                    objeto.Archivo.CopyTo(newFile);
                    newFile.Flush();
                }

                //system.data.sqlclient
                using (var conexion = new SqlConnection(_cadenaSql)) {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_documento", conexion);
                    cmd.Parameters.AddWithValue("descripcion", objeto.Descripcion);
                    cmd.Parameters.AddWithValue("ruta", rutaDocumento);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Guardado" });
            }
            catch (Exception error) {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
               
        }

        [HttpGet]
        [Route("SetImg")]
        public IActionResult SetImg() {
            
            List<Documento> lista = new List<Documento>();
            
            try
            {
                ////System.IO
                //using (FileStream newFile = System.IO.File.Create(rutaDocumento))
                //{
                //    objeto.Archivo.CopyTo(newFile);
                //    newFile.Flush();
                //}

                //system.data.sqlclient
                using (var conexion = new SqlConnection(_cadenaSql))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_documento", conexion);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = cmd.ExecuteReader()) {
                        lista.Add(new Documento()
                        {
                            IdDocumento = reader.GetInt32("Id"),
                            Ruta = reader["Ruta"].ToString()
                        });
                    }
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = lista });
            }
            catch (Exception error)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message });
            }
        }
    }
}
