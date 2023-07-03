using promodel.modelo.perfil;

namespace promodel.servicios.perfil
{
    public interface IServicioCatalogos
    {

        Task<CatalogoBase> GetCatalogoCliente(string ClaveCatalogo, string ClienteId);
        Task<List<CatalogoBase>> GetCatalogosPerfil(string ClienteId);
        Task UpdateCatalogo(CatalogoBase c);

        Task<ElementoCatalogo> BuscaCrea(string ClienteId, string ClaveCatalogo, string Texto);

    }
}
