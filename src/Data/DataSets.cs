namespace IslandGen.Data;

public static class DataSets
{
    private const string FemaleNamesFile = "assets/datasets/names_female.txt";
    private const string MaleNamesFile = "assets/datasets/names_male.txt";

    public static readonly List<string> FemaleNames = File.ReadAllLines(FemaleNamesFile).ToList();
    public static readonly List<string> MaleNames = File.ReadAllLines(MaleNamesFile).ToList();
}