MIA_AzureBlob – Laboratorio No. 2, Semana 12: Cloud Storage
Nombre: Angela Santizo Carné: 1207925
Nombre> Eduardo Moran Carné: 1119525
1. Objetivo de la aplicación
Aplicación de consola en C# (.NET) que se conecta a Azure Blob Storage mediante una Connection String y permite realizar las cuatro operaciones básicas de manejo de archivos en la nube: subir, listar, descargar y eliminar.
2. Tecnologías utilizadas
.NET (C#) – aplicación de consola
Paquete Azure.Storage.Blobs (SDK oficial de Azure)
Paquete Microsoft.Extensions.Configuration.UserSecrets para manejo seguro de credenciales
Azure Blob Storage (Storage Account + Container)
3. Configuración de Azure
Storage Account: miaarchivos01 (Azure for Students, región ____________)
Container: archivos
Nivel de acceso: Privado (sin acceso anónimo)
La Connection String se obtuvo desde Storage Account → Seguridad y redes → Claves de acceso.
Nota: el enunciado original pide el container mia_archivos, pero Azure no permite guion bajo en nombres de containers (solo minúsculas, números y guiones), por lo que se usó archivos.
4. Arquitectura de la solución
Program.cs
 ├── BlobServiceClient   → conexión al Storage Account
 │     └── BlobContainerClient  → referencia al container "mia-archivos"
 │           └── BlobClient     → referencia a cada blob individual
 └── Menú de consola (switch) → despacha a cada operación (Subir, Listar, Descargar, Eliminar)
La aplicación sigue un flujo simple: al iniciar crea el BlobServiceClient a partir de la Connection String leída desde user-secrets, obtiene el BlobContainerClient del container mia-archivos (creándolo si no existe), y desde ahí despacha cada opción del menú a un método async independiente.
5. Descripción de las cuatro operaciones
Operación	Descripción
Subir archivo	Solicita la ruta local, valida que el archivo exista, obtiene el nombre y lo sube al container con UploadAsync, sobrescribiendo si ya existe.
Listar archivos	Recorre los blobs del container con GetBlobsAsync y muestra nombre y tamaño en bytes en formato de tabla.
Descargar archivo	Solicita el nombre del blob, valida su existencia con ExistsAsync, pide carpeta de destino y descarga con DownloadToAsync.
Eliminar archivo	Solicita el nombre del blob y lo elimina con DeleteIfExistsAsync, informando si existía o no.
6. Manejo de errores
Antes de subir, se valida que el archivo local exista con File.Exists(); si no existe, se informa al usuario y se cancela la operación.
Antes de descargar o eliminar un blob, se valida su existencia en Azure con ExistsAsync() / el resultado booleano de DeleteIfExistsAsync(), evitando excepciones innecesarias por blobs inexistentes.
Se validan datos vacíos o incompletos ingresados por el usuario (string.IsNullOrWhiteSpace) antes de intentar cualquier operación contra Azure.
7. Protección de la Connection String
La Connection String no se guardó en el código fuente ni en ningún archivo del repositorio. Se almacenó como variable de entorno local:
$env:AZURE_STORAGE_CONNECTION_STRING = "<CONNECTION_STRING>"
El programa la lee en tiempo de ejecución con Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING"), por lo que el valor nunca queda escrito en ningún archivo del proyecto ni se sube a GitHub. Adicionalmente se agregó .gitignore para excluir bin/, obj/ y cualquier archivo de configuración con credenciales.
Importante: la clave de acceso (AccountKey) usada durante las pruebas iniciales fue regenerada en el Portal de Azure tras haber quedado expuesta temporalmente en el código, como medida de seguridad.
8. Instrucciones para ejecutar el proyecto
git clone <URL_DEL_REPO>
cd MIA_AzureBlob
dotnet restore
$env:AZURE_STORAGE_CONNECTION_STRING = "<TU_CONNECTION_STRING>"
dotnet run
Luego seguir el menú en consola para subir, listar, descargar o eliminar archivos del container archivos.
9. Evidencias
(Adjuntar en el PDF de entrega, dentro de semana-12/lab2/)
[ ] Storage Account configurado
[ ] Container archivos creado como privado
[ ] Aplicación ejecutándose (menú)
[ ] Archivo antes de subirlo
[ ] Archivo visible en Azure Portal después de subirlo
[ ] Listado de archivos desde C#
[ ] Archivo descargado localmente
[ ] Archivo eliminado (confirmación en consola y en Portal)