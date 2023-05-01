namespace PathfinderCharacterSheet
{
    public interface IContextValue<T, Context>
    {
        T GetValue(Context context);
    }
}
