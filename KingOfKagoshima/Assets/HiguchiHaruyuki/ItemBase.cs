using System.Collections.Generic;
using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string IconPath {  get; protected set; }
    public int UseCount { get; protected set; }
    public void AddUseCount(int useCount) {  UseCount = useCount; }
    public virtual ItemBase Use()
    {
        UseCount--;
        return this;
    }
}
