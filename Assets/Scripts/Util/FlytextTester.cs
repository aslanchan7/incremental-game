using UnityEngine;

public class FlytextTester : MonoBehaviour
{
    [SerializeField] private Flytext flytextPrefab;
    [SerializeField] private float cooldown; 
    private float lastSpawnTime;
    

    void Update()
    {
        if (Time.time - lastSpawnTime >= cooldown)
        {
            lastSpawnTime = Time.time;
            Flytext flytext = Instantiate(flytextPrefab);
            flytext.Show("bullseye!", 1f, Vector2.up, Color.red);
        }
    }
}
