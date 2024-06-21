using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000011 RID: 17
public class KillCounter : MonoBehaviour
{
    // Token: 0x06000060 RID: 96 RVA: 0x00003743 File Offset: 0x00001943
    public void AddKill()
    {
        this.killCount++;
        this.killCountText.text = string.Concat(this.killCount);
    }

    // Token: 0x04000060 RID: 96
    public Text killCountText;

    // Token: 0x04000061 RID: 97
    private int killCount;
}
