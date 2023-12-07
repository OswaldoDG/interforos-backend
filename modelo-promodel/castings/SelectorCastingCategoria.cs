namespace promodel.modelo.castings;

public class SelectorCastingCategoria
{
    public string Id { get; set; }

    public string Nombre { get; set; }

    public List<SelectorCategoria> Categorias { get; set; }
    public List<MapaUsuarioNombre> Participantes { get; set; }
    public PermisosCasting PernisosEcternos { get; set; } = new PermisosCasting();
}