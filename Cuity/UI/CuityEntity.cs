using Cuity.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent a plain, tagged instance of the library.
/// </summary>
public class CuityEntity {
    private readonly string m_name = string.Empty;
    private readonly List<IComponent> m_components = null!;

    private readonly Dictionary<Type, int> m_uniqueComponents = null!;

    /// <summary>
    /// Name of the <see cref="CuityEntity"/> instance.
    /// </summary>
    public string Name { get => m_name; }

    /// <summary>
    /// Create a new <see cref="CuityEntity"/> with specific name.
    /// </summary>
    /// <param name="name">Name of the instance.</param>
    public CuityEntity(string name) {
        m_name = name;

        m_components = new List<IComponent>();
        m_uniqueComponents = new Dictionary<Type, int>();
    }

    /// <summary>
    /// Attach a(n) <typeparamref name="T"/> component to the current instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="component">Pre-defined value of the component. If this <see langword="null"/>, then the system creates a default component.</param>
    /// <param name="isUnique">Indicates the component is unique on the <see cref="CuityEntity"/></param>
    /// <exception cref="ArgumentException"/>
    public void AttachComponent<T>(T? component = null!, bool isUnique = false) where T: class, IComponent, new() {
        component ??= new T();

        if (isUnique && !m_uniqueComponents.TryAdd(component is IRenderable ? typeof(IRenderable) : typeof(T), m_components.Count))
            throw new ArgumentException(message: "The entity has a same component.");

        this.m_components.Add(component);
    }

    /// <summary>
    /// Get a(n) <typeparamref name="T"/> component from the current instance.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <returns>Return <typeparamref name="T"/> component. If not exists, then return <see langword="null"/>.</returns>
    public T? GetComponent<T>(int index = 0) where T: class, IComponent {
        if (m_uniqueComponents.TryGetValue(typeof(T), out int i))
            return m_components[i] as T;

        int current = 0;

        foreach (IComponent component in m_components) {
            if (component is T) {
                if(++current == index)
                    return component as T;
            }
        }

        return default!;
    }

    /// <summary>
    /// Remove a component from the current <see cref="CuityEntity"/>.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <param name="index">Indicates where we want delete the component.</param>
    public void RemoveComponent<T>(int index = 0) where T: class, IComponent {
        if (m_uniqueComponents.TryGetValue(typeof(T), out int i)) {
            m_components.RemoveAt(i);
            m_uniqueComponents.Remove(typeof(T));
            return;
        }

        int indexOf = 0;

        foreach (IComponent component in m_components) {
            if (component is T) {
                if (++indexOf == index) {
                    m_components.RemoveAt(index);
                    return;
                }
            }
        }
    }
}
