using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class MiniPop : MonoBehaviour
{
    // Token: 0x06000087 RID: 135 RVA: 0x0000400B File Offset: 0x0000220B
    private void Start()
    {
        this.rb2D = base.GetComponent<Rigidbody2D>();
        this.animator = base.GetComponent<Animator>();
        this.spriteRenderer = base.GetComponent<SpriteRenderer>();
        base.StartCoroutine("Move");
    }

    // Token: 0x06000088 RID: 136 RVA: 0x0000403D File Offset: 0x0000223D
    private IEnumerator Move()
    {
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(this.minTimeWaiting, this.maxTimeWaiting));
        Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
        this.animator.Play("Attacking");
        this.rb2D.AddForce(normalized * this.vectorDistanceIncreaser, UnityEngine.ForceMode2D.Impulse);
        float num = this.AngleBetween(UnityEngine.Vector2.zero, normalized);
        if ((num < 0f && num > -90f) || (num > 0f && num < 90f))
        {
            this.spriteRenderer.flipX = false;
        }
        else
        {
            this.spriteRenderer.flipX = true;
        }
        base.StartCoroutine("Move");
        yield break;
    }

    // Token: 0x06000089 RID: 137 RVA: 0x0000404C File Offset: 0x0000224C
    private void Update()
    {
        if (this.live <= 0)
        {
            if (!this.isInvoked)
            {
                base.transform.parent.GetComponent<Room>().SendMessage("EnemyDeath");
            }
            UnityEngine.Object.Destroy(base.gameObject);
        }
        if (this.rb2D.velocity == UnityEngine.Vector2.zero)
        {
            this.animator.Play("Idle");
        }
    }

    // Token: 0x0600008A RID: 138 RVA: 0x000040B6 File Offset: 0x000022B6
    private IEnumerator ChangeColorDamaged()
    {
        this.spriteRenderer.color = UnityEngine.Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        this.spriteRenderer.color = UnityEngine.Color.white;
        yield break;
    }

    // Token: 0x0600008B RID: 139 RVA: 0x000040C8 File Offset: 0x000022C8
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Tear")
        {
            base.StartCoroutine("ChangeColorDamaged");
            this.live -= collision.gameObject.GetComponent<Tear>().GetDamage();
        }
        if (collision.gameObject.tag == "ItemPasiveDamage")
        {
            base.StartCoroutine("ChangeColorDamaged");
            this.live -= (int)(UnityEngine.Object.FindObjectOfType<GameController>().GetLevel() * GameController.damagePassiveItems);
        }
    }

    // Token: 0x0600008C RID: 140 RVA: 0x00004150 File Offset: 0x00002350
    private float AngleBetween(Vector2 v1, Vector2 v2)
    {
        Vector2 to = v2 - v1;
        float num = (v2.y < v1.y) ? -1f : 1f;
        return UnityEngine.Vector2.Angle(UnityEngine.Vector2.right, to) * num;
    }

    // Token: 0x0600008D RID: 141 RVA: 0x0000418D File Offset: 0x0000238D
    public void SetIsInvoked(bool isInvoked)
    {
        this.isInvoked = isInvoked;
    }

    // Token: 0x0400007B RID: 123
    [SerializeField]
    private float minTimeWaiting = 0.5f;

    // Token: 0x0400007C RID: 124
    [SerializeField]
    private float maxTimeWaiting = 1.5f;

    // Token: 0x0400007D RID: 125
    [SerializeField]
    private int live = 40;

    // Token: 0x0400007E RID: 126
    [SerializeField]
    private float vectorDistanceIncreaser = 1.5f;

    // Token: 0x0400007F RID: 127
    private Rigidbody2D rb2D;

    // Token: 0x04000080 RID: 128
    private Animator animator;

    // Token: 0x04000081 RID: 129
    private SpriteRenderer spriteRenderer;

    // Token: 0x04000082 RID: 130
    private bool isInvoked;
}
