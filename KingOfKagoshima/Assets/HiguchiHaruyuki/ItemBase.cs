using UnityEngine;

public class ItemBase : MonoBehaviour
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public string IconPath {  get; protected set; }
    public int UseCount { get; protected set; }
    public virtual ItemBase Effect(/*引数でプレイヤーを受け取りたい*/) { return this; }

}
