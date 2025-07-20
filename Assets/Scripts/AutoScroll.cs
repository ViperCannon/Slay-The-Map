using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollDuration = 5f;
    private bool isScrolling = false;

    void Start()
    {
        scrollRect.verticalNormalizedPosition = 1f;
    }

    public void StartAutoScroll()
    {
        if (!isScrolling)
        {
            StartCoroutine(AutoScrollCoroutine());
        }
    }

    private IEnumerator AutoScrollCoroutine()
    {
        isScrolling = true;


        scrollRect.enabled = false;
        scrollRect.verticalNormalizedPosition = 1f;

        float timeElapsed = 0f;

        // Scroll down to the bottom over the specified duration with ease-in-out acceleration
        while (timeElapsed < scrollDuration)
        {
            float normalizedPosition = Mathf.Lerp(1f, 0f, EaseInOut(timeElapsed / scrollDuration));
            scrollRect.verticalNormalizedPosition = normalizedPosition;

            timeElapsed += Time.deltaTime;
            yield return null;
        }


        scrollRect.verticalNormalizedPosition = 0f;
        scrollRect.enabled = true;

        isScrolling = false;
    }

    private float EaseInOut(float t)
    {
        if (t < 0.5f)
        {
            return 2 * t * t;
        }
        else
        {
            return -1 + (4 - 2 * t) * t;
        }
    }
}