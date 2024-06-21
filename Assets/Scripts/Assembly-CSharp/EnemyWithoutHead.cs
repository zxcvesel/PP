using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class EnemyWithoutHead : MonoBehaviour
{
    // Token: 0x06000033 RID: 51 RVA: 0x00002B98 File Offset: 0x00000D98
    private void Start()
    {
        this.rb2D = base.GetComponent<Rigidbody2D>();
        this.animator = base.GetComponent<Animator>();
        this.spriteRenderer = base.GetComponent<SpriteRenderer>();
        this.player = UnityEngine.Object.FindObjectOfType<Player>().gameObject;
        base.StartCoroutine("Move");
    }

    // Token: 0x06000034 RID: 52 RVA: 0x00002BE5 File Offset: 0x00000DE5
    private IEnumerator Move()
    {
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(this.minTimeWaiting, this.maxTimeWaiting));
        Vector2 vector = this.player.transform.position - base.transform.position;
        this.rb2D.AddForce(vector * this.vectorDistanceIncreaser, UnityEngine.ForceMode2D.Impulse);
        float num = this.AngleBetween(UnityEngine.Vector2.zero, vector);
        if ((num < 0f && num > -45f) || (num > 0f && num < 45f))
        {
            this.animator.Play("RunLeftRight");
            this.spriteRenderer.flipX = false;
        }
        else if (num < -135f || num > 135f)
        {
            this.animator.Play("RunLeftRight");
            this.spriteRenderer.flipX = true;
        }
        else
        {
            this.animator.Play("RunUpDown");
        }
        base.StartCoroutine("Move");
        yield break;
    }

    // Token: 0x06000035 RID: 53 RVA: 0x00002BF4 File Offset: 0x00000DF4
    private void Update()
    {
        if (this.rb2D.velocity != UnityEngine.Vector2.zero)
        {
            this.animator.SetBool("Moving", false);
            return;
        }
        this.animator.SetBool("Moving", true);
    }

    // Token: 0x06000036 RID: 54 RVA: 0x00002C30 File Offset: 0x00000E30
    private IEnumerator ChangeColorDamaged()
    {
        this.spriteRenderer.color = UnityEngine.Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        this.spriteRenderer.color = UnityEngine.Color.white;
        yield break;
    }

    // Token: 0x06000037 RID: 55 RVA: 0x00002C40 File Offset: 0x00000E40
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tear")
        {
            base.StartCoroutine("ChangeColorDamaged");
            this.live -= collision.gameObject.GetComponent<Tear>().GetDamage();
            if (this.live <= 0)
            {
                base.transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
        if (collision.gameObject.tag == "ItemPasiveDamage")
        {
            base.StartCoroutine("ChangeColorDamaged");
            this.live -= UnityEngine.Object.FindObjectOfType<GameController>().GetLevel() * 15;
            if (this.live <= 0)
            {
                base.transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
    }

    // Token: 0x06000038 RID: 56 RVA: 0x00002D24 File Offset: 0x00000F24
    private float AngleBetween(Vector2 v1, Vector2 v2)
    {
        Vector2 to = v2 - v1;
        float num = (v2.y < v1.y) ? -1f : 1f;
        return UnityEngine.Vector2.Angle(UnityEngine.Vector2.right, to) * num;
    }

    // Token: 0x0400002E RID: 46
    private Rigidbody2D rb2D;

    // Token: 0x0400002F RID: 47
    [SerializeField]
    private GameObject player;

    // Token: 0x04000030 RID: 48
    [SerializeField]
    private float minTimeWaiting = 1f;

    // Token: 0x04000031 RID: 49
    [SerializeField]
    private float maxTimeWaiting = 3f;

    // Token: 0x04000032 RID: 50
    [SerializeField]
    private int live = 100;

    // Token: 0x04000033 RID: 51
    [SerializeField]
    private float vectorDistanceIncreaser = 3f;

    // Token: 0x04000034 RID: 52
    private Animator animator;

    // Token: 0x04000035 RID: 53
    private SpriteRenderer spriteRenderer;
}
