using IslandGen.Services;

namespace IslandGen.Extensions;

public static class ListExtensions
{
    /// <summary>
    ///     Returns a random item in the list
    /// </summary>
    /// <param name="list"> List we want to get a random item from </param>
    /// <typeparam name="T"> Type of the items in the list </typeparam>
    /// <returns> Randomly selected item from the list </returns>
    public static T RandomItem<T>(this List<T> list)
    {
        return list[ServiceManager.GetService<Random>().Next(list.Count)];
    }
}