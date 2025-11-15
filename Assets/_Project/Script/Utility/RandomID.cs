using System.Collections.Generic;

public static class RandomID
{
    private static List<int> IDRepository = new List<int>();
    private static int id = 0;
    public static int GetConstID()
    {
        IDRepository.Add(id);
        return id++;
    }
}