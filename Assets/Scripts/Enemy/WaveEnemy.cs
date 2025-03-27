using UnityEngine;
using UnityEngine.AI;

public class WaveEnemy : MonoBehaviour
{
    public Transform player;
    public GameObject alert;
    public float visDistance = 20f;
    public float visAngle = 60f;
    public float shootDist = 10f;
    public float rotationSpeed = 5f;
    public NavMeshAgent agent;
    public Animator anim;
    public GameObject gun;
    public Vector3 offset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        Vector3 direction = player.position - transform.position;
        float angle = Vector3.Angle(direction, transform.forward);

        // Check if the player is within visibility distance and angle
        if (direction.magnitude < visDistance && angle < visAngle)
        {
            direction.y = 0; // We don't care about the height difference, just the horizontal angle
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotationSpeed);

            // If the player is within shooting distance, attack, otherwise chase
            if (direction.magnitude > shootDist)
            {
                ChasePlayer(direction); // Pass direction to ensure the AI chases in the correct direction
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            ChasePlayer(direction); // Pass direction to ensure the AI chases in the correct direction
        }
    }

    private void ChasePlayer(Vector3 direction)
    {
        alert.SetActive(true);
        anim.SetBool("run", true);
        anim.SetBool("attack", false);
        agent.speed = 1f;  // Can adjust the speed for chasing

        // Set agent's destination to player's position
        if (agent != null && player != null)
        {
            agent.SetDestination(player.position);
            agent.isStopped = false; // Ensure the agent isn't stopped
        }

        gun.GetComponent<Weapons>().StopAllCoroutines();
        gun.GetComponent<Weapons>().enabled = false;
    }

    private void AttackPlayer()
    {
        alert.SetActive(false);
        anim.SetBool("attack", true);
        anim.SetBool("run", false);
        agent.speed = 0; // Stop agent when attacking
        agent.isStopped = true; // Stop the agent while attacking
        gun.GetComponent<Weapons>().enabled = true;
    }

}
