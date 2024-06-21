using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private void Awake()
    {
        if (GameController._instance == null)
        {
            GameController._instance = this;
            UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
            return;
        }
        UnityEngine.Object.Destroy(base.gameObject);
    }

    public static float damagePassiveItems = 10f;

    private AudioSource audioSource;
    private List<Image> hearthImages;
    private int currentPosition;
    private GameController.containerState currentContainerState;
    private bool startedLevel;
    private bool CheatMode;
    private bool gamePaused;
    private bool gameFinished;

    [SerializeField] private int level = 1;
    [SerializeField] private Sprite fullContainer;
    [SerializeField] private Sprite halfContainer;
    [SerializeField] private Sprite emptyContainer;
    //[SerializeField] private GameObject waitingStartImage;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject bossFightPanel;
    [SerializeField] private Text informationText;
    [SerializeField] private AudioClip mainSong;
    [SerializeField] private AudioClip bossSong;
    [SerializeField] private AudioClip BossFightAudio;
    [SerializeField] private AudioClip waitingAudio;
    [SerializeField] private TextMeshProUGUI bossNameTitle;
    [SerializeField] private Sprite[] imageBossFight;

    private const float countFade = 0.01f;

    private static GameController _instance;

    public enum containerState
    {
        Full,
        Half,
        Empty
    }

    private void Start()
    {
        this.audioSource = base.GetComponent<AudioSource>();
        this.hearthImages = new List<Image>();
        foreach (Image image in base.GetComponentsInChildren<Image>())
        {
            if (image.tag == "Container")
            {
                this.hearthImages.Add(image);
                image.sprite = this.fullContainer;
            }
        }
        this.currentPosition = this.hearthImages.Count - 1;
        this.currentContainerState = GameController.containerState.Full;

        // Инициализация waitingStartImage, если он не был присвоен
        //if (waitingStartImage == null)
        //{
        //    Debug.LogError("waitingStartImage is not assigned in GameController!");
        //}
    }

    public void Damaged()
    {
        Image image = this.hearthImages[this.currentPosition];
        GameController.containerState containerState = this.currentContainerState;
        if (containerState == GameController.containerState.Full)
        {
            image.sprite = this.halfContainer;
            this.currentContainerState = GameController.containerState.Half;
            return;
        }
        if (containerState != GameController.containerState.Half)
        {
            return;
        }
        image.sprite = this.emptyContainer;
        this.currentContainerState = GameController.containerState.Full;
        this.currentPosition--;
    }

    public void Healthed(int amountInHalfHealths)
    {
        for (int i = 0; i < amountInHalfHealths; i++)
        {
            Image image = this.hearthImages[this.currentPosition];
            switch (this.currentContainerState)
            {
                case GameController.containerState.Full:
                    this.currentPosition++;
                    image = this.hearthImages[this.currentPosition];
                    image.sprite = this.halfContainer;
                    this.currentContainerState = GameController.containerState.Half;
                    break;
                case GameController.containerState.Half:
                    image.sprite = this.fullContainer;
                    this.currentContainerState = GameController.containerState.Full;
                    break;
                case GameController.containerState.Empty:
                    image.sprite = this.halfContainer;
                    this.currentContainerState = GameController.containerState.Half;
                    break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !this.gameFinished)
        {
            if (Time.timeScale == 0f)
            {
                this.gamePaused = false;
                Time.timeScale = 1f;
                this.audioSource.Play();
                this.pausePanel.SetActive(false);
            }
            else
            {
                this.gamePaused = true;
                Time.timeScale = 0f;
                this.audioSource.Pause();
                this.pausePanel.SetActive(true);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && this.gameFinished)
        {
            SceneManager.LoadScene("MainMenu");
            UnityEngine.Object.Destroy(UnityEngine.Object.FindObjectOfType<Player>().gameObject);
            Time.timeScale = 1f;
            UnityEngine.Object.Destroy(base.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.Space) && this.gamePaused)
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Space) && Input.GetKeyDown(KeyCode.Return))
        {
            this.CheatMode = true;
        }
        if (this.CheatMode)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                this.Healthed(1);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                this.Healthed(2);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                UnityEngine.Object.FindObjectOfType<Player>().GetTears().GetComponent<Tear>().SetDamage(200);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                UnityEngine.Object.FindObjectOfType<Player>().GetTears().GetComponent<Tear>().SetDamage(20);
            }
        }
    }

    public int GetLevel()
    {
        return this.level;
    }

    //private IEnumerator FadeOutWaitingStartImage()
    //{
    //    this.audioSource.clip = this.waitingAudio;
    //    this.audioSource.Play();

    //    yield return new WaitForSeconds(1.5f);

    //    Image[] waitingImages = this.waitingStartImage.GetComponentsInChildren<Image>();

    //    // Проверка, что waitingImages не равен null и имеет как минимум один элемент
    //    if (waitingImages != null && waitingImages.Length > 0)
    //    {
    //        float fadeSpeed = 0.01f; // Скорость затухания

    //        // Цикл затухания изображений
    //        while (waitingImages[0].color.a > 0f)
    //        {
    //            foreach (Image image in waitingImages)
    //            {
    //                Color newColor = new Color(image.color.r, image.color.g, image.color.b, Mathf.Max(image.color.a - fadeSpeed, 0f));
    //                image.color = newColor;
    //            }
    //            yield return new WaitForSeconds(0.01f); // Ожидание 0.01 секунды
    //        }
    //    }

    //    // После завершения затухания
    //    this.startedLevel = true;
    //    this.audioSource.clip = this.mainSong;
    //    this.audioSource.loop = true;
    //    this.audioSource.Play();
    //}

    private IEnumerator SetMessageItem(string message)
    {
        this.informationText.gameObject.SetActive(true);
        this.informationText.text = message;
        yield return new WaitForSecondsRealtime(1.25f);
        this.informationText.gameObject.SetActive(false);
    }

    private IEnumerator ShowImageBossFight(GameObject boss)
    {
        this.audioSource.clip = this.BossFightAudio;
        this.audioSource.loop = false;
        this.audioSource.Play();
        this.bossFightPanel.transform.Find("BossImage").gameObject.GetComponent<Image>().sprite = this.GetImageBossWithName(boss.name);
        this.bossNameTitle.SetText = boss.name.Replace("(Clone)", "");
        this.bossFightPanel.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitWhile(() => this.audioSource.isPlaying);
        Time.timeScale = 1f;
        this.bossFightPanel.SetActive(false);
        this.audioSource.clip = this.bossSong;
        this.audioSource.Play();
        boss.SetActive(true);
    }

    private void FinishRun()
    {
        this.gameFinished = true;
        this.gamePaused = true;
        Time.timeScale = 0f;
        this.audioSource.Pause();
        this.pausePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText = "GAME OVER"; // Исправлено SetText на text
        this.pausePanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText = "Press escape to main menu"; // Исправлено SetText на text
        this.pausePanel.SetActive(true);
    }

    public bool GetStartedLevel()
    {
        return this.startedLevel;
    }

    public bool GetGameIsPaused()
    {
        return this.gamePaused;
    }

    public void NextLevel()
    {
        UnityEngine.Object.FindObjectOfType<Player>().transform.position = Vector3.zero;
        this.level++;
        GameController.damagePassiveItems = this.level * 40;
        this.startedLevel = false;
        UnityEngine.Object.FindObjectOfType<Player>().SetHasTreasureRoomKey(false);
        // Image[] componentsInChildren = this.waitingStartImage.GetComponentsInChildren<Image>(); // Это было до удаления FadeOutWaitingStartImage
        SceneManager.LoadScene("NewGame");
        // foreach (Image image in componentsInChildren)
        // {
        //     image.color = Color.black;
        // }
        UnityEngine.Object.FindObjectOfType<CameraController>().SendMessage("ResetPosition");
        base.transform.position = new Vector3(0f, 0f, 15f);
    }

    private Sprite GetImageBossWithName(string nameBoss)
    {
        nameBoss = nameBoss.Replace("(Clone)", "");
        if (nameBoss == "Dingle")
        {
            return this.imageBossFight[0];
        }
        if (nameBoss == "The Haunt")
        {
            return this.imageBossFight[1];
        }
        return null;
    }
}