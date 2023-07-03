using Bogus;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using promodel.modelo;
using promodel.modelo.perfil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bogus.DataSets.Name;

namespace promodel.servicios.personas
{
    public class PersonasDemo
    {

        public static List<Persona> CreaPersonas(int cuenta)
        {
            Randomizer.Seed = new Random(8675309);
            List<Persona> personas = new List<Persona>();
            var actividades = Perfil.ActividadesBase().Select(x => x.Clave).ToArray();
            var generos = Perfil.GeneroBase().Select(x => x.Clave).ToArray();
            var cojos = Perfil.ColorOjosBase().Select(x => x.Clave).ToArray();
            var etnias = Perfil.EtniaBase().Select(x => x.Clave).ToArray();
            var tcabellos = Perfil.TipoCabelloBase().Select(x => x.Clave).ToArray();
            var ccabellos = Perfil.ColorCabelloBase().Select(x => x.Clave).ToArray();
            var paises = new List<string>() { "MX", "US", "UK" }.ToArray();
            var idiomas = new List<string>() { "ES-MX", "EN-US", "EN-UK" }.ToArray();
            var estados = Perfil.EstadosMX().Select(x => x.Clave).ToArray(); 
            var extranjero = new List<bool>() { true, false }.ToArray();

            var fpersona = new Faker<Persona>()
                .RuleFor(o => o.Clientes,  new List<string>() { "67cf68458dde08365b7636f5d500098e" } )
                .RuleFor(o => o.Exclusivo, false)
                .RuleFor(o => o.Extranjero, f => f.Random.ArrayElement<bool>(extranjero))
                .RuleFor(o => o.PermisoTrabajo, f => f.Random.ArrayElement<bool>(extranjero))
                .RuleFor(o => o.PaisActualId,  f => f.Random.ArrayElement<string>(paises))
                .RuleFor(o => o.EstadoPaisId, f => f.Random.ArrayElement<string>(estados))
                .RuleFor(o => o.IdiomasIds, f => f.Random.ArrayElements<string>(idiomas, 1).ToList())
                .RuleFor(o => o.Relacion, TipoRelacionPersona.Yo)
                .RuleFor(o => o.UsuarioId, Guid.NewGuid().ToString())
                .RuleFor(o => o.PaisOrigenId, f => f.Random.ArrayElement<string>(paises))
                .RuleFor(o => o.FechaNacimiento, f => f.Date.Past(80))
                .RuleFor(o => o.ActividadesIds, f => f.Random.ArrayElements<string>(actividades, 3).ToList())
                .RuleFor(o => o.GeneroId, f=>f.Random.ArrayElement<string>(generos))
                .RuleFor(u => u.Nombre, (f, u) => f.Name.FirstName(  Gender.Male))
                .RuleFor(u => u.Apellido1, (f) => f.Name.LastName())
                .RuleFor(u => u.Apellido2, (f) => f.Name.LastName())
                .RuleFor(u => u.NombreArtistico, f => f.Internet.UserName());

            var fproFisicas = new Faker<PropiedadesFisicas>()
                .RuleFor(o => o.MKS, true)
                .RuleFor(o => o.Altura, f => f.Random.Int(140, 210))
                .RuleFor(o => o.Peso, f => f.Random.Int(48, 150))
                .RuleFor(o => o.ColorOjosId, f => f.Random.ArrayElement<string>(cojos))
                .RuleFor(o => o.ColorCabelloId, f => f.Random.ArrayElement<string>(ccabellos))
                .RuleFor(o => o.TipoCabelloId, f => f.Random.ArrayElement<string>(tcabellos))
                .RuleFor(o => o.EtniaId, f => f.Random.ArrayElement<string>(etnias));


            var fVEstuario = new Faker<PropiedadesVestuario>()
                .RuleFor(o => o.Pantalon, f => f.Random.Int(18, 60))
            .RuleFor(o => o.Playera, f => f.Random.Int(18, 60))
            .RuleFor(o => o.Calzado, f => f.Random.Int(5, 9))
            .RuleFor(o => o.TipoTallaId, "MX");

            for (int i = 0; i < cuenta; i++)
            {
                Persona p = fpersona.Generate();
                p.PropiedadesFisicas = fproFisicas.Generate();
                p.NombreBusqueda = ServicioPersonas.NombreBusuqedaPersona(p);
                p.PropiedadesVestuario = fVEstuario.Generate();
                p.PropiedadesFisicas.IMC = ServicioPersonas.IMC(p);
                p.TicksFechaNacimiento = p.FechaNacimiento.Value.Ticks;
                p.Id = $"00000000-0000-0000-0000-000000{i.ToString().PadLeft(6,'0')}" ;
                p.UsuarioId = p.Id;
                p.ElementoMedioPrincipalId = Guid.Empty.ToString();
                personas.Add(p);
            }

            return personas;

        }

    }
}
