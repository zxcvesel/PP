using System.Collections;
using UnityEngine;

public class EnemyWorm : MonoBehaviour
{
    [SerializeField] private float minTimeWaitingShot = 1f;
    [SerializeField] private float maxTimeWaitingShot = 3f;
    [SerializeField] private float minTimeWaiting = 1f;
    [SerializeField] private float maxTimeWaiting = 3f;
    [SerializeField] private float live = 80f; // Изменили тип данных на float
    [SerializeField] private GameObject tear;
    [SerializeField] private float shotSpeed = 50f;
    [SerializeField] private float timeShot = 1f;

    private Bounds bounds;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("Hidden");
        bounds = transform.parent.GetChild(0).GetComponent<Collider2D>().bounds;
        Invoke("Appear", Random.Range(minTimeWaiting, maxTimeWaiting));
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
        Invoke("PrepareShot", Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    public void InstantiateShot()
    {
        Vector2 direction = FindObjectOfType<Player>().transform.position - transform.position;
        GameObject newTear = Instantiate(tear, transform.position, Quaternion.identity);
        newTear.GetComponent<Rigidbody2D>().AddForce(direction.normalized * shotSpeed);
        newTear.GetComponent<EnemyTear>().Invoke("StartAnim", timeShot);
    }

    private void Appear()
    {
        animator.Play("Appears");
        GetComponent<Collider2D>().enabled = true;
        Invoke("Shot", Random.Range(minTimeWaitingShot, maxTimeWaitingShot));
    }

    private void Shot()
    {
        Vector3 direction = FindObjectOfType<Player>().transform.position - transform.position;
        GameObject newTear = Instantiate(tear, transform.position + direction.normalized / 2f, Quaternion.identity);
        newTear.GetComponent<Rigidbody2D>().AddForce(direction.normalized * shotSpeed);
        newTear.GetComponent<EnemyTear>().Invoke("StartAnim", timeShot);
        Invoke("Disappear", Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    private void Disappear()
    {
        animator.Play("Disappear");
        GetComponent<Collider2D>().enabled = false;
        Invoke("Appear", Random.Range(minTimeWaiting, maxTimeWaiting));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tear")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= collision.gameObject.GetComponent<Tear>().GetDamage();
        }
        if (collision.gameObject.tag == "ItemPassiveDamage")
        {
            StartCoroutine("ChangeColorDamaged");
            live -= GameController.damagePassiveItems; // Используем damagePassiveItems из GameController
        }
        if (live <= 0)
        {
            transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
            Destroy(gameObject);
        }
    }

    public Vector2 RandomPointInBounds()
    {
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }
}