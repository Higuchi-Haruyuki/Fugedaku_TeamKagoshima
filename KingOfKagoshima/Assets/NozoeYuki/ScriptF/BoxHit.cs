using UnityEngine;

public class BoxHit : MonoBehaviour
{
    [SerializeField] private string areaName;

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log("標高～～～～～～～M");


        // UI 表示
       // AreaUIManager.Instance.ShowAreaUI(areaName);
    }

}
