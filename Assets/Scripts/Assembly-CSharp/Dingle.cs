using System.Collections;
using UnityEngine;

public class Dingle : MonoBehaviour
{
    private float baseLive = 1000f;             // ������� ���������� ������
    private float liveBonus = 200f;             // ����� � ������ � ����������� �� ������
    private float totalLive;                    // ����� ���������� ������
    private float currentLive;                  // ������� ���������� ������
    private GameObject imageCurrentLive;        // ������ ����������� ������� ������ �� ����������
    private float offsetSubstract = 0.048f;     // �������� ��� ������� ����������� ������� ������
    private float offsetPercent = 0.02666f;     // ���������� �������� ��� ������� ����������� ������� ������
    private Rigidbody2D rb2D;                  // ��������� Rigidbody2D �������
    private Animator animator;                 // ��������� Animator �������
    private SpriteRenderer spriteRenderer;     // ��������� SpriteRenderer �������
    private int phase = 1;                     // ���� ����� (1 - ����������, 2 - �����)
    [SerializeField] private GameObject minipop;               // ������ ����-����
    [SerializeField] private float minTimeWaiting = 1f;       // ����������� ����� �������� ����� �������
    [SerializeField] private float maxTimeWaiting = 3f;       // ������������ ����� �������� ����� �������
    [SerializeField] private float vectorDistanceIncreaser = 3f; // ���������� ������� ����������� �����

    private int countAttacks;                   // ������� ����������� ����

    private void Start()
    {
        // ����������� ������ ���������� ������ � ����������� �� ������
        int level = FindObjectOfType<GameController>().GetLevel();
        totalLive = baseLive + liveBonus * level;

        // ��������� ����������� �������
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ����� ������ ����� ������������ ����� � ������ �������� �����
        Invoke("FindImageBar", 1f);
        StartCoroutine("Attack");
    }

    // ���������� ������� ����������� ������� ������
    public void FindImageBar()
    {
        imageCurrentLive = transform.parent.Find("CurrentLive").gameObject;
        transform.parent = null;
    }

    private void Update()
    {
        // �������� �� ���������� �������� ������ ���������� ������
        if (currentLive == totalLive / 2f)
        {
            phase = 2; // ������� � ���� 2
        }

        // �������� ����������� ����
        if (countAttacks == 3)
        {
            countAttacks = 0;
            animator.Play("FinishedAttack");
            StartCoroutine("Attack");
        }
    }

    // ��������� ����� ������� ��� ��������� �����
    private IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;    // ��������� ����� �� �������
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;  // ������� ����� �� �����
    }

    // �������� ��� ���������� �����
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(Random.Range(minTimeWaiting, maxTimeWaiting)); // �������� ���������� �������

        if (phase == 1)
        {
            animator.Play("PreparingAttack");   // ������ �������� ���������� �����
        }
    }

    // ����� ���������� ����� � ���� 1
    private void AttackPhase1()
    {
        // ���������� ����������� ������� � ������
        Vector2 direction = FindObjectOfType<Player>().transform.position - transform.position;
        rb2D.AddForce(direction * vectorDistanceIncreaser, ForceMode2D.Impulse); // ���������� �������� � ��������� �����������

        // ����������� ����������� ������� ������� � ����������� �� ���� ����� �������� � ���� X
        float angle = AngleBetween(Vector2.zero, direction);
        if ((angle < 0f && angle > -45f) || (angle > 0f && angle < 45f))
        {
            spriteRenderer.flipX = false; // ����������� ������
        }
        else if (angle < -135f || angle > 135f)
        {
            spriteRenderer.flipX = true; // ����������� �����
        }
    }

    // ���������� �������� ����������� ����
    public void IncreaseCountAttack()
    {
        countAttacks++;
    }

    // ����������� �������
    public void Destroy()
    {
        transform.parent.GetComponent<Room>().SendMessage("EnemyDeath"); // �������� ��������� � ������ �����
        Destroy(gameObject); // ����������� �������
    }

    // ���������� ������� ������ �����
    public void DecreaseLiveBar(float damage)
    {
        float x = (currentLive - damage) / totalLive; // ���������� ������ �������� �� ��� X
        float offset = damage / totalLive * offsetSubstract / offsetPercent; // ���������� �������� ������� ����������� ������� ������
        currentLive -= damage; // ���������� ������� ������
        imageCurrentLive.transform.localScale = new Vector3(x, imageCurrentLive.transform.localScale.y); // ��������� ������ ��������
        imageCurrentLive.transform.position = new Vector3(imageCurrentLive.transform.position.x - offset, imageCurrentLive.transform.position.y, imageCurrentLive.transform.position.z); // ��������� ����� �������
    }

    // ��������� ������������ � ������� ���������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tear") // �������� ������������ � ��������
        {
            StartCoroutine("ChangeColorDamaged"); // ��������� ����� ��� ��������� �����
            float damage = collision.gameObject.GetComponent<Tear>().GetDamage(); // ��������� ����� �� �������
            DecreaseLiveBar(damage); // ���������� ������� ������ �� �������� �����

            if (currentLive <= 0f) // �������� �� ������ �����
            {
                imageCurrentLive.SetActive(false); // ����������� ����������� ������� ������
                animator.Play("Death"); // ��������������� �������� ������
            }
        }
        else if (collision.gameObject.tag == "ItemPasiveDamage") // �������� ������������ � ���������, ��������� ��������� ����
        {
            StartCoroutine("ChangeColorDamaged"); // ��������� ����� ��� ��������� �����
            float damage = FindObjectOfType<GameController>().GetLevel() * GameController.damagePassiveItems; // ��������� ����� �� ���������� ��������
            DecreaseLiveBar(damage); // ���������� ������� ������ �� �������� �����

            if (currentLive <= 0f) // �������� �� ������ �����
            {
                imageCurrentLive.SetActive(false); // ����������� ����������� ������� ������
                animator.Play("Death"); // ��������������� �������� ������
            }
        }
    }
    private float AngleBetween(Vector2 v1, Vector2 v2)
    {
        Vector2 to = v2 - v1; // ���������� ������� �� v1 � v2
        float direction = (v2.y < v1.y) ? -1f : 1f; // ����������� ����������� �� ��� Y
        return Vector2.Angle(Vector2.right, to) * direction; // ���������� ���� ����� �������� to � ���� X
    }
}