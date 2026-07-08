using UnityEngine;

public class BoxHit : MonoBehaviour
{
    [SerializeField] private int areaName;
    [SerializeField] private MainUIDirector ma;
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        ma.SetHeight(areaName);

        ma.DisplayHeight(3);

        hasTriggered = true;

        // UI 表示
        // AreaUIManager.Instance.ShowAreaUI(areaName);
    }

}
