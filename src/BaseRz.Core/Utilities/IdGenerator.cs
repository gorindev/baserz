namespace BaseRz.Core.Utilities;

public static class IdGenerator
{
    private static long s_counter;

    public static string Next(string prefix = "baserz") =>
        $"{prefix}-{Interlocked.Increment(ref s_counter)}";
}
