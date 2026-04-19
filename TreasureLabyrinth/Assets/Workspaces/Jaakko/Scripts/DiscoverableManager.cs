using System.Collections.Generic;
using Unity.VisualScripting;

public static class DiscoverableManager
{
    private static Dictionary<DiscoverableType, List<Discoverable>> m_discoverables = new();

    public static void Add(Discoverable d) 
    {
        if (d == null) return;

        if (!m_discoverables.TryGetValue(d.Type, out var list))
        {
            list = new List<Discoverable>();
            m_discoverables[d.Type] = list;
        }

        if (!list.Contains(d))
        {
            list.Add(d);
        }
    }
    public static void Remove(Discoverable d) 
    {
        if (d == null) return;

        if (m_discoverables.TryGetValue(d.Type, out var list))
        {
            list.Remove(d);

            if (list.Count == 0)
            {
                m_discoverables.Remove(d.Type);
            }
        }
    }
    public static List<Discoverable> GetDiscoverablesOfType(DiscoverableType type) 
    {
        if (!m_discoverables.TryGetValue(type, out var list)) 
        {
            return null;
        }
        return list;
    }
    public static void Clear() 
    {
        m_discoverables.Clear();
    }    
}
