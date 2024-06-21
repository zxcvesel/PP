using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class MiniHaunt : MonoBehaviour
{
    // Token: 0x06000080 RID: 128 RVA: 0x00003E84 File Offset: 0x00002084
    private void Start()
    {
        this.animator = base.GetComponent<Animator>();
        this.spriteRenderer = base.GetComponent<SpriteRenderer>();
    }

    // Token: 0x06000081 RID: 129 RVA: 0x00003EA0 File Offset: 0x000020A0
    private void Update()
    {
        if (this.isActive)
        {
            Transform transform = UnityEngine.Object.FindObjectOfType<Player>().transform;
            base.transform.position = Vector2.MoveTowards(base.transform.position, transform.position, this.speed * Time.deltaTime);
        }
    }

    // Token: 0x06000082 RID: 130 RVA: 0x00003EFC File Offset: 0x000020FC
    public void Active()
    {
        this.animator.Play("Attacking");
        this.spriteRenderer.color = Color.white;
        this.isActive = true;
        base.transform.parent = null;
    }

    // Token: 0x06000083 RID: 131 RVA: 0x00003F31 File Offset: 0x00002131
    private IEnumerator ChangeColorDamaged()
    {
        this.spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        this.spriteRenderer.color = Color.white;
        yield break;
    }

    // Token: 0x06000084 RID: 132 RVA: 0x00003F40 File Offset: 0x00002140
    private void Damaged(int damage)
    {
        base.StartCoroutine("ChangeColorDamaged");
        this.live -= damage;
        if (this.live <= 0)
        {
            UnityEngine.Object.FindObjectOfType<TheHaunt>().DiedMiniHaunt();
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    // Token: 0x06000085 RID: 133 RVA: 0x00003F7C File Offset: 0x0000217C
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.isActive && collision.tag == "Tear")
        {
            int damage = collision.GetComponent<Tear>().GetDamage();
            this.Damaged(damage);
        }
        if (this.isActive && collision.gameObject.tag == "ItemPasiveDamage")
        {
            int damage2 = (int)(UnityEngine.Object.FindObjectOfType<GameController>().GetLevel() * GameController.damagePassiveItems);
            this.Damaged(damage2);
        }
    }

    // Token: 0x04000076 RID: 118
    private Animator animator;

    // Token: 0x04000077 RID: 119
    private SpriteRenderer spriteRenderer;

    // Token: 0x04000078 RID: 120
    private float speed = 2f;

    // Token: 0x04000079 RID: 121
    private bool isActive;

    // Token: 0x0400007A RID: 122
    [SerializeField]
    private int live = 200;
}
