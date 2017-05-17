namespace Lykke.Core
{
    public interface IEntity
    {
        string RowKey { get; set; }

        string ETag { get; set; }
    }
}