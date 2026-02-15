using Cuity.Rendering;
using Cuity.UI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Cuity.UI;

/// <summary>
/// Represent a plain, tagged instance of the library.
/// </summary>
public class Entity {
    private readonly List<IComponent> m_components = null!;
    private readonly Dictionary<int, int> m_uniqueComponents = null!;

    private readonly string m_name = string.Empty;
    private int m_version = 0;

    private EntityState m_state = EntityState.LOCKED;

    /// <summary>
    /// Version of the entity.
    /// </summary>
    internal int Version { get => m_version; set => m_version = value; }

    /// <summary>
    /// Name of the entity.
    /// </summary>
    public string Name { get => m_name; init => m_name = value; }

    /// <summary>
    /// Indicates the <see cref="Entity"/> is lock state by the <see cref="Renderer"/>.
    /// </summary>
    internal EntityState State { get => m_state; set => m_state = value; }

    /// <summary>
    /// Create a new <see cref="Entity"/> with specific name.
    /// </summary>
    /// <param name="name">Name of the instance.</param>
    public Entity() {
        m_version = 0;

        m_components = new List<IComponent>();
        m_uniqueComponents = new Dictionary<int, int>();
    }

    /// <summary>
    /// Attach a(n) <typeparamref name="T"/> component to the current instance.
    /// </summary>
    /// <typeparam name="T">Type of the instance.</typeparam>
    /// <param name="component">Pre-defined value of the component. If this <see langword="null"/>, then the system creates a default component.</param>
    /// <param name="isUnique">Indicates the component is unique on the <see cref="Entity"/>.</param>
    /// <returns>Return <see langword="true"/> if the component is added to the entity. Otherwise return <see langword="false"/>.</returns>
    public bool AttachComponent<T>(T? component = null!, bool isUnique = false) where T: class, IComponent, IStaticType {
        if (component == null) return false;
        if(isUnique || component.IsType(RenderComponent.Name)) {
            if(!m_uniqueComponents.TryAdd(ComponentTypeProvider.QueryComponent(T.Name), m_components.Count))
                return false;
        }

        this.m_components.Add(component);
        ++m_version;

        return true;
    }

    /// <summary>
    /// Get a(n) <typeparamref name="T"/> component from the current instance.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <returns>Return <typeparamref name="T"/> component. If not exists, then return <see langword="null"/>.</returns>
    public virtual T? GetComponent<T>(int index = 0) where T: class, IComponent, IStaticType {
        if (m_uniqueComponents.TryGetValue(ComponentTypeProvider.QueryComponent(T.Name), out int i))
            return (T)m_components[i];

        int current = 0;
        foreach (IComponent component in m_components) {
            if (component.IsType(T.Name) && current++ == index)
                return component as T;
        }

        return default!;
    }

    /// <summary>
    /// Remove a component from the current <see cref="Entity"/>.
    /// </summary>
    /// <typeparam name="T">Type of the component.</typeparam>
    /// <param name="index">Indicates where we want delete the component.</param>
    public void RemoveComponent<T>(int index = 0) where T: class, IComponent, IStaticType {
        if (m_uniqueComponents.TryGetValue(key: ComponentTypeProvider.QueryComponent(T.Name), out int i)) {
            m_components.RemoveAt(i);
            m_uniqueComponents.Remove(key: ComponentTypeProvider.QueryComponent(name: T.Name));

            ++m_version;
            return;
        }

        int indexOf = 0;

        foreach (IComponent component in m_components) {
            if (component.IsType(T.Name) && ++indexOf == index) {
                m_components.RemoveAt(index);
                ++m_version;
                return;
            }
        }
    }

    /// <summary>
    /// Resolve all <see cref="IStyleComponent"/> instances, which attached to the current instance.
    /// </summary>
    /// <returns>Return a <see cref="IEnumerable{T}"/> instance.</returns>
    internal IEnumerable<IStyleComponent> ResolveStyles() {
        foreach(IComponent component in m_components) {
            if(component.IsType(type: Style.Name))
                yield return ((Style)component);
        }
    }

    public ComponentIterator GetEnumerator() => new ComponentIterator(components: m_components);
}

/// <summary>
/// Ref-like iterator for <see cref="IComponent"/> instances.
/// </summary>
public ref struct ComponentIterator {
    private List<IComponent> m_components = null!;
    private int m_current = -1;

    public IComponent Current { get => m_components[m_current]; }

    public ComponentIterator(List<IComponent> components)
        => m_components = components;

    public bool MoveNext() {
        if (++m_current < m_components.Count)
            return true;

        return false;
    }
}