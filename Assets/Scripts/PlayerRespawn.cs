using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    private Transform currentCheckpoint;
    private HealthManager healthManager;
    private bool playerInRange;
    private Transform checkpointToSet;

    [SerializeField] private Transform checkpointHintText;
    [Space(10)]

    [SerializeField] private Vector3 initialTextPosition;
    [SerializeField] private Vector3 targetTextPosition;
    
    [Space(10)]
    
    [SerializeField] private Vector3 initialTextScale;
    [SerializeField] private Vector3 targetTextScale;

    private Coroutine animateTextCoroutine;
    
    [Space(10)]
    public float personHeight = 5f;
    
    [Space(10)]
    [SerializeField] private ScreenFade screenFade;
    [SerializeField] private float fadeDuration = 1f;

    private Animator animator;

    [Space(10)]
    public PersonMovement personMovement;

    private void Awake()
    {
        healthManager = FindFirstObjectByType<HealthManager>();
        animator = GetComponentInChildren<Animator>();
        personMovement = GetComponent<PersonMovement>();

        if (personMovement == null)
        {
            Debug.Log("Я того рот шатал");
        }
    }

    private void Start()
    {
        if (checkpointHintText != null)
        {
            initialTextPosition = checkpointHintText.localPosition;
            targetTextPosition = initialTextPosition + new Vector3(0f, 1.5f, 0f);

            initialTextScale = checkpointHintText.localScale;
            targetTextScale = initialTextScale * 1.2f;

            CanvasGroup canvasGroup = checkpointHintText.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = checkpointHintText.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
            checkpointHintText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            SetCheckpoint(checkpointToSet);
            healthManager.RestoreHealth();
        }

        if (healthManager.GetHealth() <= 0 && !isFading)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private bool isFading = false;

    private IEnumerator HandleDeath()
    {
        personMovement.DisableMovement();
        animator.SetTrigger("IsDead");

        isFading = true;
        screenFade.FadeOut(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);

        Respawn();

        animator.SetFloat("Speed", 0f);
        animator.SetTrigger("IsRespawned");

        screenFade.FadeIn(fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        isFading = false;

        personMovement.EnableMovement();
    }

    public void Respawn()
    {
        if (currentCheckpoint != null)
        {
            Vector3 newPosition = currentCheckpoint.position;
            newPosition.y += personHeight;
            transform.position = newPosition;
            healthManager.RestoreHealth();
        }
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Checkpoint")
        {
            playerInRange = true;
            checkpointToSet = collision.transform;

            checkpointHintText.gameObject.SetActive(true);

            if (animateTextCoroutine != null)
            {
                StopCoroutine(animateTextCoroutine);
            }
            animateTextCoroutine = StartCoroutine(AnimateText(targetTextPosition, targetTextScale, 0.2f, true));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Checkpoint")
        {
            playerInRange = false;
            checkpointToSet = null;

            if (animateTextCoroutine != null)
            {
                StopCoroutine(animateTextCoroutine);
            }
            animateTextCoroutine = StartCoroutine(AnimateText(initialTextPosition, initialTextScale, 0.2f, false));
        }
    }

    private IEnumerator AnimateText(Vector3 targetPosition, Vector3 targetScale, float duration, bool show)
    {
        float elapsed = 0f;
        Vector3 startPosition = checkpointHintText.localPosition;
        Vector3 startScale = checkpointHintText.localScale;
        CanvasGroup canvasGroup = checkpointHintText.GetComponent<CanvasGroup>();

        float startAlpha = canvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;

        if (show)
        {
            checkpointHintText.gameObject.SetActive(true);
        }

        while (elapsed < duration)
        {
            checkpointHintText.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            checkpointHintText.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        checkpointHintText.localPosition = targetPosition;
        checkpointHintText.localScale = targetScale;
        canvasGroup.alpha = targetAlpha;

        if (!show)
        {
            checkpointHintText.gameObject.SetActive(false);
        }
    }
}