using System.Collections.Generic;

public static class RandomID
{
    private static List<int> IDRepository = new List<int>();
    private static int id = 0;
    public static int GetConstID()
    {
        id = DataManager.Inst.GetConstId();
        IDRepository.Add(id);
        DataManager.Inst.SetConstID(id+1);
        return id++;
    }
    
    
}