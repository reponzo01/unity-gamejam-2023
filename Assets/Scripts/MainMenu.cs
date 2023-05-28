using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject storyButton;
    [SerializeField] private GameObject zenButton;

    private bool _mainTitleActive;

    // Start is called before the first frame update
    void Start()
    {
        _mainTitleActive = true;
        if (PlayerPrefs.GetInt("IsFirstTimePlay", 1) == 0)
        {
            startButton.SetActive(false);
            playButton.SetActive(true);
            storyButton.SetActive(true);
        }
        if (PlayerPrefs.GetInt("IsGameFinished", 0) == 1)
        {
            zenButton.SetActive(true);
        }
        StartCoroutine(RainbowText(titleText));
        AudioManager.Instance.PlayMusicMainMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private (bool colorIncrement, int colorValue) UpdateColor(bool colorIncrement, int colorValue)
    {
        if (colorIncrement)
        {
            if (colorValue == 255)
            {
                colorValue--;
                colorIncrement = false;
            }
            else
            {
                colorValue++;
            }
        }
        else
        {
            if (colorValue == 0)
            {
                colorValue++;
                colorIncrement = true;
            }
            else
            {
                colorValue--;
            }
        }
        return (colorIncrement, colorValue);
    }

    private IEnumerator RainbowText(TextMeshProUGUI textObject)
    {
        float timeElapsed = 0f;
        var secondsForFullRainbow = 4f;
        var secondsPerColorIncrement = secondsForFullRainbow / 255f;
        var colorR = Random.Range(1, 256);
        var colorG = Random.Range(1, 256);
        var colorB = Random.Range(1, 256);

        var colorRIncrement = true;
        var colorGIncrement = true;
        var colorBIncrement = true;

        while (_mainTitleActive)
        {
            if (timeElapsed >= secondsPerColorIncrement)
            {
                Color color = new Color(colorR/255f, colorG/255f, colorB/255f);
                textObject.color = color;
                (colorRIncrement, colorR) = UpdateColor(colorRIncrement, colorR);
                (colorGIncrement, colorG) = UpdateColor(colorGIncrement, colorG);
                (colorBIncrement, colorB) = UpdateColor(colorBIncrement, colorB);
                timeElapsed = 0;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void ShowMainMenu(bool show)
    {
        gameObject.SetActive(show);
    }

    public void ZenButtonClicked()
    {
        _mainTitleActive = false;
        AudioManager.Instance.StopAllAudio();
        GameManager.Instance.isZenMode = true;
        GameManager.Instance.Play2D();
        CanvasManager.Instance.ActivateZenMode();
        ShowMainMenu(false);
    }

    public void PlayButtonClicked()
    {
        _mainTitleActive = false;
        AudioManager.Instance.StopAllAudio();
        GameManager.Instance.Play2D();
        ShowMainMenu(false);
    }

    public void StartButtonClicked()
    {
        StoryButtonClicked();
    }

    public void StoryButtonClicked()
    {
        _mainTitleActive = false;
        StoryManager.Instance.StartStory();
        ShowMainMenu(false);
    }
}
