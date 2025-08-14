namespace FunGame.Common.Helpers
{
    public static class EnumHelper
    {
        public static T[] GetValuesWithout<T>(this T excludedValue) where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Where(x => !x.Equals(excludedValue))
                .ToArray(); // Convert to array
        }
    }
}
