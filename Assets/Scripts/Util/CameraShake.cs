using System.Collections;
using UnityEngine;

/// <summary>
/// This is made by Claude, so if it's bad, blame Claude.
/// </summary>
public class CameraShake : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilRecoverySpeed = 8f;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionNoiseFrequency = 25f;

    private Vector3 originalLocalPos;
    private Coroutine activeShakeCoroutine;

    // Recoil-specific state
    private Vector3 recoilOffset;
    private Vector3 recoilVelocity; // used for SmoothDamp

    private void Awake() {
        if (cameraTransform == null) cameraTransform = transform;
        originalLocalPos = cameraTransform.localPosition;
    }

    /// <summary>
    /// Sharp directional kick that recovers smoothly. Good for gunfire/hits.
    /// </summary>
    public void Recoil(Vector2 direction, float strength, Transform targetTransform) {
        direction.Normalize();
        recoilOffset += (Vector3)(direction * strength);

        if (activeShakeCoroutine == null) {
            activeShakeCoroutine = StartCoroutine(RecoilUpdateLoop(targetTransform));
        }
    }

    /// <summary>
    /// Omnidirectional decaying shake. Good for explosions/impacts.
    /// magnitude: how strong the shake is at its peak.
    /// duration: how long the shake lasts before fully decaying.
    /// </summary>
    public void Explosion(float magnitude, float duration) {
        if (activeShakeCoroutine != null) StopCoroutine(activeShakeCoroutine);
        activeShakeCoroutine = StartCoroutine(ExplosionRoutine(magnitude, duration));
    }

    private IEnumerator RecoilUpdateLoop(Transform targetTransform) {
        Vector3 origLocalPos = targetTransform.localPosition;
        while (recoilOffset.sqrMagnitude > 0.0001f) {
            recoilOffset = Vector3.SmoothDamp(recoilOffset, Vector3.zero, ref recoilVelocity, 1f / recoilRecoverySpeed);
            targetTransform.localPosition = origLocalPos + recoilOffset;
            yield return null;
        }
        recoilOffset = Vector3.zero;
        targetTransform.localPosition = origLocalPos;
        activeShakeCoroutine = null;
    }

    private IEnumerator ExplosionRoutine(float magnitude, float duration) {
        float elapsed = 0f;
        float seedX = Random.Range(0f, 100f);
        float seedY = Random.Range(0f, 100f);

        while (elapsed < duration) {
            elapsed += Time.deltaTime;

            // Decay the shake strength over time (starts strong, fades to 0)
            float damper = 1f - Mathf.Clamp01(elapsed / duration);

            // Perlin noise gives smooth, organic randomness instead of jittery Random.Range per-frame
            float offsetX = (Mathf.PerlinNoise(seedX, Time.time * explosionNoiseFrequency) - 0.5f) * 2f;
            float offsetY = (Mathf.PerlinNoise(seedY, Time.time * explosionNoiseFrequency) - 0.5f) * 2f;

            Vector3 explosionOffset = new Vector3(offsetX, offsetY, 0f) * magnitude * damper;
            cameraTransform.localPosition = originalLocalPos + explosionOffset;

            yield return null;
        }

        cameraTransform.localPosition = originalLocalPos;
        activeShakeCoroutine = null;
    }
}