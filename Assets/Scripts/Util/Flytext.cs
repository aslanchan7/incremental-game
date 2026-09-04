using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class Flytext : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float animInTime = 0.2f;
    [SerializeField] private float animOutTime = 0.2f;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private Color defaultColor = Color.white;

    [Header("Rotation Settings")]
    [SerializeField] private float minXRot;
    [SerializeField] private float maxXRot;
    [SerializeField] private float minYRot;
    [SerializeField] private float maxYRot;
    [SerializeField] private float minZRot;
    [SerializeField] private float maxZRot;

    [Header("Spawn Offset Settings")]
    [SerializeField] private Vector3 maxOffset;

    [Header("References")]
    private TextMeshPro tmp;

    [Header("Variables")]
    private float displayDuration;
    private Vector3 moveDir;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        gameObject.SetActive(false); // start hidden
    }

    public void Show(string text, float duration, Vector3 dir, Color color)
    {
        tmp.text = text;
        if (color == null) color = defaultColor;
        tmp.color = color;
        // TODO: adjust size?
        // spawn offset
        transform.localPosition += GetRandomPositionOffset();
        // random rotation
        transform.localEulerAngles += GetRandomRotation();


        displayDuration = duration;
        moveDir = dir;

        gameObject.SetActive(true);
        
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        StartCoroutine(Pop());
        StartCoroutine(Float());

        // TODO: arc/gravity

        yield return new WaitForSeconds(displayDuration - animOutTime);
        yield return Fade();
        
        Destroy(gameObject);
    }

    private IEnumerator Pop()
    {
        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                float tween = BasicAnimations.EaseOutBack(t);
                transform.localScale = new(tween, tween);
            },
            null,
            animInTime
        );
    }

    private IEnumerator Float()
    {
        float deltaTime;
        float lastT = 0;
        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                deltaTime = t - lastT;
                transform.localPosition += deltaTime * floatSpeed * moveDir;
                lastT = t;
            },
            null,
            displayDuration
        );
    }

    private IEnumerator Fade()
    {
        yield return BasicAnimations.Interpolate(
            null,
            (t) =>
            {
                tmp.alpha = 1f-t;
            },
            null,
            animOutTime
        );
    }

    private Vector3 GetRandomPositionOffset()
    {
        float x = Random.Range(-maxOffset.x, maxOffset.x);
        float y = Random.Range(-maxOffset.y, maxOffset.y);
        float z = Random.Range(-maxOffset.z, maxOffset.z);
    
        return new Vector3(x, y, z);
    }

    private Vector3 GetRandomRotation()
    {
        float x = Random.Range(minXRot, maxXRot);
        float y = Random.Range(minYRot, maxYRot);
        float z = Random.Range(minZRot, maxZRot);

        return new Vector3(x, y, z);
    }
}
