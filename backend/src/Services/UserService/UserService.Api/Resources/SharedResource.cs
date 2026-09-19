namespace UserService.Api;

// Marker class — anchors SharedResource.resx for IStringLocalizer<SharedResource> resolution.
// Must live in the root namespace: the SDK pairs the same-named .resx to this class and embeds
// it as "UserService.Api.SharedResource", which is exactly what the localizer factory computes
// when no ResourcesPath is configured. A ".Resources" (or any) namespace suffix would break this.
public sealed class SharedResource;
