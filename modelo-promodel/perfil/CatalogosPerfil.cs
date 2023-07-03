namespace promodel.modelo.perfil
{

    public static partial class Perfil { 
    
        public const string CAT_GENERO = "genero";
        public const string CAT_PAISES = "pais";
        public const string CAT_COLOR_OJOS = "colorojos";
        public const string CAT_ETNIA = "etnia";
        public const string CAT_COLOR_CABELLO = "colorcabello";
        public const string CAT_TIPO_CABELLO = "tipocabello";
        public const string CAT_ESTADOS_MX = "estadomx";
        public const string CAT_ESTADOS_US = "estadous";
        public const string CAT_IDIOMA = "idiomas";
        public const string CAT_ACTIVIDADES = "actividades";
        public const string CAT_TALLAS_VESTUARIO = "tallasvestuario";
        public const string CAT_AGENCIAS = "agencias";

        public static CatalogoBase NuevoCatalogo(string ClienteId, string Clave, List<ElementoCatalogo> Elementos)
        {
            CatalogoBase c = new CatalogoBase()
            {
                ClienteId = ClienteId,
                TipoPropiedad = Clave,
                Id = Guid.NewGuid().ToString(),
                Elementos = Elementos
            };

            return c;
        }


        public static List<ElementoCatalogo> CatalogoDeLista(this List<string> list, string? ClavePadre = null, string Idioma = "es-MX")
        {
            List<ElementoCatalogo> elementos = new List<ElementoCatalogo>();
            list.ForEach(i =>
            {
                elementos.Add(new ElementoCatalogo() { Clave = i.ToUpper().Split(':')[0], ClavePadre = ClavePadre?.ToUpper(), Idioma = Idioma, Texto = i.Split(':')[1].ToTitleCase() });
            });
            return elementos;
        }

        public static string ToTitleCase(this string str)
        {
            var tokens = str.Split(new[] { " ", "-" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];
                tokens[i] = token == token.ToUpper()
                    ? token
                    : token.Substring(0, 1).ToUpper() + token.Substring(1).ToLower();
            }

            return string.Join(" ", tokens);
        }

    }

}
