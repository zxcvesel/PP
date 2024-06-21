using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class EnemyTear : MonoBehaviour
{
    // Token: 0x0600002F RID: 47 RVA: 0x00002AC8 File Offset: 0x00000CC8
    private void OnCollisionEnter2D(Collision2D collision)
    {
        base.transform.lossyScale.Set(1f, 1f, 1f);
        if (collision.gameObject.tag != "Enemy" && collision.gameObject.tag != "EnemyTear")
        {
            this.StartAnim();
        }
    }

    // Token: 0x06000030 RID: 48 RVA: 0x00002B2C File Offset: 0x00000D2C
    public void StartAnim()
    {
        base.GetComponent<Animator>().Play("Touch");
        base.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
        AudioSource.PlayClipAtPoint(this.touchedClip, base.transform.position);
        base.Invoke("Destroy", 0.5f);
    }

    // Token: 0x06000031 RID: 49 RVA: 0x00002B89 File Offset: 0x00000D89
    public void Destroy()
    {
        UnityEngine.Object.Destroy(base.gameObject);
    }

    // Token: 0x0400002D RID: 45
    [SerializeField]
    private AudioClip touchedClip;
}
