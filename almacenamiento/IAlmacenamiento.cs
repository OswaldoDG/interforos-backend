namespace almacenamiento
{
    public interface IAlmacenamiento
    {

        /// <summary>
        /// Crear un archivo en el folder padre
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="FilePath"></param>
        /// <param name="Name"></param>
        /// <param name="FolderId"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        Task<StorageObjectDescriptor> CreateFile(string ClientId, string FilePath, string Name, string FolderId, string? ContentType = null);

        /// <summary>
        /// Crea un folder en el folder padre, si el Id del folder padre es nulo se utiliza el id del folder compartido
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="Name"></param>
        /// <param name="ParentFolderId"></param>
        /// <returns></returns>
        Task<StorageObjectDescriptor> CreateFolder(string ClientId, string Name, string? ParentFolderId = null);


        /// <summary>
        /// Desacarga un archivo en base al Id
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<MemoryStream> DownloadFile(string ClientId, string FileId);


        /// <summary>
        /// Busca un folder en un subfolder, si el Id del folder padre es nulo se utiliza el id del folder compartido
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="Name"></param>
        /// <param name="ParentFolderId"></param>
        /// <returns></returns>
        Task<StorageObjectDescriptor?> FindFolder(string ClientId, string Name, string? ParentFolderId = null);


        /// <summary>
        /// Renombra un forlder existente
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="FolderId"></param>
        /// <param name="NewName"></param>
        /// <returns></returns>
        Task<StorageObjectDescriptor> RemameFolder(string ClientId, string FolderId, string NewName);

        string NombreValidoFolder(string Nombre);

        /// <summary>
        /// Elimina un archivo del contnido
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="FileId"></param>
        /// <returns></returns>
        Task<bool> DeleteFile(string ClientId, string FileId);


        /// <summary>
        /// Establece o elimina el acceso público a un contenido
        /// </summary>
        /// <param name="archivoId"></param>
        /// <param name="publico"></param>
        /// <returns></returns>
        Task AccesoPublico(string ClientId ,string archivoId, bool publico);
    }
}