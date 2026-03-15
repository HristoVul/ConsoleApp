namespace ConsoleApp.Models;

public class StorageEntry<T> where T : class
{
    public T Value { get; set; }

    public DateTime UpdatedOn { get; set; }
}