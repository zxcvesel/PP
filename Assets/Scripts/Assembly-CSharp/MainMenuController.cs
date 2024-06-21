using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000013 RID: 19
public class MainMenuController : MonoBehaviour
{
    // Token: 0x06000070 RID: 112 RVA: 0x00003C92 File Offset: 0x00001E92
    private void Start()
    {
        base.StartCoroutine("Animate");
    }

    // Token: 0x06000071 RID: 113 RVA: 0x00003CA0 File Offset: 0x00001EA0
    private void Update()
    {
        if (!this.menuShown && Input.anyKeyDown)
        {
            base.StopCoroutine("Animate");
            this.imageTitle.color = new Color(1f, 1f, 1f, 1f);
            this.imageIsaac.color = new Color(1f, 1f, 1f, 1f);
            this.imageMonsters.color = new Color(1f, 1f, 1f, 1f);
            this.ShowMenu();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            base.StartCoroutine("SelectItem");
        }
    }

    // Token: 0x06000072 RID: 114 RVA: 0x00003D5B File Offset: 0x00001F5B
    private IEnumerator SelectItem()
    {
        base.StartCoroutine("FadeOutImage", this.imageIsaac);
        base.StartCoroutine("FadeOutImage", this.imageTitle);
        base.StartCoroutine("FadeOutImage", this.imageMonsters);
        yield return new WaitUntil(() => this.imageMonsters.color.a <= 0f);
        SceneManager.LoadScene("NewGame");
        yield break;
    }

    // Token: 0x06000073 RID: 115 RVA: 0x00003D6A File Offset: 0x00001F6A
    private void ShowMenu()
    {
        this.textNewGame.gameObject.SetActive(true);
        this.menuShown = true;
        base.InvokeRepeating("BlinkMenuItem", 0f, 0.5f);
    }

    // Token: 0x06000074 RID: 116 RVA: 0x00003D99 File Offset: 0x00001F99
    private void BlinkMenuItem()
    {
        this.textNewGame.color = new Color(1f, 1f, 1f, -this.textNewGame.color.a);
    }

    // Token: 0x06000075 RID: 117 RVA: 0x00003DCB File Offset: 0x00001FCB
    private IEnumerator Animate()
    {
        yield return new WaitUntil(() => this.imageIsaac.color.a <= 0f);
        base.StartCoroutine("FadeInImage", this.imageIsaac);
        yield return new WaitUntil(() => this.imageIsaac.color.a >= 1f);
        base.StartCoroutine("FadeOutImage", this.imageIsaac);
        yield return new WaitUntil(() => this.imageIsaac.color.a <= 0f);
        base.StartCoroutine("FadeInImage", this.imageMonsters);
        yield return new WaitUntil(() => this.imageMonsters.color.a >= 1f);
        base.StartCoroutine("FadeOutImage", this.imageMonsters);
        yield return new WaitUntil(() => this.imageMonsters.color.a <= 0f);
        base.StartCoroutine("FadeInImage", this.imageIsaac);
        base.StartCoroutine("FadeInImage", this.imageMonsters);
        base.StartCoroutine("FadeInImage", this.imageTitle);
        yield return new WaitUntil(() => this.imageTitle.color.a >= 1f);
        this.ShowMenu();
        yield break;
    }

    // Token: 0x06000076 RID: 118 RVA: 0x00003DDA File Offset: 0x00001FDA
    private IEnumerator FadeInImage(Image image)
    {
        yield return new WaitForSeconds(0.0001f);
        while (image.color.a < 1f)
        {
            image.color = new Color(1f, 1f, 1f, image.color.a + 0.01f);
            yield return new WaitForSeconds(0.0001f);
        }
        yield break;
    }

    // Token: 0x06000077 RID: 119 RVA: 0x00003DE9 File Offset: 0x00001FE9
    private IEnumerator FadeOutImage(Image image)
    {
        yield return new WaitForSeconds(0.0001f);
        while (image.color.a > 0f)
        {
            image.color = new Color(1f, 1f, 1f, image.color.a - 0.01f);
            yield return new WaitForSeconds(0.0001f);
        }
        yield break;
    }

    // Token: 0x04000070 RID: 112
    [SerializeField]
    private Image imageMonsters;

    // Token: 0x04000071 RID: 113
    [SerializeField]
    private Image imageTitle;

    // Token: 0x04000072 RID: 114
    [SerializeField]
    private Image imageIsaac;

    // Token: 0x04000073 RID: 115
    [SerializeField]
    private TextMeshProUGUI textNewGame;

    // Token: 0x04000074 RID: 116
    private bool menuShown;

    // Token: 0x04000075 RID: 117
    private const float countFade = 0.01f;
}
