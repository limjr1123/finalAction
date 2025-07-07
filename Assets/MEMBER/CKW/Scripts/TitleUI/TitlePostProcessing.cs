using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightningEffect : MonoBehaviour
{
    [Header("Lightning Settings")]
    public Volume postProcessVolume;
    public float lightningInterval = 5f;
    public float lightningDuration = 0.2f;

    private ColorAdjustments colorAdjustments;
    private Bloom bloom;
    private Vignette vignette;

    void Start()
    {
        // 포스트 프로세싱 컴포넌트 가져오기
        if (postProcessVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments) == false)
        {
            colorAdjustments = postProcessVolume.profile.Add<ColorAdjustments>();
        }

        if (postProcessVolume.profile.TryGet<Bloom>(out bloom) == false)
        {
            bloom = postProcessVolume.profile.Add<Bloom>();
        }

        if (postProcessVolume.profile.TryGet<Vignette>(out vignette) == false)
        {
            vignette = postProcessVolume.profile.Add<Vignette>();
        }

        InvokeRepeating(nameof(TriggerLightning), lightningInterval, lightningInterval);
    }

    void TriggerLightning()
    {
        StartCoroutine(LightningFlash());
    }

    IEnumerator LightningFlash()
    {
        // 번개 효과 시작 - postExposure 사용 (밝기 조절)
        colorAdjustments.postExposure.value = 1.5f;  // 밝게
        colorAdjustments.contrast.value = 0.3f;
        bloom.intensity.value = 2f;
        vignette.intensity.value = 0.3f;

        yield return new WaitForSeconds(0.1f);

        // 잠깐 어두워지기
        colorAdjustments.postExposure.value = -0.5f;  // 어둡게

        yield return new WaitForSeconds(0.05f);

        // 다시 밝아지기
        colorAdjustments.postExposure.value = 2f;     // 매우 밝게
        bloom.intensity.value = 3f;

        yield return new WaitForSeconds(0.05f);

        // 원래 상태로 복귀
        colorAdjustments.postExposure.value = 1f;     // 원래 밝기
        colorAdjustments.contrast.value = 0f;
        bloom.intensity.value = 45f;
        vignette.intensity.value = 0.6f;
    }
}