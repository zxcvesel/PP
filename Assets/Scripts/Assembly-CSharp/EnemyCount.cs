using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000006 RID: 6
public class EnemyCount : MonoBehaviour
{
    // Token: 0x0600001F RID: 31 RVA: 0x00002632 File Offset: 0x00000832
    private void Start()
    {
        this.text = base.GetComponent<Text>();
    }

    // Token: 0x06000020 RID: 32 RVA: 0x00002640 File Offset: 0x00000840
    private void Update()
    {
        this.text.text = EnemyCount.enemys.ToString();
    }

    // Token: 0x04000018 RID: 24
    private Text text;

    // Token: 0x04000019 RID: 25
    public static int enemys;
}
