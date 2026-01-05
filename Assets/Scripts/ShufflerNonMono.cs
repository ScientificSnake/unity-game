public static class ShufflerNonMono
{
    private static System.Random _rng;

    static ShufflerNonMono()
    {
        _rng = new System.Random();
    }

    public static void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = _rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}