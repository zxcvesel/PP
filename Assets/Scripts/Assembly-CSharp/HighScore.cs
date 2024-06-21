using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000010 RID: 16
public class HighScore : MonoBehaviour
{
    // Token: 0x0600005D RID: 93 RVA: 0x000036CC File Offset: 0x000018CC
    private void Start()
    {
        this.highScoreText.text = string.Concat(PlayerPrefs.GetInt(this.highScoreKey));
    }

    // Token: 0x0600005E RID: 94 RVA: 0x000036F0 File Offset: 0x000018F0
    public void CheckHighScore(int currentScore)
    {
        int @int = PlayerPrefs.GetInt(this.highScoreKey, 0);
        if (currentScore > @int)
        {
            PlayerPrefs.SetInt(this.highScoreKey, currentScore);
            this.highScoreText.text = string.Concat(currentScore);
        }
    }

    // Token: 0x0400005E RID: 94
    public Text highScoreText;

    // Token: 0x0400005F RID: 95
    private string highScoreKey = "HighScore";
}
