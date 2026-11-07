using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float intensity = 0.08f;
    public float duration = 6f;

    private Vector3 originalPosition;

    public void StartShake()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(Shake());
    }

    public void StopShake()
    {
        StopAllCoroutines();
        transform.localPosition = originalPosition;
    }

    private IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 offset = Random.insideUnitSphere * intensity;
            transform.localPosition = originalPosition + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
