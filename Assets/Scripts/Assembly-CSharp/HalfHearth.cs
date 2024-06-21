using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class HalfHearth : MonoBehaviour
{
    // Token: 0x0600005A RID: 90 RVA: 0x000036A3 File Offset: 0x000018A3
    private void Start()
    {
        this.animator = base.GetComponent<Animator>();
        base.StartCoroutine("Pump");
    }

    // Token: 0x0600005B RID: 91 RVA: 0x000036BD File Offset: 0x000018BD
    private IEnumerator Pump()
    {
        yield return new WaitForSecondsRealtime(3f);
        this.animator.Play("Pump");
        base.StartCoroutine("Pump");
        yield break;
    }

    // Token: 0x0400005D RID: 93
    private Animator animator;
}
