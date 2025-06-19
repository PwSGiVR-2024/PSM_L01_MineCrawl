using UnityEngine;
using TMPro;

public class SkillTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private GameObject okno;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        okno.gameObject.SetActive(false);
    }

    public void Show(string content)
    {
        tooltipText.text = content;
        okno.gameObject.SetActive(true);
    }

    public void Hide()
    {
        okno.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                transform.parent.GetComponent<RectTransform>(),
                Input.mousePosition,
                null,
                out pos);
            rectTransform.localPosition = pos + new Vector2(10, -10);
        }
    }
}
