using UnityEngine;

public class VFXDeath : MonoBehaviour
{
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject LoseMenu;
    [SerializeField]
    private GameObject StartPosition;
    [SerializeField]
    private GameObject[] BloodPrefab;
    [SerializeField]
    private GameObject VFXPrefab;
    [SerializeField]
    private float LifeTime;
    [SerializeField]
    private RopeSystem RopeSwinging;

    [SerializeField]
    public GameObject player;
    private Sound sound;

    private void Awake()
    {
        player = Instantiate(Player, StartPosition.transform.position, Quaternion.identity);
        sound = GetComponent<Sound>();
    }

    public void DeathEffect(Vector2 position)
    {
        var death = Instantiate(VFXPrefab, position, Quaternion.identity, transform);
        var blood = Instantiate(BloodPrefab[Random.Range(0, BloodPrefab.Length)], position, Quaternion.identity, transform);

        Destroy(death, LifeTime);
        Destroy(blood, LifeTime);
    }

    public void BloodEffect(Vector2 position)
    {
        var death = Instantiate(VFXPrefab, position, Quaternion.identity, transform);
        Destroy(death, LifeTime);
    }

    public void Death()
    {
        sound.PlayClip(sound.deathSound);
        Destroy(player);
        LoseMenu.SetActive(true);
    }

    public void Resurrection()
    {
        HealthSystem.HealphCount = 100;
        player = Instantiate(Player, StartPosition.transform.position, Quaternion.identity);
        LoseMenu.SetActive(false);
        Player.transform.position = StartPosition.transform.position;
        Player.SetActive(true);
    }
}