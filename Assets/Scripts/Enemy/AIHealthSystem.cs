using UnityEngine;

public class AIHealthSystem : MonoBehaviour
{
    public int health = 10;
    public Transform player;
    public GameObject parent;
    public Animator anim;
    public bool killed;
    public GameObject[] damagePanels;
    public GameObject damagePopup;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            TakeDamage(2);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Instantiate(damagePopup, damagePanels[Random.Range(0, damagePanels.Length)].transform);
        if (health <= 0) Die();
    }

    private void Die()
    {
        anim.SetBool("falltodeath", true);
        DisableComponents();
        if (!killed)
        {
            player.GetComponent<PlayerHealth>().totalkills++;
            killed = true;
        }
        Destroy(transform.parent.gameObject,2);
    }

    private void DisableComponents()
    {
        var lineOfSight = parent.GetComponent<WaveEnemy>();
        if (lineOfSight != null)
        {
            lineOfSight.agent.speed = 0;
            lineOfSight.gun.GetComponent<Weapons>().StopAllCoroutines();
            lineOfSight.gun.GetComponent<Weapons>().enabled = false;
            lineOfSight.enabled = false;
        }
    }
}
