﻿namespace PathsOfPower.Core.Models;

public class InventoryItem : IInventoryItem
{
    public string Name { get; set; }
    public InventoryItem()
    {
        Name = string.Empty;
    }
}
