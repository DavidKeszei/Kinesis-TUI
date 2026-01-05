using System;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent a plain, tagged instance of the library.
/// </summary>
public class CuityObject {
    private readonly string m_name = string.Empty;
    private readonly List<IComponent> m_components = null!;

    /// <summary>
    /// Name of the <see cref="CuityObject"/> instance.
    /// </summary>
    public string Name { get => m_name; }

    /// <summary>
    /// Create a new <see cref="CuityObject"/> with specific name.
    /// </summary>
    /// <param name="name">Name of the instance.</param>
    public CuityObject(string name) {
        m_name = name;
        m_components = new List<IComponent>();
    }

    /// <summary>
    /// Attach a(n) <typeparamref name="T"/> component to the current instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="component">Pre-defined value of the component. If this <see langword="null"/>, then the system creates a default component.</param>
    public void AttachComponent<T>(T? component = default) where T: class, IComponent, new() {
        component ??= new T();
        this.m_components.Add(component);
    }

    /// <summary>
    /// Get a(n) <typeparamref name="T"/> component from the current instance.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <returns>Return <typeparamref name="T"/> component. If not exists, then return <see langword="null"/>.</returns>
    public T? GetComponent<T>() where T: class, IComponent {
        foreach (IComponent component in m_components) {
            if (component is T)
                return component as T;
        }

        return default!;
    }

    /// <summary>
    /// Remove a component from the current <see cref="CuityObject"/>.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <param name="index">Indicates where we want delete the component.</param>
    public void RemoveComponent<T>(int index = 0) where T: class, IComponent {
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
