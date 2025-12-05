using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableObject : MonoBehaviour
{
    [SerializeField] private float speedMax;
    [SerializeField] private float speedMin;
    [SerializeField] private float heightMax;
    [SerializeField] private float heightMin;

    private float leftEdge;
    private float speed;
    private float height;

    private AudioSource audioSource;
    private ParticleSystem particleSystem;
    
    private Spawner spawner;

    public bool isDead = false;



    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();
        spawner = GetComponentInParent<Spawner>();
    }

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
        speed = Random.Range(speedMin, speedMax);
    }

    private void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < leftEdge)
        {
            Destroy(gameObject);
        }
    }

    public float GetHeight()
    {
        height = Random.Range(heightMin, heightMax);
        return height;
    }



    public void Destroy()
    {
        audioSource.Play();
        particleSystem.Play();
        isDead = true;
    }

    public void Die()
    {
        audioSource.Play();
        particleSystem.Play();
        isDead = true;
        Destroy(transform.GetChild(0).gameObject);
    }

    public void Activate(Player player)
    {
        audioSource.Play();
        transform.position = player.transform.position + Vector3.back;
        speed = 0.0f;
        transform.SetParent(player.transform, true);
    }

    public void ActivateTrap(Player player)
    {
        audioSource.Play();
        player.transform.position = transform.position;
        player.isCaptured = true;
        player.transform.SetParent(transform, true);
    }

    public void Deactivate()
    {
        particleSystem.Play();
        Destroy(transform.GetChild(0).gameObject);
        Destroy(this.gameObject, 3.0f);
    }

    public void DeactivateTrap()
    {
        particleSystem.Play();
        Destroy(transform.GetChild(0).gameObject);
        Destroy(this.gameObject, 3.0f);
    }

    private void OnDestroy()
    {
        spawner.spawnedObjects.Remove(gameObject);
    }



}
