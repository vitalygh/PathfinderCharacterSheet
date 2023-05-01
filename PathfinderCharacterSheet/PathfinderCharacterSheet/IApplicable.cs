namespace PathfinderCharacterSheet
{
    public interface IApplicable<T>
    {
        T Apply(T value);
    }
}
