﻿namespace BeaKona.AutoInterfaceGenerator;

internal static class ITypeSymbolExtensions
{
    public static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol @this)
    {
        foreach (ISymbol member in @this.GetMembers())
        {
            if (member is IMethodSymbol method)
            {
                if (method.MethodKind == MethodKind.Ordinary)
                {
                    yield return method;
                }
            }
        }
    }

    public static IEnumerable<IPropertySymbol> GetProperties(this ITypeSymbol @this)
    {
        foreach (ISymbol member in @this.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                if (property.IsIndexer == false)
                {
                    yield return property;
                }
            }
        }
    }

    public static IEnumerable<IPropertySymbol> GetIndexers(this ITypeSymbol @this)
    {
        foreach (ISymbol member in @this.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                if (property.IsIndexer)
                {
                    yield return property;
                }
            }
        }
    }

    public static IEnumerable<IEventSymbol> GetEvents(this ITypeSymbol @this)
    {
        foreach (ISymbol member in @this.GetMembers())
        {
            if (member is IEventSymbol @event)
            {
                yield return @event;
            }
        }
    }

    public static bool IsMemberImplemented(this ITypeSymbol @this, ISymbol member)
    {
        if (@this.FindImplementationForInterfaceMember(member) is IMethodSymbol memberImplementation)
        {
            return memberImplementation.ContainingType.Equals(@this, SymbolEqualityComparer.Default) && memberImplementation.MethodKind == MethodKind.ExplicitInterfaceImplementation;
        }

        return false;
    }

    public static ISymbol? FindImplementationForInterfaceMemberBySignature(this ITypeSymbol @this, ISymbol interfaceMember)
    {
        string name = interfaceMember.Name;
        if (string.IsNullOrEmpty(name) == false)
        {
            foreach (ISymbol member in @this.GetMembers(name).Where(i => i.Kind == interfaceMember.Kind))
            {
                var comparer = new ComparerBySignature();
                if (comparer.Equals(member, interfaceMember))
                {
                    return member;
                }
            }
        }

        return null;
    }

    public static bool IsMemberImplementedBySignature(this ITypeSymbol @this, ISymbol member)
    {
        return @this.FindImplementationForInterfaceMemberBySignature(member) != null;
    }

    public static bool IsAllInterfaceMembersImplementedBySignature(this ITypeSymbol @this, ITypeSymbol @interface)
    {
        if (@interface.TypeKind != TypeKind.Interface)
        {
            return false;
        }

        foreach (ISymbol member in @interface.GetMembers())
        {
            if (@this.IsMemberImplementedBySignature(member) == false)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsMatchByTypeOrImplementsInterface(this ITypeSymbol @this, ITypeSymbol @interface)
    {
        return @this.Equals(@interface, SymbolEqualityComparer.Default) || @this.AllInterfaces.Contains(@interface, SymbolEqualityComparer.Default);
    }
}
