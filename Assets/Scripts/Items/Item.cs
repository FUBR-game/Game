using Assets.Scripts;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public int index;
    public string name = "item";
    public Color hotBarColor = new Color(255, 255, 255, 0.2f);

    public Loot dropItem()
    {
        return null;
    }
}