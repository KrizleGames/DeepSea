using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class Player : MonoBehaviour
{
    [SerializeField] private Spawner fishSpawner;
    [SerializeField] private Spawner skySpawner;
    [SerializeField] private Spawner waterSpawner;
    [SerializeField] private Spawner groundSpawner;


    private GameManager gameManager;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;

    private Animator animator;
    private MultiplierText multiplierText;
    private AirBar airBar;


    public Vector3 direction;
    public float gravity = 9.8f;
    public float strength = 5f;


    public bool isDead = true;
    public bool isUnderWater = false;
    
    public int pointMultiplier = 1;
    public float airMultiplier = 1; //

    public bool infiniteAir = false;
    public bool undestroyable;

    public bool isCaptured = false;
    public float trapForce = 0;

    

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioSource = GetComponent<AudioSource>();
        particleSystem = GetComponent<ParticleSystem>();

        animator = GetComponentInChildren<Animator>();
        multiplierText = GetComponentInChildren<MultiplierText>();
        airBar = GetComponentInChildren<AirBar>();
    }



    private void Update()
    {
        if (isDead) return;

        if (this.gameObject.transform.position.y >= 0)
        {

            gameManager.IncreaseAir(Time.deltaTime);
            isUnderWater = false;
        }
        else
        {
            isUnderWater = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && !isCaptured)
        {
            if (!infiniteAir)
            {
                gameManager.DecreaseAir();
            }
            direction = Vector3.down * strength;
        }

        /*
        direction.y += gravity * Time.deltaTime;
        transform.position += direction * Time.deltaTime;
        float height = Mathf.Clamp(transform.position.y, -8, 0); 
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        */
    }

    private void FixedUpdate()
    {
        if (isCaptured) return;
        //if (!isUnderWater) return;

        /*
        if (hasAnchor)
        {
            rigidbody.AddForce(Vector2.down * trapForce, ForceMode2D.Impulse);
            hasAnchor = false;
            trapForce = 0;
        }
        */

        direction.y += (gravity + trapForce) * Time.fixedDeltaTime;
        transform.position += direction * Time.fixedDeltaTime;
        float height = Mathf.Clamp(transform.position.y, -7, 0); //-8
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
        if (height >= 0 || height <= -7)
        {
            direction = Vector3.zero;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.tag == "Enemy")
        {
            if (!undestroyable)
            {
                PlayerDie();
            }
            else
            {
                Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
                enemy.Die();
            }
        }

        else if (collision.gameObject.tag == "Fish")
        {
            Fish fish = collision.gameObject.GetComponentInParent<Fish>();
            gameManager.IncreaseScore(fish.points * pointMultiplier);
            fish.Die();
        }

        else if (collision.gameObject.tag == "Bonus")
        {
            Bonus bonus = collision.gameObject.GetComponentInParent<Bonus>();

            if (bonus.air > 0)
            {
                bonus.Activate(this);
                gameManager.ReloadAir(bonus.air);
                bonus.Deactivate();
            }
            else if (bonus.air < 0)
            {
                StartCoroutine(AirConstant(bonus));
            }
            else if (bonus.airMultiplier > 0)
            {
                StartCoroutine(AirMultiplier(bonus));
            }
            else if (bonus.points > 0)
            {
                bonus.Activate(this);
                gameManager.IncreaseScore(bonus.points * pointMultiplier);
                bonus.Deactivate();
            }
            else if (bonus.pointMultiplier > 0)
            {
                StartCoroutine(PointMultiplier(bonus));
            }
            else if (bonus.power)
            {
                StartCoroutine(ActivatePower(bonus));
            }
            else if (bonus.sonar)
            {
                StartCoroutine(ActivateSonar(bonus));
            }
            else if (bonus.bomb)
            {
                StartCoroutine(ActivateBombing(bonus));
            }
        }

        else if (collision.gameObject.tag == "Trap")
        {
            Trap trap = collision.gameObject.GetComponentInParent<Trap>();

            if (trap.force > 0)
            {
                StartCoroutine(Hook(trap));
            }
            else if (trap.force < 0)
            {
                StartCoroutine(Anker(trap));
            }
            else if (trap.escape > 0)
            {
                StartCoroutine(Captured(trap));
            }
            else if (trap.spin)
            {
                StartCoroutine(Spin(trap));
            }
            else if (trap.wave)
            {
                StartCoroutine(Wave(trap));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.tag == "Boat")
        {
            Boat boat = collision.gameObject.GetComponentInParent<Boat>();
            boat.Destroy();
        }
    }



    public void PlayerDie()
    {
        isDead = true;
        this.particleSystem.Play();
        this.audioSource.Play();
        this.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.gameObject.GetComponentInChildren<AirBar>().gameObject.SetActive(false);
        this.gameObject.GetComponentInChildren<MultiplierText>().gameObject.SetActive(false);
        
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        StopAllCoroutines();

        gameManager.PlayerDead();
    }



    //Bonus
    private IEnumerator AirConstant(Bonus bubble)
    {
        bubble.Activate(this);
        infiniteAir = true;
        airBar.ChangeColor(Color.blue);
        yield return new WaitForSeconds(5f);
        airBar.ChangeColor(Color.white);
        infiniteAir = false;
        bubble.gameObject.GetComponent<AudioSource>().Play();
        bubble.Deactivate();
    }

    private IEnumerator AirMultiplier(Bonus bonus)
    {
        bonus.Activate(this);
        float multiplier = bonus.airMultiplier;
        bonus.Deactivate();
        gameManager.airSpeed *= multiplier;
        airBar.ChangeColor(Color.cyan);
        yield return new WaitForSeconds(3f);
        airBar.ChangeColor(Color.white);
        gameManager.airSpeed /= multiplier;
    }

    private IEnumerator PointMultiplier(Bonus bonus)
    {
        bonus.Activate(this);
        int multiplier = bonus.pointMultiplier;
        bonus.Deactivate();
        pointMultiplier *= multiplier;
        multiplierText.UpdateText(pointMultiplier);
        yield return new WaitForSeconds(5f);
        pointMultiplier /= multiplier;
        multiplierText.UpdateText(pointMultiplier);
    }


    private IEnumerator ActivatePower(Bonus power)
    {
        power.Activate(this);
        while (undestroyable == true)
        {
            yield return null;
        }
        this.undestroyable = true;
        animator.SetBool("isPowerOn", this.undestroyable);
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1.0f);
            power.gameObject.GetComponent<AudioSource>().Play();
        }
        this.undestroyable = false;
        animator.SetBool("isPowerOn", this.undestroyable);
        power.Deactivate();
    }

    private IEnumerator ActivateSonar(Bonus sonar)
    {
        sonar.Activate(this);
        yield return new WaitForSeconds(2.0f);
        List<GameObject> currentFishs = new List<GameObject>(fishSpawner.spawnedObjects);
        foreach (GameObject gameObject in currentFishs)
        {
            if (gameObject != null)
            {
                Fish fish = gameObject.GetComponent<Fish>();
                if (!fish.isDead)
                {
                    gameManager.IncreaseScore(fish.points * pointMultiplier);
                    fish.Die();
                    Destroy(gameObject.transform.GetChild(0).gameObject);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        sonar.Deactivate();
    }

    private IEnumerator ActivateBombing(Bonus bonus)
    {
        bonus.Activate(this);
        bonus.Deactivate();
        yield return new WaitForSeconds(1.0f);
        List<GameObject> currentEnemies = new List<GameObject>(waterSpawner.spawnedObjects.Concat(skySpawner.spawnedObjects).Concat(groundSpawner.spawnedObjects));
        foreach (GameObject gameObject in currentEnemies)
        {
            if (gameObject != null)
            {
                Enemy enemy = gameObject.GetComponent<Enemy>();
                if (!enemy.isDead)
                {
                    enemy.Die();
                    Destroy(gameObject.transform.GetChild(0).gameObject);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }



    // Traps
    private IEnumerator Hook(Trap trap)
    {
        trap.Activate(this);
        trapForce = trap.force;
        yield return new WaitForSeconds(5f);
        trapForce = 1.0f;
        trap.Deactivate();
    }

    private IEnumerator Anker(Trap trap)
    {
        trap.Activate(this);
        trapForce = trap.force;
        direction = Vector3.zero;
        yield return new WaitForSeconds(1f);
        trapForce = 1.0f;
        trap.Deactivate();
    }

    private IEnumerator Captured(Trap net)
    {
        net.ActivateTrap(this);
        isCaptured = true;
        float time = net.escape;
        yield return new WaitForSeconds(time);
        isCaptured = false;
        net.Deactivate();
    }


    private IEnumerator Spin(Trap spin)
    {
        spin.Activate(this);
        isCaptured = true;
        animator.SetBool("isSpinning", true);
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isSpinning", false);
        isCaptured = false;
        spin.Deactivate();
    }

    private IEnumerator Wave(Trap wave)
    {
        wave.Activate(this);
        isCaptured = true;
        animator.SetBool("isWaving", true);
        yield return new WaitForSeconds(1.0f);
        animator.SetBool("isWaving", false);
        isCaptured = false;
        wave.Deactivate();
    }



}
