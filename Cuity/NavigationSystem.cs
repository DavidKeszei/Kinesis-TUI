using System;
using System.Collections.Generic;
using System.Text;

using Cuity.UI;

namespace Cuity.Navigation;

/// <summary>
/// Represent a navigation 
/// </summary>
public class NavigationSystem {
    private static NavigationSystem m_instance = null!;

    private readonly Dictionary<string, (Func<Page> Method, Page? Page)> m_routes = null!;
    private readonly Stack<Page> m_navigationFrame = null!;

    /// <summary>
    /// Current instance of the <see cref="NavigationSystem"/>.
    /// </summary>
    public static NavigationSystem Instance { get => m_instance; }

    /// <summary>
    /// Current page of the application.
    /// </summary>
    internal Page Current { 
        get {
            if (!m_navigationFrame.TryPeek(out Page? page))
                return null!;

            if (!page!.UIElements.Any())
                page.CreateRenderSet();

            return page;
        } 
    }

    public NavigationSystem() {
        m_routes = new Dictionary<string, (Func<Page>, Page?)>();
        m_navigationFrame = new Stack<Page>();

        m_instance ??= this;
    }

    /// <summary>
    /// Register route to the <see cref="NavigationSystem"/> with name.
    /// </summary>
    /// <param name="route">Route indentifier of the page.</param>
    /// <param name="page">Page of the route.</param>
    /// <returns>Return <see langword="true"/>, if the route is successfully added to the <see cref="NavigationSystem"/>.</returns>
    internal bool Register(string route, Func<Page> page) {
        bool success = m_routes.TryAdd(route, (page, null!));
        if (m_navigationFrame.Count == 0 && success) {

            m_routes[route] = (null!, page());
            m_navigationFrame.Push(m_routes[route].Page!);
        }

        return success;
    }

    /// <summary>
    /// Navigate to the specified <paramref name="page"/>.
    /// </summary>
    /// <param name="page">Creation method for the page.</param>
    public void NavigateTo(Func<Page> page) {
        Page target = page();
        m_navigationFrame.Push(target);
    }

    public void NavigateTo(string route) {
        (Func<Page> creation, Page? page) = m_routes[route];
        page ??= creation();

        m_navigationFrame.Push(page);
    }

    /// <summary>
    /// Navigate back to the previosus <see cref="Page"/>.
    /// </summary>
    public void NavigateBack() => m_navigationFrame.Pop();
}
