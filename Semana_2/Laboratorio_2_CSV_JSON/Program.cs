using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        // 1. Leer el archivo CSV
        string[] lineas = File.ReadAllLines("estudiantes.csv");

        // 2. Crear la lista de estudiantes
        List<Estudiante> estudiantes = new List<Estudiante>();

        // 3. Omitir la primera línea (encabezado) y procesar el resto
        for (int i = 1; i < lineas.Length; i++)
        {
            string[] datos = lineas[i].Split(',');

            Estudiante est = new Estudiante
            {
                Id = int.Parse(datos[0]),
                Nombre = datos[1],
                Carrera = datos[2].Trim()
            };

            estudiantes.Add(est);
        }

        // 4. Mostrar todos los estudiantes en consola
        foreach (Estudiante est in estudiantes)
        {
            Console.WriteLine($"{est.Id} - {est.Nombre} - {est.Carrera}");
        }

        // 5. Convertir la lista a formato JSON
        var opciones = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(estudiantes, opciones);

        // 6. Guardar el resultado en estudiantes.json
        File.WriteAllText("estudiantes.json", json);

        Console.WriteLine("Archivo estudiantes.json creado correctamente.");
    }
}
