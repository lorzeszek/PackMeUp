using PackMeUp.Models;

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
    PackingItem Item
    );
}
