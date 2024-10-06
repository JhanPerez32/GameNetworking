using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("World Space Health bar")]
    public Slider WorldSpaceHealthBar;
    public Canvas HealthBarContainer;

    [Header("UI Health bar")]
    public Slider UIHealthBar;

    private PlayerHealth Health;

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
        Health.OnDamageEvent += OnHealthBarDamaged;

        if (WorldSpaceHealthBar)
        {
            WorldSpaceHealthBar.value = 1;
        }
    }
    private void LateUpdate()
    {
        if (HealthBarContainer)
            HealthBarContainer.transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                Camera.main.transform.rotation * Vector3.up);
    }

    private void OnHealthBarDamaged(float percent)
    {
        StopAllCoroutines();

        if (WorldSpaceHealthBar)
        {
            WorldSpaceHealthBar.value = percent;
        }

        if (UIHealthBar)
        {
            UIHealthBar.value = percent;
        }

        StartCoroutine(ShowHealthBar(1f));
    }

    private IEnumerator ShowHealthBar(float hideDelay)
    {
        CanvasGroup canvasGroup = HealthBarContainer.GetComponent<CanvasGroup>();
        if (canvasGroup)
        {
            canvasGroup.alpha = 1;  // Make sure it's visible
            yield return new WaitForSeconds(hideDelay);
            canvasGroup.alpha = 0;  // Hide it after the delay
        }
    }
}
