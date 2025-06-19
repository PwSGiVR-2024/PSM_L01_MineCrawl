using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TMP_Text textMesh;
    private float moveSpeed = 1f;
    private float fadeSpeed = 2f;
    private float lifetime = 1f;
    private Vector3 moveDirection;

    private float timer;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        moveDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1f, 0f); // lekki rozrzut
        timer = 0f;
        canvasGroup.alpha = 1f;  // reset alpha na start
    }

    void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
                if (canvasGroup.alpha <= 0)
                    Destroy(gameObject);
            }
        }
    }
}
