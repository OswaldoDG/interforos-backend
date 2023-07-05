using Bogus;
using promodel.modelo.castings;
using promodel.modelo.proyectos;

namespace promodel.servicios.castings.Mock;

public static  class ExtencionesBogusService
{
    public static string GenerarIdCliente(this Faker faker)
    {
        return faker.Random.AlphaNumeric(faker.Random.Int(1, 32));
    }

    public static string GenerarIdUsuario(this Faker faker)
    {
        return "3300d9ee-3669-477b-bdc8-d75bdad8cbef";
    }
    public static List<string> GenerarColaboradores(this Faker faker)

    {

    return new List<string>() { "3300d9ee-3669-477b-bdc8-d75bdad8cbef", "3300d9ee-3669-477b-bdc8-d75bdad8cbef" };
    }
    public static CastingListElement Castear (this Casting casting)

    {

        return new CastingListElement() {
            Id= Guid.NewGuid().ToString(),
            Nombre=casting.Nombre,
            NombreCliente=casting.NombreCliente,
            FechaApertura=casting.FechaApertura,
            FechaCierre=casting.FechaCierre,
            AceptaAutoInscripcion=casting.AceptaAutoInscripcion,
            Activo=casting.Activo,
            AperturaAutomatica=casting.AperturaAutomatica,
            CierreAutomatico=casting.CierreAutomatico
        };
    }

}
