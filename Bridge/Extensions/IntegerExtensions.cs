namespace ToP.Bridge.Extensions
{
    public static class IntegerExtensions
    {
        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }
    }
}