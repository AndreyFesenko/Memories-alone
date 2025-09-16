//C:\_C_Sharp\MyOtus_Prof\Memories_alone\src\IdentityService\IdentityService.Domain\Entities\Role.cs
namespace IdentityService.Domain.Entities;

public class Role
{
    public Guid Id { get; set; }

    /// <summary>
    /// Имя роли (например: "AccountHolder")
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Имя роли в верхнем регистре для поиска/уникальности (например: "ACCOUNTHOLDER")
    /// </summary>
    public string NormalizedName { get; set; } = null!;

    /// <summary>
    /// Описание роли
    /// </summary>
    public string? Description { get; set; }
}