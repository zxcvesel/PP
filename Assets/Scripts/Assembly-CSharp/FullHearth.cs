using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200000D RID: 13
public class FullHearth : MonoBehaviour
{
    // Token: 0x06000046 RID: 70 RVA: 0x00003173 File Offset: 0x00001373
    private void Start()
    {
        this.animator = base.GetComponent<Animator>();
        base.StartCoroutine("Pump");
    }

    // Token: 0x06000047 RID: 71 RVA: 0x0000318D File Offset: 0x0000138D
    private IEnumerator Pump()
    {
        yield return new WaitForSecondsRealtime(3f);
        this.animator.Play("Pump");
        base.StartCoroutine("Pump");
        yield break;
    }

    // Token: 0x04000043 RID: 67
    private Animator animator;
}
