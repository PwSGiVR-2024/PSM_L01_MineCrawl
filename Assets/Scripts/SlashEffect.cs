using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    public float duration = 0.4f;
    public float moveDistance = 1f;
    public float fadeOutTime = 0.2f;
    public Sprite slashSprite; // mo¿na przypisaæ dynamicznie

    private SpriteRenderer sr;
    private float elapsed = 0f;
    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = slashSprite;
        sr.sortingOrder = 10;
        sr.color = new Color(1, 1, 1, 1);

        startPos = transform.position;
        endPos = startPos + new Vector3(Random.Range(-0.2f, 0.2f), moveDistance, 0);

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        transform.position = Vector3.Lerp(startPos, endPos, t);

        if (t > 1f - fadeOutTime / duration)
        {
            float fade = Mathf.InverseLerp(1f - fadeOutTime / duration, 1f, t);
            sr.color = new Color(1, 1, 1, 1 - fade);
        }

        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
