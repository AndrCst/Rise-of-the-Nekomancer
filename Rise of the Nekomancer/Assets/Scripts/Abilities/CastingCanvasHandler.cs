
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CastingCanvasHandler : MonoBehaviour
{
    [SerializeField] private Slider castingSlider;
    [SerializeField] private TMP_Text castingText;
    private float fillTime;
    private bool okToFill;
    private float duration;
    private string castingString = "Casting...";
    private string channelingString = "Channeling...";

    public void HandleSlider(float castTime, bool isCasting)
    {
        HandleText(isCasting);
        duration = castTime;
        FillCoroutine = StartCoroutine(AllowSliderToFill(duration));
    }

     void HandleText(bool isCasting)
    {
        if (isCasting)
        {
            castingText.text = castingString;
        }
        else
        {
            castingText.text = channelingString;
        }
    }

    private void FixedUpdate()
    {
        FillSlider();
    }

    void FillSlider()
    {
        if (castingSlider.value < castingSlider.maxValue && okToFill)
        {
            castingSlider.value = Mathf.Lerp(0, castingSlider.maxValue, fillTime);

            fillTime += Time.deltaTime / duration;
        }
    }

    public Coroutine FillCoroutine;
    IEnumerator AllowSliderToFill(float duration)
    {
        castingSlider.value = 0;
        fillTime = 0;
        okToFill = true;
        castingSlider.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        castingSlider.gameObject.SetActive(false);
        okToFill = false;
    }
    
    public void HandleCastInterruption()
    {
            castingSlider.gameObject.SetActive(false);
            okToFill = false;
            StopCoroutine(FillCoroutine);
            FillCoroutine = null;
    }
}
