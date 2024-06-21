using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Player : MonoBehaviour
{

    private void Awake()
    {
        if (Player._instance == null)
        {
            Player._instance = this;
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            return;
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public class LevelController : MonoBehaviour
    {
        // Статические переменные для смещения между дверями
        public static float offsetBetweenDoorsX = 2.0f;
        public static float offsetBetweenDoorsY = 2.0f; // Примерные значения, подставьте нужные вам

        // Другие части класса LevelController
    }

    private void Start()
    {
        this.game = UnityEngine.Object.FindObjectOfType<GameController>();
        this.audioSource = base.GetComponent<AudioSource>();
        this.animatorBody = this.body.GetComponent<Animator>();
        this.animatorHead = this.head.GetComponent<Animator>();
        this.spriteBody = this.body.GetComponent<SpriteRenderer>();
        this.spriteHead = this.head.GetComponent<SpriteRenderer>();
        this.direction = Player.Direction.ToDown;
        this.currentLives = (this.maxLives = 3f);
    }

    private void Update()
    {
        float axis = UnityEngine.Input.GetAxis("Horizontal");
        float axis2 = UnityEngine.Input.GetAxis("Vertical");
        if (this.isAlive && !this.blockedMovement)
        {
            base.transform.Translate(axis * UnityEngine.Time.deltaTime * this.speed, axis2 * UnityEngine.Time.deltaTime * this.speed, 0f);
            if (!this.isDamaged)
            {
                this.Animate();
            }
            base.StartCoroutine("InputShot");
        }
    }

    private IEnumerator InputShot()
    {
        if (UnityEngine.Time.time - this.lastShotTime > this.timeBeweenShots)
        {
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
            {
                int i = 0;
                while ((float)i < this.shotsPerInput)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.tearShot, base.transform.position, UnityEngine.Quaternion.identity);
                    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, this.shotSpeed));
                    gameObject.GetComponent<Tear>().Invoke("StartAnim", this.timeShot);
                    this.lastShotTime = UnityEngine.Time.time;
                    yield return new WaitForSeconds(0.1f);
                    int num = i;
                    i = num + 1;
                }
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.LeftArrow))
            {
                int i = 0;
                while ((float)i < this.shotsPerInput)
                {
                    GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.tearShot, base.transform.position, UnityEngine.Quaternion.identity);
                    gameObject2.GetComponent<Rigidbody2D>().AddForce(new Vector2(-this.shotSpeed, 0f));
                    gameObject2.GetComponent<Tear>().Invoke("StartAnim", this.timeShot);
                    this.lastShotTime = UnityEngine.Time.time;
                    this.spriteHead.flipX = true;
                    yield return new WaitForSeconds(0.1f);
                    int num = i;
                    i = num + 1;
                }
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
            {
                int i = 0;
                while ((float)i < this.shotsPerInput)
                {
                    GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.tearShot, base.transform.position, UnityEngine.Quaternion.identity);
                    gameObject3.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, -this.shotSpeed));
                    gameObject3.GetComponent<Tear>().Invoke("StartAnim", this.timeShot);
                    this.lastShotTime = UnityEngine.Time.time;
                    yield return new WaitForSeconds(0.1f);
                    int num = i;
                    i = num + 1;
                }
            }
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.RightArrow))
            {
                int i = 0;
                while ((float)i < this.shotsPerInput)
                {
                    GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.tearShot, base.transform.position, UnityEngine.Quaternion.identity);
                    gameObject4.GetComponent<Rigidbody2D>().AddForce(new Vector2(this.shotSpeed, 0f));
                    gameObject4.GetComponent<Tear>().Invoke("StartAnim", this.timeShot);
                    this.lastShotTime = UnityEngine.Time.time;
                    this.spriteHead.flipX = false;
                    yield return new WaitForSeconds(0.1f);
                    int num = i;
                    i = num + 1;
                }
            }
        }
        yield break;
    }

    private void Animate()
    {
        float axisRaw = UnityEngine.Input.GetAxisRaw("Horizontal");
        float axisRaw2 = UnityEngine.Input.GetAxisRaw("Vertical");
        if (axisRaw2 > 0f)
        {
            this.direction = Player.Direction.ToUp;
        }
        else if (axisRaw2 < 0f)
        {
            this.direction = Player.Direction.ToDown;
        }
        else if (axisRaw > 0f)
        {
            this.direction = Player.Direction.ToRight;
        }
        else if (axisRaw < 0f)
        {
            this.direction = Player.Direction.ToLeft;
        }
        else
        {
            this.direction = Player.Direction.Idle;
        }
        this.AnimateBody();
        this.AnimateHead();
    }

    private void AnimateBody()
    {
        if (this.direction == Player.Direction.ToUp || this.direction == Player.Direction.ToDown)
        {
            this.animatorBody.Play("WalkUpDown");
            return;
        }
        if (this.direction == Player.Direction.ToLeft)
        {
            this.animatorBody.Play("WalkLeftRight");
            this.spriteBody.flipX = true;
            return;
        }
        if (this.direction == Player.Direction.ToRight)
        {
            this.animatorBody.Play("WalkLeftRight");
            this.spriteBody.flipX = false;
            return;
        }
        this.animatorBody.Play("Idle");
    }

    private void AnimateHead()
    {
        if (this.direction == Player.Direction.ToUp)
        {
            this.animatorHead.Play("LookToUp");
            return;
        }
        if (this.direction == Player.Direction.ToLeft)
        {
            this.animatorHead.Play("LookToLeftRight");
            this.spriteHead.flipX = true;
            return;
        }
        if (this.direction == Player.Direction.ToRight)
        {
            this.animatorHead.Play("LookToLeftRight");
            this.spriteHead.flipX = false;
            return;
        }
        if (this.direction == Player.Direction.ToDown || this.direction == Player.Direction.ToDown)
        {
            this.animatorHead.Play("LookToDown");
        }
    }

    public void SetActiveCollider(bool isActive)
    {
        base.GetComponent<Collider2D>().enabled = isActive;
    }

    private IEnumerator ChangeColorDamaged()
    {
        this.spriteBody.color = UnityEngine.Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        this.spriteBody.color = UnityEngine.Color.white;
        this.animatorHead.gameObject.SetActive(true);
        this.isDamaged = false;
        base.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        this.animatorBody.SetBool("IsDamaged", this.isDamaged);
        yield break;
    }

    private IEnumerator ItemTaken(string message)
    {
        this.game.StartCoroutine("SetMessageItem", message);
        this.animatorHead.gameObject.SetActive(false);
        this.animatorHead.enabled = false;
        this.animatorBody.Play("ItemTaken");
        this.blockedMovement = true;
        yield return new WaitForSecondsRealtime(1.25f);
        this.animatorHead.gameObject.SetActive(true);
        this.animatorHead.enabled = true;
        this.blockedMovement = false;
        yield break;
    }

    public GameObject GetTears()
    {
        return this.tearShot;
    }

    public void IsDamaged()
    {
        this.currentLives -= this.halfHearth;
        if (this.currentLives != 0f)
        {
            this.audioSource.clip = this.damagedClips[UnityEngine.Random.Range(0, this.damagedClips.Length)];
            this.audioSource.Play();
            this.animatorHead.gameObject.SetActive(false);
            base.StartCoroutine("ChangeColorDamaged");
            this.animatorBody.SetBool("IsDamaged", this.isDamaged);
            return;
        }
        this.audioSource.clip = this.deathClip;
        this.audioSource.Play();
        this.animatorHead.gameObject.SetActive(false);
        this.isAlive = false;
        this.animatorBody.SetBool("IsAlive", this.isAlive);
        this.animatorBody.Play("Death");
        this.game.SendMessage("FinishRun");
    }

    public bool GetHasTreasureRoomKey()
    {
        return this.hasTreasureRoomKey;
    }

    public void SetHasTreasureRoomKey(bool hasTreasureRoomKey)
    {
        this.hasTreasureRoomKey = hasTreasureRoomKey;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Door")
        {
            Door component = collision.GetComponent<Door>();
            if (component.GetIsOpened())
            {
                string a = collision.name;
                if (a == "DoorPointLeft")
                {
                    base.transform.position = base.transform.position + new Vector3(LevelController.offsetBetweenDoorsX, 0f, 0f);
                    return;
                }
                if (a == "DoorPointRight")
                {
                    base.transform.position = base.transform.position + new Vector3(-LevelController.offsetBetweenDoorsX, 0f, 0f);
                    return;
                }
                if (a == "DoorPointUp")
                {
                    base.transform.position = base.transform.position + new Vector3(0f, LevelController.offsetBetweenDoorsY, 0f);
                    return;
                }
                if (!(a == "DoorPointDown"))
                {
                    return;
                }
                base.transform.position = base.transform.position + new Vector3(0f, -LevelController.offsetBetweenDoorsY, 0f);
                return;
            }
            else if (component.GetNeedKey() && this.hasTreasureRoomKey)
            {
                component.SendMessage("OpenWithKey");
                return;
            }
        }
        else
        {
            if (collision.tag == "RoomFloor")
            {
                Room componentInParent = collision.gameObject.GetComponentInParent<Room>();
                GameObject pointZero = componentInParent.GetPointZero();
                UnityEngine.Object.FindObjectOfType<CameraController>().Move(pointZero.transform);
                componentInParent.EnterFocus();
                return;
            }
            if (collision.tag.Contains("Item"))
            {
                this.animatorHead.enabled = false;
                string a = collision.tag;
                if (a == "SpeedBallItem")
                {
                    base.StartCoroutine("ItemTaken", "Speed Ball: Speed shot up!");
                    this.timeBeweenShots -= this.timeBeweenShots / 3f;
                    UnityEngine.Object.Destroy(collision.gameObject);
                    return;
                }
                if (a == "PolyphemusItem")
                {
                    base.StartCoroutine("ItemTaken", "Polyphemus: Look that tears!");
                    Vector2 scale = this.tearShot.GetComponent<Tear>().GetScale();
                    this.tearShot.GetComponent<Tear>().AddScale(new Vector3(scale.x * 1.15f, scale.y * 1.15f, 0f));
                    UnityEngine.Object.Destroy(collision.gameObject);
                    return;
                }
                if (a == "DoubleShotItem")
                {
                    base.StartCoroutine("ItemTaken", "Double Shot: I am seeing double?");
                    this.shotsPerInput *= 2f;
                    UnityEngine.Object.Destroy(collision.gameObject);
                    return;
                }
                if (a == "CubeOfMeatItem")
                {
                    base.StartCoroutine("ItemTaken", "Cube of meat: It's time to eat");
                    collision.GetComponent<CubeOfMeat>().SetPlayer(base.gameObject);
                    collision.tag = "ItemPasiveDamage";
                    collision.transform.position = (collision.transform.position - base.transform.position).normalized / 1.5f + collision.transform.position;
                    collision.transform.parent = base.transform;
                    return;
                }
                if (!(a == "InnerEyeItem"))
                {
                    return;
                }
                base.StartCoroutine("ItemTaken", "Inner Eye: shot, shot, shot");
                this.shotsPerInput += 2f;
                int damage = this.tearShot.GetComponent<Tear>().GetDamage();
                this.tearShot.GetComponent<Tear>().SetDamage(damage - 7);
                UnityEngine.Object.Destroy(collision.gameObject);
                return;
            }
            else
            {
                if (collision.tag == "TreasureRoomKey")
                {
                    this.hasTreasureRoomKey = true;
                    UnityEngine.Object.Destroy(collision.gameObject);
                    return;
                }
                if (collision.tag == "NextLevelDoor")
                {
                    this.game.SendMessage("NextLevel");
                }
            }
        }
    }

   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyTear") && !this.isDamaged)
        {
            UnityEngine.MonoBehaviour.print("holaa22a");
            this.isDamaged = true;
            this.game.SendMessage("Damaged");
            this.IsDamaged();
            return;
        }
        if (collision.gameObject.tag == "FullHearth" && this.currentLives <= this.maxLives - this.halfHearth * 2f)
        {
            this.currentLives += this.halfHearth * 2f;
            this.game.Healthed(2);
            UnityEngine.Object.Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.tag == "HalfHearth" && this.currentLives <= this.maxLives - this.halfHearth)
        {
            this.currentLives += this.halfHearth;
            this.game.Healthed(1);
            UnityEngine.Object.Destroy(collision.gameObject);
        }
    }

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float shotSpeed = 180f;

    [SerializeField]
    private float timeShot = 1f;

    [SerializeField]
    private float shotsPerInput = 1f;

    [SerializeField]
    private float lastShotTime;

    [SerializeField]
    private float timeBeweenShots = 0.5f;

    [SerializeField]
    private float maxLives;

    [SerializeField]
    private float currentLives;

    [SerializeField]
    private float halfHearth = 0.5f;

    [SerializeField]
    private GameObject tearShot;

    [SerializeField]
    private GameObject body;

    [SerializeField]
    private GameObject head;

    [SerializeField]
    private AudioClip[] damagedClips;

    [SerializeField]
    private AudioClip deathClip;

    private GameController game;

    private AudioSource audioSource;

    private Animator animatorBody;

    private Animator animatorHead;

    private SpriteRenderer spriteBody;

    private SpriteRenderer spriteHead;

    private bool isDamaged;

    private bool isAlive = true;

    private bool hasTreasureRoomKey;

    private bool blockedMovement;

    private Player.Direction direction;

    private static Player _instance;

    private enum Direction
    {

        ToLeft,
  
        ToRight,
    
        ToUp,
   
        ToDown,

        Idle
    }
}
