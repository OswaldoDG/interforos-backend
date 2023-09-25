using promodel.modelo.proyectos;

namespace promodel.modelo.castings;

public class SelectorCategoria
{
    public string Id { get; set; }

    public string Nombre { get; set; }

    public List<ModeloOrdenable> Modelos { get; set; }

    public List<ComentarioCategoriaModeloCasting> Comentarios { get; set;}

    /// <summary>
    /// Devuelve una lista de los votos por persona en la categoría para ser utilziada como parte de los componentes del frontend
    /// </summary>
    public List<VotoModeloMapeo> Votos { get; set; }

}
