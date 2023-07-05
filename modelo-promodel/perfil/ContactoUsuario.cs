namespace promodel.modelo.perfil;

public class ContactoUsuario
{

    /// <summary>
    /// Identificador único del usuario en el sistema
    /// </summary>
    public string? Id { get; set; }


    /// <summary>
    /// Email de usuario
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Nombre completo 
    /// </summary>
    public string? NombreCompleto { get; set; }


    /// <summary>
    /// Tipo de rol asignado al contacto
    /// </summary>
    public TipoRolCliente? Rol { get; set; }

    /// <summary>
    /// Indioca si el contacto ha sido localizado como usuario del sistema
    /// </summary>
    public bool Localizado { get; set; }
}
