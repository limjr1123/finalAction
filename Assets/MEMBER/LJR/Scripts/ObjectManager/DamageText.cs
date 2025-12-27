using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private Transform target; // 추적할 타겟
    private Vector3 offset; // 타겟 위 오프셋 (월드 좌표)
    private float moveSpeed = 100f; // 화면 좌표 기준 이동 속도 (픽셀/초)
    private float fadeSpeed = 1f; // 페이드 아웃 속도
    private float lifeTime = 1.5f; // 텍스트 생존 시간
    private float timer;
    private RectTransform rectTransform;
    private Vector2 animationOffset; // 애니메이션용 오프셋
    private Canvas canvas;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void Initialize(float damage, Transform target, Vector3 offset, bool isCritical, bool isHeal)
    {
        textMesh.text = isHeal ? "+" + damage.ToString(): "-" + damage.ToString();
        textMesh.color = isCritical ? Color.yellow : Color.red;
        textMesh.color = isHeal ? Color.green : Color.red;
        this.target = target;
        this.offset = offset;
        timer = 0f;
        animationOffset = Vector2.zero; // 애니메이션 오프셋 초기화
        gameObject.SetActive(true);
        UpdatePosition();
    }

    private void Update()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // 위치 업데이트
        UpdatePosition();

        // 애니메이션 오프셋으로 위로 이동
        animationOffset += Vector2.up * moveSpeed * Time.deltaTime;
        rectTransform.anchoredPosition = GetBaseAnchoredPosition() + animationOffset;

        // 페이드 아웃
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifeTime);
        textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);

        if (timer >= lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdatePosition()
    {
        // 월드 좌표를 화면 좌표로 변환
        Vector3 worldPosition = target.position + offset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // Canvas 스케일링을 고려해 anchoredPosition으로 변환
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.worldCamera,
            out anchoredPosition
        );
        rectTransform.anchoredPosition = anchoredPosition + animationOffset;
    }

    private Vector2 GetBaseAnchoredPosition()
    {
        Vector3 worldPosition = target.position + offset;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.worldCamera,
            out anchoredPosition
        );
        return anchoredPosition;
    }
}