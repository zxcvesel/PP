using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class TheHaunt : MonoBehaviour
{
    // Token: 0x060000BA RID: 186 RVA: 0x000052D8 File Offset: 0x000034D8
    private void Start()
    {
        int level = UnityEngine.Object.FindObjectOfType<GameController>().GetLevel();
        this.currentLive = (this.totalLive = this.baseLive + this.liveBonus * (float)level);
        this.animator = base.GetComponent<Animator>();
        this.spriteRenderer = base.GetComponent<SpriteRenderer>();
        base.Invoke("FindImageBar", 1f);
        base.Invoke("ActiveMiniHaunts", (float)UnityEngine.Random.Range(2, 5));
    }

    // Token: 0x060000BB RID: 187 RVA: 0x0000534C File Offset: 0x0000354C
    private void Update()
    {
        Transform transform = UnityEngine.Object.FindObjectOfType<Player>().transform;
        if (this.isActive)
        {
            base.transform.position = UnityEngine.Vector2.MoveTowards(base.transform.position, this.nextPosition, this.speed * UnityEngine.Time.deltaTime);
            if (this.isActive && this.wayPoints[0].transform.position.y - transform.position.y < 1f)
            {
                this.speed = 6f;
                if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
                {
                    this.animator.Play("PrepareCharge");
                }
            }
            else
            {
                this.speed = 2f;
            }
            if (UnityEngine.Vector2.Distance(base.transform.position, this.nextPosition) < this.distanceChange)
            {
                this.currentPosition++;
                if (this.currentPosition >= this.wayPoints.Length)
                {
                    this.currentPosition = 0;
                }
                this.nextPosition = this.wayPoints[this.currentPosition].transform.position;
                return;
            }
        }
        else
        {
            if (UnityEngine.Vector2.Distance(base.transform.position, this.nextPosition) < this.distanceChange)
            {
                this.nextPosition = this.RandomPointInBounds();
            }
            base.transform.position = UnityEngine.Vector2.MoveTowards(base.transform.position, this.nextPosition, this.speed * UnityEngine.Time.deltaTime);
        }
    }

    // Token: 0x060000BC RID: 188 RVA: 0x000054EF File Offset: 0x000036EF
    private void ActiveMiniHaunts()
    {
        this.miniHaunts[this.miniHaunts.Count - 1].Active();
        this.miniHaunts.RemoveAt(this.miniHaunts.Count - 1);
    }

    // Token: 0x060000BD RID: 189 RVA: 0x00005528 File Offset: 0x00003728
    public void DiedMiniHaunt()
    {
        this.miniHauntsAlive--;
        if (this.miniHauntsAlive == 2)
        {
            for (int i = 0; i < 2; i++)
            {
                this.ActiveMiniHaunts();
            }
        }
        if (this.miniHauntsAlive == 0)
        {
            this.spriteRenderer.color = UnityEngine.Color.white;
            this.animator.Play("FinishImmortal");
            this.wayPoints = UnityEngine.GameObject.FindGameObjectsWithTag("WayPoint");
            this.nextPosition = this.wayPoints[0].transform.position;
            this.isActive = true;
            base.StartCoroutine("Shot");
        }
    }

    // Token: 0x060000BE RID: 190 RVA: 0x000055C6 File Offset: 0x000037C6
    private IEnumerator Shot()
    {
        yield return new WaitForSeconds((float)UnityEngine.Random.Range(3, 6));
        this.animator.Play("Shot");
        int num;
        for (int i = 0; i < 7; i = num + 1)
        {
            Vector2 a = UnityEngine.Object.FindObjectOfType<Player>().transform.position - base.transform.position;
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.tear, base.transform.position - new Vector3(0f, 0.3f, 0f), UnityEngine.Quaternion.identity);
            gameObject.GetComponent<Rigidbody2D>().AddForce(a * this.shotSpeed);
            gameObject.GetComponent<EnemyTear>().Invoke("StartAnim", this.timeShot);
            yield return new WaitForSeconds(0.05f);
            num = i;
        }
        base.StartCoroutine("Shot");
        yield break;
    }

    // Token: 0x060000BF RID: 191 RVA: 0x000055D8 File Offset: 0x000037D8
    public void FindImageBar()
    {
        this.imageCurrentLive = UnityEngine.GameObject.Find("CurrentLive").gameObject;
        UnityEngine.Object gameObject = base.transform.parent.gameObject;
        base.transform.parent.DetachChildren();
        UnityEngine.Object.Destroy(gameObject);
        this.nextPosition = this.RandomPointInBounds();
    }

    // Token: 0x060000C0 RID: 192 RVA: 0x0000562B File Offset: 0x0000382B
    private IEnumerator ChangeColorDamaged()
    {
        this.spriteRenderer.color = UnityEngine.Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        this.spriteRenderer.color = UnityEngine.Color.white;
        yield break;
    }

    // Token: 0x060000C1 RID: 193 RVA: 0x0000563A File Offset: 0x0000383A
    public void Destroy()
    {
        UnityEngine.GameObject.Find("BossRoom(Clone)").gameObject.GetComponent<Room>().SendMessage("EnemyDeath");
        UnityEngine.Object.Destroy(base.gameObject);
    }

    // Token: 0x060000C2 RID: 194 RVA: 0x00005668 File Offset: 0x00003868
    public void DecreaseLiveBar(float damage)
    {
        float x = (this.currentLive - damage) / this.totalLive;
        float num = damage / this.totalLive * this.offsetSubstract / this.offsetPercent;
        this.currentLive -= damage;
        this.imageCurrentLive.transform.localScale = new Vector3(x, this.imageCurrentLive.transform.localScale.y);
        this.imageCurrentLive.transform.position = new Vector3(this.imageCurrentLive.transform.position.x - num, this.imageCurrentLive.transform.position.y, this.imageCurrentLive.transform.position.z);
    }

    // Token: 0x060000C3 RID: 195 RVA: 0x0000572C File Offset: 0x0000392C
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.isActive && collision.gameObject.tag == "Tear")
        {
            base.StartCoroutine("ChangeColorDamaged");
            float damage = (float)collision.gameObject.GetComponent<Tear>().GetDamage();
            this.DecreaseLiveBar(damage);
            if (this.currentLive <= 0f)
            {
                this.imageCurrentLive.SetActive(false);
                this.animator.Play("Death");
                base.Invoke("Destroy", 1f);
            }
        }
        if (this.isActive && collision.gameObject.tag == "ItemPasiveDamage")
        {
            base.StartCoroutine("ChangeColorDamaged");
            float damage2 = (float)(UnityEngine.Object.FindObjectOfType<GameController>().GetLevel() * GameController.damagePassiveItems);
            this.DecreaseLiveBar(damage2);
            if (this.currentLive <= 0f)
            {
                this.imageCurrentLive.SetActive(false);
                this.animator.Play("Death");
            }
        }
    }

    // Token: 0x060000C4 RID: 196 RVA: 0x00005824 File Offset: 0x00003A24
    private Vector2 RandomPointInBounds()
    {
        Bounds bounds = UnityEngine.GameObject.Find("BossRoom(Clone)").gameObject.transform.GetChild(0).GetComponent<Collider2D>().bounds;
        return new Vector2(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), UnityEngine.Random.Range(bounds.min.y, bounds.max.y));
    }

    // Token: 0x040000AC RID: 172
    private float baseLive = 600f;

    // Token: 0x040000AD RID: 173
    private float liveBonus = 200f;

    // Token: 0x040000AE RID: 174
    private float totalLive;

    // Token: 0x040000AF RID: 175
    private float currentLive;

    // Token: 0x040000B0 RID: 176
    private GameObject imageCurrentLive;

    // Token: 0x040000B1 RID: 177
    private float offsetSubstract = 0.048f;

    // Token: 0x040000B2 RID: 178
    private float offsetPercent = 0.02666f;

    // Token: 0x040000B3 RID: 179
    [SerializeField]
    private float distanceChange = 1f;

    // Token: 0x040000B4 RID: 180
    [SerializeField]
    private int currentPosition;

    // Token: 0x040000B5 RID: 181
    [SerializeField]
    private GameObject tear;

    // Token: 0x040000B6 RID: 182
    private GameObject[] wayPoints;

    // Token: 0x040000B7 RID: 183
    private Vector2 nextPosition;

    // Token: 0x040000B8 RID: 184
    [SerializeField]
    private List<MiniHaunt> miniHaunts;

    // Token: 0x040000B9 RID: 185
    private Animator animator;

    // Token: 0x040000BA RID: 186
    private SpriteRenderer spriteRenderer;

    // Token: 0x040000BB RID: 187
    private float speed = 2f;

    // Token: 0x040000BC RID: 188
    private bool isActive;

    // Token: 0x040000BD RID: 189
    private int miniHauntsAlive = 3;

    // Token: 0x040000BE RID: 190
    [SerializeField]
    private float shotSpeed = 50f;

    // Token: 0x040000BF RID: 191
    [SerializeField]
    private float timeShot = 1f;
}
