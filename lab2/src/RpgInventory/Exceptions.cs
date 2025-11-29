using System;

namespace RpgInventory;

public class InventoryException : Exception
{
    public InventoryException() {}
    public InventoryException(string message) : base(message) {}
    public InventoryException(string message, Exception inner) : base(message, inner) {}
}

public class EquipException : InventoryException
{
    public EquipException(string message) : base(message) {}
}

public class UseException : InventoryException
{
    public UseException(string message) : base(message) {}
}

public class CombineException : InventoryException
{
    public CombineException(string message) : base(message) {}
}

