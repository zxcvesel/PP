using System.Collections;
using UnityEngine;

public class Dingle : MonoBehaviour
{
    private float baseLive = 1000f;             // Базовое количество жизней
    private float liveBonus = 200f;             // Бонус к жизням в зависимости от уровня
    private float totalLive;                    // Общее количество жизней
    private float currentLive;                  // Текущее количество жизней
    private GameObject imageCurrentLive;        // Объект изображения текущих жизней на интерфейсе
    private float offsetSubstract = 0.048f;     // Смещение для позиции изображения текущих жизней
    private float offsetPercent = 0.02666f;     // Процентное смещение для позиции изображения текущих жизней
    private Rigidbody2D rb2D;                  // Компонент Rigidbody2D объекта
    private Animator animator;                 // Компонент Animator объекта
    private SpriteRenderer spriteRenderer;     // Компонент SpriteRenderer объекта
    private int phase = 1;                     // Фаза атаки (1 - подготовка, 2 - атака)
    [SerializeField] private GameObject minipop;               // Префаб мини-попа
    [SerializeField] private float minTimeWaiting = 1f;       // Минимальное время ожидания между атаками
    [SerializeField] private float maxTimeWaiting = 3f;       // Максимальное время ожидания между атаками
    [SerializeField] private float vectorDistanceIncreaser = 3f; // Увеличение вектора направления атаки

    private int countAttacks;                   // Счетчик выполненных атак

    private void Start()
    {
        // Определение общего количества жизней в зависимости от уровня
        int level = FindObjectOfType<GameController>().GetLevel();
        totalLive = baseLive + liveBonus * level;

        // Настройка компонентов объекта
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Вызов метода через определенное время и запуск корутины атаки
        Invoke("FindImageBar", 1f);
        StartCoroutine("Attack");
    }

    // Нахождение объекта изображения текущих жизней
    public void FindImageBar()
    {
        imageCurrentLive = transform.parent.Find("CurrentLive").gameObject;
        transform.parent = null;
    }

    private void Update()
    {
        // Проверка на достижение половины общего количества жизней
        if (currentLive == totalLive / 2f)
        {
            phase = 2; // Переход к фазе 2
        }

        // Проверка выполненных атак
        if (countAttacks == 3)
        {
            countAttacks = 0;
            animator.Play("FinishedAttack");
            StartCoroutine("Attack");
        }
    }

    // Изменение цвета объекта при получении урона
    private IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;    // Изменение цвета на красный
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;  // Возврат цвета на белый
    }

    // Корутина для выполнения атаки
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(Random.Range(minTimeWaiting, maxTimeWaiting)); // Ожидание случайного времени

        if (phase == 1)
        {
            animator.Play("PreparingAttack");   // Запуск анимации подготовки атаки
        }
    }

    // Метод выполнения атаки в фазе 1
    private void AttackPhase1()
    {
        // Вычисление направления вектора к игроку
        Vector2 direction = FindObjectOfType<Player>().transform.position - transform.position;
        rb2D.AddForce(direction * vectorDistanceIncreaser, ForceMode2D.Impulse); // Добавление импульса в указанном направлении

        // Определение направления взгляда спрайта в зависимости от угла между вектором и осью X
        float angle = AngleBetween(Vector2.zero, direction);
        if ((angle < 0f && angle > -45f) || (angle > 0f && angle < 45f))
        {
            spriteRenderer.flipX = false; // Направление вправо
        }
        else if (angle < -135f || angle > 135f)
        {
            spriteRenderer.flipX = true; // Направление влево
        }
    }

    // Увеличение счетчика выполненных атак
    public void IncreaseCountAttack()
    {
        countAttacks++;
    }

    // Уничтожение объекта
    public void Destroy()
    {
        transform.parent.GetComponent<Room>().SendMessage("EnemyDeath"); // Отправка сообщения о смерти врага
        Destroy(gameObject); // Уничтожение объекта
    }

    // Уменьшение текущих жизней врага
    public void DecreaseLiveBar(float damage)
    {
        float x = (currentLive - damage) / totalLive; // Вычисление нового масштаба по оси X
        float offset = damage / totalLive * offsetSubstract / offsetPercent; // Вычисление смещения позиции изображения текущих жизней
        currentLive -= damage; // Уменьшение текущих жизней
        imageCurrentLive.transform.localScale = new Vector3(x, imageCurrentLive.transform.localScale.y); // Установка нового масштаба
        imageCurrentLive.transform.position = new Vector3(imageCurrentLive.transform.position.x - offset, imageCurrentLive.transform.position.y, imageCurrentLive.transform.position.z); // Установка новой позиции
    }

    // Обработка столкновений с другими объектами
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Tear") // Проверка столкновения с снарядом
        {
            StartCoroutine("ChangeColorDamaged"); // Изменение цвета при получении урона
            float damage = collision.gameObject.GetComponent<Tear>().GetDamage(); // Получение урона от снаряда
            DecreaseLiveBar(damage); // Уменьшение текущих жизней на величину урона

            if (currentLive <= 0f) // Проверка на смерть врага
            {
                imageCurrentLive.SetActive(false); // Деактивация изображения текущих жизней
                animator.Play("Death"); // Воспроизведение анимации смерти
            }
        }
        else if (collision.gameObject.tag == "ItemPasiveDamage") // Проверка столкновения с предметом, наносящим пассивный урон
        {
            StartCoroutine("ChangeColorDamaged"); // Изменение цвета при получении урона
            float damage = FindObjectOfType<GameController>().GetLevel() * GameController.damagePassiveItems; // Получение урона от пассивного предмета
            DecreaseLiveBar(damage); // Уменьшение текущих жизней на величину урона

            if (currentLive <= 0f) // Проверка на смерть врага
            {
                imageCurrentLive.SetActive(false); // Деактивация изображения текущих жизней
                animator.Play("Death"); // Воспроизведение анимации смерти
            }
        }
    }
    private float AngleBetween(Vector2 v1, Vector2 v2)
    {
        Vector2 to = v2 - v1; // Вычисление вектора от v1 к v2
        float direction = (v2.y < v1.y) ? -1f : 1f; // Определение направления по оси Y
        return Vector2.Angle(Vector2.right, to) * direction; // Вычисление угла между вектором to и осью X
    }
}