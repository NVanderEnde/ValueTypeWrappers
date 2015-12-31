namespace ValueTypeWrappers
{
    /// <summary>
    /// Immutable wrapper around strings
    /// </summary>
    public abstract class StringWrapper : TypeWrapper<string>
    {
        protected StringWrapper(string value) : base(value)
        {
        }
    }
}
