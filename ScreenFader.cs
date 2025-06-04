using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ScreenFader : MonoBehaviour
{
    private Image blackImage;

    void Start()
    {
        // BlackScreen 오브젝트에서 Image 컴포넌트 가져오기
        GameObject blackScreen = GameObject.Find("BlackScreen");
        blackImage = blackScreen.GetComponent<Image>();
    }

    //------------FadeOut------------//
    public void StartFadeOut(float duration, System.Action onComplete = null)
    {
        StartCoroutine(FadeOutRoutine(duration, onComplete));
    }

    private IEnumerator FadeOutRoutine(float duration, System.Action onComplete)
    {
        float t = 0f;
        Color color = blackImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);  // 0 -> 1로 알파 증가 (점점 불투명)
            blackImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        blackImage.color = new Color(color.r, color.g, color.b, 1f);  // 완전 불투명으로 세팅

        onComplete?.Invoke();  // 완료 콜백 호출
    }

    //------------FadeIn------------//
    public void StartFadeIn(float duration, System.Action onComplete = null)
    {
        StartCoroutine(FadeInRoutine(duration, onComplete));
    }

    private IEnumerator FadeInRoutine(float duration, System.Action onComplete)
    {
        float t = 0f;
        Color color = blackImage.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - t / duration);  // 1 -> 0으로 알파 감소 (투명해짐)
            blackImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        blackImage.color = new Color(color.r, color.g, color.b, 0f);  // 완전 투명으로 세팅

        onComplete?.Invoke();
    }
}