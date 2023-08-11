using promodel.modelo.proyectos;

namespace promodel.modelo.castings;

public class SelectorCategoria
{
    public string Id { get; set; }

    public string Nombre { get; set; }

    public List<string> Modelos { get; set; }

    public List<ComentarioCategoriaModeloCasting> Comentarios { get; set;}

}
