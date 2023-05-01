namespace PathfinderCharacterSheet
{
    public interface IPrototype<T>
    {
        T Clone { get; }
        T Fill(T other);
    }
}
