using PackMeUp.Models.DTO;

namespace PackMeUp.Repositories.Enums
{
    public enum PackingItemChangeType
    {
        Insert,
        Update,
        Delete
    }

    public record PackingItemChange(
    PackingItemChangeType Type,
    PackingItemDTO Item
    );
}
