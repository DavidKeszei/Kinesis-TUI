using Cuity.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity;

internal static class ComponentTypeProvider {
    private static readonly Dictionary<string, int> m_registeredComponents = null!;

    static ComponentTypeProvider()
        => m_registeredComponents = new Dictionary<string, int>();

    internal static bool RegisterComponent<T>(string name) where T : IComponent 
        => m_registeredComponents.TryAdd(key: name, value: m_registeredComponents.Count);

    internal static int QueryComponent(string name) {
        if (m_registeredComponents.TryGetValue(name, out int id))
            return id;

        return -1;
    }
}
