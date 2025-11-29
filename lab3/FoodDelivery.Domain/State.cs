
using System;
using System.Collections.Generic;
using System.Linq;

namespace FoodDelivery.Domain;

public class InvalidTransition : Exception
{
    public InvalidTransition(string message) : base(message) { }
}

/// <summary>Паттерн State: состояние заказа и допустимые переходы.</summary>
public abstract class OrderState
{
    public abstract string Name { get; }
    public abstract IEnumerable<Type> Allowed();

    public virtual void Enter(Order order)
    {
        order.AppendHistory(Name);
    }

    public void Transition(Order order, Type target)
    {
        if (!Allowed().Contains(target))
            throw new InvalidTransition($"Cannot transition from {Name} to {target.Name}");
        var newState = (OrderState)Activator.CreateInstance(target)!;
        order.SetState(newState);
        newState.Enter(order);
    }
}

public sealed class Created : OrderState
{
    public override string Name => "CREATED";
    public override IEnumerable<Type> Allowed() => new[] { typeof(Preparing), typeof(Cancelled) };
}

public sealed class Preparing : OrderState
{
    public override string Name => "PREPARING";
    public override IEnumerable<Type> Allowed() => new[] { typeof(Delivering), typeof(Cancelled) };
}

public sealed class Delivering : OrderState
{
    public override string Name => "DELIVERING";
    public override IEnumerable<Type> Allowed() => new[] { typeof(Completed), typeof(Cancelled) };
}

public sealed class Completed : OrderState
{
    public override string Name => "COMPLETED";
    public override IEnumerable<Type> Allowed() => Array.Empty<Type>();
}

public sealed class Cancelled : OrderState
{
    public override string Name => "CANCELLED";
    public override IEnumerable<Type> Allowed() => Array.Empty<Type>();
}
