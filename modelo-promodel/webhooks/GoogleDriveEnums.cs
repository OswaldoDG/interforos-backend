using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace promodel.modelo.webhooks;

[JsonConverter(typeof(StringEnumConverter))]
public enum ReourceState {
    /// <summary>
    /// A channel was successfully created. You can expect to start receiving notifications for it.
    /// </summary>
    sync,
    /// <summary>
    /// A resource was created or shared.
    /// </summary>
    add,
    /// <summary>
    /// An existing resource was deleted or unshared
    /// </summary>
    remove,
    /// <summary>
    /// One or more properties (metadata) of a resource have been updated.
    /// </summary>
    update,
    /// <summary>
    /// A resource has been moved to the trash.
    /// </summary>
    trash,
    /// <summary>
    /// 	A resource has been removed from the trash.
    /// </summary>
    untrash,
    /// <summary>
    /// 	One or more changelog items have been added.
    /// </summary>
    change
}

[JsonConverter(typeof(StringEnumConverter))]
public enum ResourceChanges {
    /// <summary>
    /// The content of the resource has been updated.
    /// </summary>
    content,
    /// <summary>
    /// One or more properties of the resource have been updated.
    /// </summary>
    properties,
    /// <summary>
    /// One or more parents of the resource have been added or removed.
    /// </summary>
    parents,
    /// <summary>
    /// One or more children of the resource have been added or removed.
    /// </summary>
    children,
    /// <summary>
    /// The permissions of the resource have been updated.
    /// </summary>
    permissions
}