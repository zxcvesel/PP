using System.Collections;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    [SerializeField] private float minTimeWaiting = 1f;
    [SerializeField] private float maxTimeWaiting = 3f;
    [SerializeField] private int live = 60;
    [SerializeField] private GameObject tear;
    [SerializeField] private float shotSpeed = 50f;
    [SerializeField] private float timeShot = 1f;
    [SerializeField] private float speed = 1f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        InvokeRepeating("PrepareShot", Random.Range(minTimeWaiting, maxTimeWaiting), Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    private void Update()
    {
        Transform playerTransform = FindObjectOfType<Player>().transform;
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }

    private IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;
    }

    private void PrepareShot()
    {
        animator.Play("Shot");
    }

    public void InstantiateShot()
    {
        Vector2 direction = FindObjectOfType<Player>().transform.position - transform.position;
        GameObject newTear = Instantiate(tear, transform.position, Quaternion.identity);
        newTear.GetComponent<Rigidbody2D>().AddForce(direction.normalized * shotSpeed);
        newTear.GetComponent<EnemyTear>().Invoke("StartAnim", timeShot);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tear"))
        {
            StartCoroutine(ChangeColorDamaged());
            live -= collision.GetComponent<Tear>().GetDamage();
        }
        else if (collision.CompareTag("ItemPassiveDamage"))
        {
            StartCoroutine(ChangeColorDamaged());
            live -= Mathf.RoundToInt(UnityEngine.Object.FindObjectOfType<GameController>().GetLevel() * GameController.damagePassiveItems);
        }

        if (live <= 0)
        {
            transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
            Destroy(gameObject);
        }
    }
}