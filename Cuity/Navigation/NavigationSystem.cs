using System;
using System.Collections.Generic;
using System.Text;

using Cuity.UI;

namespace Cuity.Navigation;

/// <summary>
/// Represent a stack-based navigator in the library.
/// </summary>
public class NavigationSystem: INavigator {
    private readonly Dictionary<string, NavigationTarget> m_routes = null!;
    private readonly Stack<Page> m_navigationFrame = null!;

    private readonly ISystemProvider m_provider = null!;

    public SystemBehavior Behavior { get => SystemBehavior.STATIC; }

    /// <summary>
    /// Current page of the application.
    /// </summary>
    internal Page Current { 
        get {
            if (!m_navigationFrame.TryPeek(out Page? page))
                return null!;

            if (!page.Tree.Any())
                page.CreateRenderSet();

            return page;
        } 
    }

    internal NavigationSystem(ISystemProvider provider) {
        m_provider = provider;

        m_navigationFrame = new Stack<Page>();
        m_routes = new Dictionary<string, NavigationTarget>();
    }

    /// <summary>
    /// Register route to the <see cref="NavigationSystem"/> with name.
    /// </summary>
    /// <param name="route">Route identifier of the page.</param>
    /// <param name="creationMethod">Page of the route.</param>
    /// <returns>Return <see langword="true"/>, if the route is successfully added to the <see cref="NavigationSystem"/>.</returns>
    internal bool Register(string route, Func<ISystemProvider, Page> creationMethod) {
        bool success = m_routes.TryAdd(route, new NavigationTarget(creationMethod, null!));
        if (m_navigationFrame.Count == 0 && success) {

            m_routes[route] = new NavigationTarget(Creation: null!, Page: m_routes[route].Creation(m_provider));
            m_navigationFrame.Push(m_routes[route].Page!);
        }

        return success;
    }

    /// <summary>
    /// Navigate to the specified <paramref name="page"/>.
    /// </summary>
    /// <param name="page">Creation method for the page.</param>
    public void NavigateTo(Func<ISystemProvider, Page> page) {
        Page target = page(m_provider);
        m_navigationFrame.Push(target);
    }

    public void NavigateTo(string route) {
        (Func<ISystemProvider, Page> creation, Page? page) = m_routes[route];
        page ??= creation(m_provider);

        m_navigationFrame.Push(page);
    }

    /// <summary>
    /// Navigate back to the previous <see cref="Page"/>.
    /// </summary>
    public void NavigateBack() => m_navigationFrame.Pop();
}