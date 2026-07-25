using UnityEngine;


public enum ItemType
    {

        None,

        RawGold,
        RawCoal,
        RawDiamond,

        ProcessedGold,
        ProcessedCoal,
        ProcessedDiamond

    }

public static class ItemTypeExtensions
{
   

    public static int GetPrice(this ItemType type)
    {
        switch (type)
        {
            case ItemType.ProcessedGold: return 150;
            case ItemType.ProcessedCoal: return 30; 
            case ItemType.ProcessedDiamond: return 500;
            default: return 0;
        }
    }

   
}

