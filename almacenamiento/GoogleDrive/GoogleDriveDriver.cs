using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using static Google.Apis.Requests.BatchRequest;


namespace almacenamiento.GoogleDrive;

public class GoogleDriveDriver : IAlmacenamiento
{

    private readonly IGoogleDriveConfigProvider provider;
    private readonly IConfiguration configuration;
    public GoogleDriveDriver(IGoogleDriveConfigProvider provider, IConfiguration configuration)
    {
        this.provider = provider;
        this.configuration = configuration;
    }

    /// <summary>
    /// Busca un folder en un subfolder, si el Id del folder padre es nulo se utiliza el id del folder compartido
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="Name"></param>
    /// <param name="ParentFolderId"></param>
    /// <returns></returns>
    public async Task<StorageObjectDescriptor?> FindFolder(string ClientId, string Name, string? ParentFolderId = null)
    {
        var cfg = await provider.GetConfig(ClientId);
        StorageObjectDescriptor f = new();
        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
        .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        if (ParentFolderId == null)
        {
            ParentFolderId = cfg.RootDirectory;
        }

        FilesResource.ListRequest listRequest = service.Files.List();
        listRequest.PageSize = 1;
        listRequest.Q = $"mimeType = 'application/vnd.google-apps.folder' and name = '{Name}' and '{ParentFolderId}' in parents";
        listRequest.Fields = "nextPageToken, files(id, name)";
        IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute().Files;

        if (files != null && files.Count > 0)
        {
            return new StorageObjectDescriptor() { Id = files[0].Id, Name = files[0].Name };
        }
        

        return null;
    }

    /// <summary>
    /// Renoambra un folder
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="FolderId"></param>
    /// <param name="NewName"></param>
    /// <returns></returns>
    public async Task<StorageObjectDescriptor> RemameFolder(string ClientId, string FolderId, string NewName)
    {

        var cfg = await provider.GetConfig(ClientId);
        StorageObjectDescriptor f = new();
        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
        .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = NewName,
            MimeType = "application/vnd.google-apps.folder",
        };

        var request = service.Files.Update(fileMetadata, FolderId);
        request.Fields = "id";
        var file = request.Execute();

        f.Id = file.Id;

        return f;

    }



    public async Task<bool> DeleteFile(string ClientId, string FileId)
    {

        var cfg = await provider.GetConfig(ClientId);
        StorageObjectDescriptor f = new();
        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
        .CreateScoped(DriveService.ScopeConstants.Drive);
        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        
        var request = service.Files.Delete(FileId);
        var result = request.Execute();
        return true;

    }

    /// <summary>
    /// Crea un folder en el folder padre, si el Id del folder padre es nulo se utiliza el id del folder compartido
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="Name"></param>
    /// <param name="ParentFolderId"></param>
    /// <returns></returns>
    public async Task<StorageObjectDescriptor> CreateFolder(string ClientId, string Name, string? ParentFolderId = null)
    {
        StorageObjectDescriptor f = new();
        var existingFolder = await FindFolder(ClientId, Name, ParentFolderId);
        if(existingFolder != null)
        {
            return existingFolder;

        } else
        {

            var cfg = await provider.GetConfig(ClientId);

            var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "promodel"
            });

            List<string> ParentFolder = new();
            if (ParentFolderId != null)
            {
                ParentFolder.Add(ParentFolderId);
            }
            else
            {
                ParentFolder.Add(cfg.RootDirectory);
            }


            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = Name,
                MimeType = "application/vnd.google-apps.folder",
                Parents = ParentFolder
            };

            var request = service.Files.Create(fileMetadata);
            request.Fields = "id";
            var file = request.Execute();

            f.Id = file.Id;

            return f;
        }



    }

    /// <summary>
    /// Crear un archivo en el folder padre
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="FilePath"></param>
    /// <param name="Name"></param>
    /// <param name="FolderId"></param>
    /// <param name="ContentType"></param>
    /// <returns></returns>
    public async Task<StorageObjectDescriptor> CreateFile(string ClientId, string FilePath, string Name, string FolderId, string? ContentType = null)
    {

        var cfg = await provider.GetConfig(ClientId);

        StorageObjectDescriptor f = new();

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        var fileMetadata = new Google.Apis.Drive.v3.Data.File()
        {
            Name = Name,
            Parents = new List<string>() { FolderId }
        };

        if (string.IsNullOrEmpty(ContentType))
        {
            ContentType = FilePath.GetMimeTypeForFileExtension();
        }

        FilesResource.CreateMediaUpload request;
        using (var stream = new FileStream(FilePath, FileMode.Open))
        {
            request = service.Files.Create(fileMetadata, stream, ContentType);
            request.Fields = "id";
            request.Upload();
        }

        var file = request.ResponseBody;
        f.Id = file.Id;
        f.Name = Name;
        f.Created = file.CreatedTime;

        return f;

    }

    /// <summary>
    /// Desacarga un archivo en base al Id
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<MemoryStream> DownloadFile(string ClientId, string fileId)
    {
        var cfg = await provider.GetConfig(ClientId);

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        var request = service.Files.Get(fileId);
        request.Fields = "parents";
        var response = request.Execute();
        response.Parents.ToList().ForEach(x => { Console.WriteLine($"{x}"); });
        var stream = new MemoryStream();

        // Add a handler which will be notified on progress changes.
        // It will notify on each chunk download and when the
        // download is completed or failed.
        request.MediaDownloader.ProgressChanged +=
            progress =>
            {
                switch (progress.Status)
                {
                    case DownloadStatus.Downloading:
                        {
                            Console.WriteLine(progress.BytesDownloaded);
                            break;
                        }
                    case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download complete.");
                            break;
                        }
                    case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                }
            };
        request.Download(stream);
        return stream;
    }



    /// <summary>
    /// Desacarga un archivo en base al Id
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<List<string>> ObtienePadre(string ClientId, string fileId)
    {
        Google.Apis.Drive.v3.Data.File response = null;
        var padre = new List<string>(); 
        var cfg = await provider.GetConfig(ClientId);

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        var request = service.Files.Get(fileId);
        request.Fields = "parents";
        try
        {
            response = request.Execute();
            padre = response.Parents.ToList();
        }
        catch
        {
        }
        return padre;     
    }

    /// <summary>
    /// Obtiene los metadatos de un archivo
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="fileId"></param>
    /// <returns></returns>
    public async Task<Google.Apis.Drive.v3.Data.File> getMetadatos(string ClientId, string fileId)
    {
        Google.Apis.Drive.v3.Data.File response=null;
        var cfg = await provider.GetConfig(ClientId);

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });

        var request = service.Files.Get(fileId);
        request.Fields = "*";
        try
        {
            response = request.Execute();
        }
        catch
        {
        }
       
        return response;

    }

    public string NombreValidoFolder(string Nombre)
    {
        string validos = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        string valido = "";
        var caracteres =  Nombre.ToUpper().ToCharArray();
        foreach(var car in caracteres)
        {
            if (validos.Any(c => c == car))
            {
                valido += car;
            } else
            {
                valido += "_";
            }
        }

        return valido;
    }
/// <summary>
/// Aaigna Acceso pusblico a un archivo en google drive
/// </summary>
/// <param name="ClientId"></param>
/// <param name="archivoId"></param>
/// <param name="publico"></param>
/// <returns></returns>
    public async Task AccesoPublico(string ClientId,string archivoId, bool publico)
    {

        var cfg = await provider.GetConfig(ClientId);

        StorageObjectDescriptor f = new();

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });
        if(publico)
        {
        Permission newPermission = new Permission();
        newPermission.Role = "reader";
        newPermission.Type = "anyone";
        try
        {
             service.Permissions.Create(newPermission, archivoId).Execute();
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        }

    }

    public async Task<string> ObtieneToken()
    {
        
        var cfg = await provider.GetConfig("");
        StorageObjectDescriptor f = new();
        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
        .CreateScoped(DriveService.ScopeConstants.Drive);

        var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync(null, CancellationToken.None);

        return token.ToString();
 
    
    }
    /// <summary>
    /// Asigna un canal de notificaciones para un archivo de drive 
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="archivoId"></param>
    /// <param name="publico"></param>
    /// <returns></returns>
    public async Task<Channel> WhatchArchivo(string ClientId, string archivoId)
    {
       
        var cfg = await provider.GetConfig(ClientId);

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });
        var channel = new Channel()
        { Id = Guid.NewGuid().ToString(),
            Type = "web_hook",
            Address = configuration.GetValue<string>("GoogledriveEventsWebHook")
        };
        var request = service.Files.Watch(channel,archivoId);   
        var response = request.Execute();
        return response;
    }

    /// <summary>
    /// elimina un canal de notificaciones para un archivo de drive 
    /// </summary>
    /// <param name="ClientId"></param>
    /// <param name="archivoId"></param>
    /// <param name="publico"></param>
    /// <returns></returns>
    public async Task DeleteWhatchArchivo(string ClientId, string canalId, string resorceId)
    {
        var cfg = await provider.GetConfig(ClientId);

        var credential = GoogleCredential.FromFile(cfg.AuthJsonPath)
            .CreateScoped(DriveService.ScopeConstants.Drive);

        var service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "promodel"
        });
        var channel = new Channel()
        {
            Id = canalId,
            ResourceId=resorceId,
        };
        var request = service.Channels.Stop(channel);
        request.Execute();
        
    }
}
