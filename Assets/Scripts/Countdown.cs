using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;



    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //audioSource.Play();
        StartCoroutine(CountdownCoroutine());
    }

    private IEnumerator CountdownCoroutine()
    {
        for (int i = sprites.Length; i > 0; i--)
        {
            audioSource.Play();
            spriteRenderer.sprite = sprites[i - 1];
            yield return new WaitForSeconds(1);
        }
        gameManager.StartGame();
    }
}
