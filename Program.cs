using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Text;

//Elaborado por: Sandra Eslava
//Fecha: 08/07/2021
//Se asume que los archivos estarán siempre ubicados en una  misma ruta

namespace ConsoleApp_Tranzact
{    
    class Program
    {
        static void Main(string[] args)
        {
           
            try
            {
                string path = @"C:\Users\sandr\Documents\Apps\ConsoleApp_Tranzact\Files\";
                List<LineGroup> listGroup = new List<LineGroup>();
                try
                {
                    //Buscar los archivos de las últimas 5 horas
                    List<string> filesFound = GetLatestFiles(path);

                    
                    foreach (var fileName in filesFound)
                    {
                        //Guardar cada línea del archivo                        
                        string[] lines = File.ReadAllLines(fileName);

                        foreach (string line in lines)
                        {                            
                           listGroup.Add(new LineGroup { domain_code = line.Split(" ")[0], page_title = line.Split(" ")[3], count_views = Int32.Parse(line.Split(" ")[2]) });
                        }
                    }

                    //Agrupar lista
                    if (listGroup.Count > 0)
                    {
                        var result = listGroup.GroupBy(d => d.domain_code)
                            .Select(
                                g => new
                                {
                                    domain_code = g.First().domain_code,
                                    page_title = g.First().page_title,
                                    count_views = g.Sum(s => s.count_views)
                                })
                            .OrderByDescending(a => a.count_views)
                            .ToList();

                        //Mostrar cada línea del resultado en pantalla
                        foreach (var resultLine in result)
                        {
                            Console.WriteLine(resultLine);
                        }
                    }
                    else {
                        Console.WriteLine("No se encontraron archivos en las últimas 5 horas");
                    }
                }
                catch (Exception UnauthorizedAccessException)
                {
                    Console.WriteLine("Error de autorización en ruta");
                }
               
            }

            catch (Exception e)
            {
                Console.WriteLine("Hubo un error: {0}", e.ToString());
            }

        }

        public static List<string> GetLatestFiles(string path)
        {
            //obtener las rutas de los archivos creados en las ultimas 5 horas
            List<string> listFiles = new List<string>();
            DirectoryInfo di = new DirectoryInfo(path);            
            foreach (var fi in di.GetFiles().Where(f => f.LastWriteTimeUtc > DateTime.Now.AddHours(-5)).ToArray())
            {
                //Console.WriteLine("path:" + path + fi.Name);
                listFiles.Add(path + fi.Name);
            }

            return listFiles;

        }

        //Objeto para almacenar datos agrupados de vistas
        public class LineGroup
        {
            public string domain_code { get; set; }
            public string page_title { get; set; }
            public Int32 count_views { get; set; }
           
        }


    }
}
