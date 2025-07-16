using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{

    public static HitStop Instance;

    [Header("Time Settings")]
    public float stopTime = 0.1f;
    public float timeScaleRecoverySpeed = 0.1f;

    [Header("Camera Shake")]
    [SerializeField] private Transform shakeCam;
    public float shakeIntensity = 0.1f;
    public float shakeFrequency = 0.1f;

    private bool isHitStopped;
    private Vector3 originalCamPosition;
    private Coroutine shakeCoroutine;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void StopTime()
    {
        if (!isHitStopped)
        {
            isHitStopped = true;
            Time.timeScale = 0f;

            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);

            shakeCoroutine = StartCoroutine(ShakeCamera());
            StartCoroutine(ReturnTimeScale());
        }
    }

    private IEnumerator ShakeCamera()
    {
        originalCamPosition = shakeCam.localPosition;
        float elapsed = 0f;

        while (elapsed < stopTime)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity * stopTime;
            float y = Random.Range(-1f, 1f) * shakeIntensity * stopTime;

            shakeCam.localPosition = new Vector3(x, y, originalCamPosition.z);

            elapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(shakeFrequency);
        }

        shakeCam.localPosition = originalCamPosition;
    }

    private IEnumerator ReturnTimeScale()
    {


        yield return new WaitForSecondsRealtime(stopTime);

        while (Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, Time.unscaledDeltaTime * timeScaleRecoverySpeed);
            yield return null;
        }

        Time.timeScale = 1f;
        isHitStopped = false;
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);
    }
}
