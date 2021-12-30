namespace BlazorApp.Shared.Identity;

public class RoleRequest
{
    public string? Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}