
using Bogus;
using promodel.modelo.castings;
using promodel.modelo.proyectos;

namespace promodel.servicios.castings.Mock;

public class BogusService : IBogusService
{
    private List<Casting> listaCastings = new List<Casting>();
    private List<CastingListElement> listaCastingListElement= new List<CastingListElement>();
    private List<StaffCasting> listaStaffCasting=new List<StaffCasting>();
    private List<ComentarioCasting> listaComentariosCasting= new List<ComentarioCasting>();
    private List<CategoriaCasting> listaCategoriasCasting= new List<CategoriaCasting>();
    public List<CastingListElement> GenerarCastingListElementFicticios (int numeroCastings)
    {
   
        var bogusCasting = new Faker<Casting>()
                       .RuleFor(_ => _.ClienteId, f => f.GenerarIdCliente())
                       .RuleFor(_ => _.Nombre, f => f.Commerce.ProductName())
                       .RuleFor(_ => _.NombreCliente, f => f.Company.ToString())
                       .RuleFor(_ => _.FechaCreacionTicks, f => f.Person.DateOfBirth.Ticks)
                       .RuleFor(_ => _.UsuarioId, f => f.GenerarIdUsuario())
                       .RuleFor(_ => _.FechaApertura, f => f.Person.DateOfBirth)
                       .RuleFor(_ => _.FechaCierre, f => f.Person.DateOfBirth)
                       .RuleFor(_ => _.AceptaAutoInscripcion, f => f.Random.Bool())
                       .RuleFor(_ => _.Staff, f => GenerarStaffCasting(10))
                       .RuleFor(_ => _.Descripcion, f => f.Commerce.ProductDescription())
                       .RuleFor(_ => _.Comentarios, f => GenerarComentarioCastingFicticios(10))
                       .RuleFor(_ => _.ColaboradoresIds, f => f.GenerarColaboradores())
                       .RuleFor(_ => _.Activo, f => f.Random.Bool())
                       .RuleFor(_ => _.AperturaAutomatica, f => f.Random.Bool())
                       .RuleFor(_ => _.CierreAutomatico, f => f.Random.Bool())
                       .RuleFor(_ => _.Categorias, f =>GenerarCategoriasCastingFicticias(5) );

        for (int i = 0; i < numeroCastings; i++)
        {
            Casting castingGenerado = bogusCasting.Generate();
            listaCastings.Add(castingGenerado);
            listaCastingListElement.Add(castingGenerado.Castear());
        }

        return listaCastingListElement;
    }

        public List<StaffCasting> GenerarStaffCasting (int numeroStaff)
    {
        var bogusStaff = new Faker<StaffCasting>()
                       .RuleFor(_ => _.UsuarioId, f => Guid.NewGuid().ToString())
                       .RuleFor(_ => _.Email, f => f.Person.Email)
                       .RuleFor(_ => _.Confirmado, f => f.Random.Bool())
                       .RuleFor(_ => _.UltimoIngreso, f => f.Person.DateOfBirth);
      
        for (int i = 0; i < numeroStaff; i++)
        {
            StaffCasting staffGenerado = bogusStaff.Generate();
            listaStaffCasting.Add(staffGenerado);
        }

        return listaStaffCasting;
    }

    public List<ComentarioCasting> GenerarComentarioCastingFicticios(int numeroComentarios)
    {
        var bogusComentarioStaff = new Faker<ComentarioCasting>()
                       .RuleFor(_ => _.Id, f => Guid.NewGuid().ToString())
                       .RuleFor(_ => _.Fecha, f => f.Person.DateOfBirth)
                       .RuleFor(_ => _.UsuarioId, f => Guid.NewGuid().ToString())
                       .RuleFor(_ => _.Comentario, f => f.Lorem.Text());
               

        for (int i = 0; i < numeroComentarios; i++)
        {
            ComentarioCasting comentarioGenerado = bogusComentarioStaff.Generate();
            listaComentariosCasting.Add(comentarioGenerado);
        }

        return listaComentariosCasting;
    }

    public List<CategoriaCasting> GenerarCategoriasCastingFicticias(int numeroCategorias)
    {
        var bogusCategoriaStaff = new Faker<CategoriaCasting>()
                       .RuleFor(_ => _.Id, f => Guid.NewGuid().ToString())
                       .RuleFor(_ => _.Nombre, f => f.Commerce.Product())
                       .RuleFor(_ => _.Descripcion, f => f.Commerce.ProductDescription());
                       

        for (int i = 0; i < numeroCategorias; i++)
        {
            CategoriaCasting CategoriaGenerado = bogusCategoriaStaff.Generate();
            listaCategoriasCasting.Add(CategoriaGenerado);
        }

        return listaCategoriasCasting;
    }

    public Task<List<CastingListElement>> CreaDatosDemo()
    {
     return  Task.FromResult(GenerarCastingListElementFicticios(100));
    }
}