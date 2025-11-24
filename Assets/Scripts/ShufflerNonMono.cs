using UnityEngine;

public static class ShufflerNonMono
{ // yo this code may be ai but
    /// <summary>
    /// Shuffles a generic array using the Fisher-Yates algorithm.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to be shuffled.</param>
    public static void Shuffle<T>(T[] array)
    {
        // Use Unity's Random class for consistent behavior within the engine
        System.Random random = new();

        for (int i = array.Length - 1; i > 0; i--)
        {
            // Pick a random index from 0 to i
            int randomIndex = random.Next(0, i + 1);

            // Swap the element at the current index with the element at the random index
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
