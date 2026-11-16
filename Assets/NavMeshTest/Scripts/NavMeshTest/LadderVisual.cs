using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// 梯子の見た目を管理するクラス。
/// </summary>
public class LadderVisual : MonoBehaviour
{
    private Vector3 initialScale;

    private float initialWorldHeight;

    private float initialBottomY;

    private bool isInitialized;

    private NavMeshLink navMeshLink;

    private void Awake()
    {
        Initialize();

        navMeshLink = GetComponent<NavMeshLink>();

        if (navMeshLink != null)
        {
            navMeshLink.enabled = false;
        }
    }

    private void Initialize()
    {
        if (isInitialized) return;

        initialScale = transform.localScale;

        Renderer renderer = GetComponentInChildren<Renderer>();

        if (renderer == null)
        {
            Debug.LogError($"{name}: 梯子にRendererがありません。", this);

            return;
        }

        initialWorldHeight = renderer.bounds.size.y;

        initialBottomY = renderer.bounds.min.y;

        if (initialWorldHeight <= 0.0001f)
        {
            Debug.LogError($"{name}: 梯子の元の高さが0に近すぎます。", this);

            return;
        }

        isInitialized = true;
    }


    /// <summary>
    /// 梯子の高さをワールド単位で設定する。
    /// </summary>
    /// <param name="targetHeight">目標とする高さ</param>
    public void SetHeight(float targetHeight)
    {
        if (!isInitialized) return;

        targetHeight = Mathf.Max(0f, targetHeight);

        float scaleMultiplier = targetHeight / initialWorldHeight;

        Vector3 newScale = initialScale;

        newScale.y = initialScale.y * scaleMultiplier;

        transform.localScale = newScale;

        Renderer renderer = GetComponentInChildren<Renderer>();

        if (renderer == null) return;

        float currentBottomY = renderer.bounds.min.y;

        float bottomOffset = initialBottomY - currentBottomY;

        transform.position += Vector3.up * bottomOffset;

        UpdateNavMeshLink(targetHeight);
    }

    /// <summary>
    /// はしごの高さに合わせてNavMeshLinkを更新する(Y座標だけ)
    /// </summary>
    public void UpdateNavMeshLink(float height)
    {
        if (navMeshLink == null) return;

        Vector3 start = navMeshLink.startPoint;
        Vector3 end = navMeshLink.endPoint;

        start.y = 0f;
        end.y = height;

        navMeshLink.startPoint = start;
        navMeshLink.endPoint = end;
    }

    /// <summary>
    /// はしご完成時にリンクを有効化する
    /// </summary>
    public void CompleteLadder()
    {
        if (navMeshLink == null) return;

        navMeshLink.enabled = true;
    }
}
