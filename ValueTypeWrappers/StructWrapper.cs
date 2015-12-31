namespace ValueTypeWrappers
{
    /// <summary>
    /// Immutable wrapper around value types / structs
    /// </summary>
    /// <typeparam name="TPrimitive"></typeparam>
    public abstract class StructWrapper<TPrimitive> : TypeWrapper<TPrimitive> where TPrimitive : struct
    {
        protected StructWrapper(TPrimitive value) : base(value)
        {
        }
    }
}