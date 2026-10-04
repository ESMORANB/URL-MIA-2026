using DotNetEnv;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

// Carga el archivo .env (si existe). En GitHub Actions no hay .env,
// ahí la variable ya viene del Secret, así que esto no estorba.
Env.Load();

string connectionString =
	Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING")
	?? throw new InvalidOperationException(
		"Falta la variable de entorno AZURE_STORAGE_CONNECTION_STRING (¿se te olvidó el .env?).");
const string containerName = "archivos";

BlobServiceClient blobServiceClient = new(connectionString);
BlobContainerClient containerClient =
	blobServiceClient.GetBlobContainerClient(containerName);

await containerClient.CreateIfNotExistsAsync();

while (true)
{
	Console.Clear();
	Console.WriteLine("=================================");
	Console.WriteLine("     MIA - AZURE BLOB STORAGE");
	Console.WriteLine("=================================");
	Console.WriteLine("1. Subir archivo");
	Console.WriteLine("2. Listar archivos");
	Console.WriteLine("3. Descargar archivo");
	Console.WriteLine("4. Eliminar archivo");
	Console.WriteLine("5. Salir");
	Console.WriteLine("=================================");
	Console.Write("Seleccione una opción: ");

	switch (Console.ReadLine())
	{
		case "1":
			await SubirArchivoAsync();
			break;
		case "2":
			await ListarArchivosAsync();
			break;
		case "3":
			await DescargarArchivoAsync();
			break;
		case "4":
			await EliminarArchivoAsync();
			break;
		case "5":
			return;
		default:
			Console.WriteLine("Opción no válida.");
			break;
	}

	Console.WriteLine("\nPresione ENTER para continuar...");
	Console.ReadLine();
}

async Task SubirArchivoAsync()
{
	Console.Write("Ruta del archivo: ");
	string? ruta = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(ruta) || !File.Exists(ruta))
	{
		Console.WriteLine("El archivo no existe.");
		return;
	}

	string nombre = Path.GetFileName(ruta);
	BlobClient blob = containerClient.GetBlobClient(nombre);
	await blob.UploadAsync(ruta, overwrite: true);
	Console.WriteLine($"Archivo '{nombre}' subido correctamente.");
}

async Task ListarArchivosAsync()
{
	Console.WriteLine("\nArchivos almacenados:");
	bool hayArchivos = false;

	await foreach (BlobItem item in containerClient.GetBlobsAsync())
	{
		hayArchivos = true;
		Console.WriteLine($"- {item.Name} ({item.Properties.ContentLength ?? 0} bytes)");
	}

	if (!hayArchivos)
		Console.WriteLine("No hay archivos.");
}

async Task DescargarArchivoAsync()
{
	Console.Write("Nombre del archivo en Azure: ");
	string? nombre = Console.ReadLine();
	Console.Write("Ruta local de destino: ");
	string? destino = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(destino))
	{
		Console.WriteLine("Datos incompletos.");
		return;
	}

	BlobClient blob = containerClient.GetBlobClient(nombre);
	if (!await blob.ExistsAsync())
	{
		Console.WriteLine("El archivo no existe en Azure.");
		return;
	}

	await blob.DownloadToAsync(destino);
	Console.WriteLine("Archivo descargado correctamente.");
}

async Task EliminarArchivoAsync()
{
	Console.Write("Nombre del archivo a eliminar: ");
	string? nombre = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(nombre))
	{
		Console.WriteLine("Nombre no válido.");
		return;
	}

	BlobClient blob = containerClient.GetBlobClient(nombre);
	if (await blob.DeleteIfExistsAsync())
		Console.WriteLine("Archivo eliminado correctamente.");
	else
		Console.WriteLine("El archivo no existe.");
}