namespace promodel.servicios.comunes
{
    public class RequestPaginado<T> where T : class
    {
        public T Request { get; set; }
        public bool? OrdernarASC { get; set; }
        public string? OrdenarPor { get; set; }
        public int Pagina { get; set; } =0;
        public int Tamano { get; set; } = 10;
        public bool Contar { get; set; } = true;
    }
}
