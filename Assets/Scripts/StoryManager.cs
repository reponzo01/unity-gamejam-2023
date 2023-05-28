using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoryManager : MonoBehaviour
{
    private static StoryManager _instance;

    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject panel9Effect;
    [SerializeField] private GameObject character;
    [SerializeField] private int typewriterWordsPerMinute = 400;

    private GameObject _activeStoryPanelGameObject;
    private Coroutine _activeTypewriterCoroutine;
    private Coroutine _panel9EffectCoroutine; // TODO: Delete if I don't need to stop this.
    private int _activeStoryPanelNumber = 0;
    private float _panel9EffectEffectDuration = 4f;
    private Vector3 _panel9EffectStartSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 _panel9EffectEndSize = new Vector3(4.5f, 4.5f, 4.5f);


    public static StoryManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Story Manager is NULL");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator StartPanel9Effect()
    {
        panel9Effect.SetActive(true);
        float timeElapsed = 0;
        while (timeElapsed < _panel9EffectEffectDuration)
        {
            panel9Effect.transform.localScale = Vector3.Slerp(
                _panel9EffectStartSize,
                _panel9EffectEndSize,
                timeElapsed / _panel9EffectEffectDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator Typewriter(TextMeshProUGUI textObject, int wordsPerMinute)
    {
        if (textObject == null) yield return null;

        var text = textObject.text;
        var timeElapsed = 0f;
        var secondsPerCharacter = 1 / ((wordsPerMinute * 5) / 60f);
        var charactersProcessed = 0;
        var textOnScreen = string.Empty;
        var colorTop = new Color(194f/255f, 232f/255f, 212f/255f);
        var colorBottom = new Color(95f/255f, 152f/255f, 201f/255f);
        textObject.colorGradient = new VertexGradient(colorTop, colorTop, colorBottom, colorBottom);
        textObject.SetText(string.Empty);
        while (charactersProcessed < text.Length)
        {
            if (timeElapsed >= secondsPerCharacter)
            {
                textOnScreen = text.Substring(0, charactersProcessed + 1);
                charactersProcessed++;
                textObject.SetText(textOnScreen);
                timeElapsed = 0;
            }
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private void Play2D()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(false);
        GameManager.Instance.Play2D();
    }

    private void Play3D()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(false);
        GameManager.Instance.Play3D();
    }

    private void PlayFinalPanel()
    {
        nextButton.SetActive(false);
        skipButton.SetActive(false);
        _activeStoryPanelNumber++;
        GoToStoryPanel(_activeStoryPanelNumber);
    }

    public void NextButtonClicked()
    {
        // Panels with special cases:
        // 5 -> Switch to 2D
        // 8 -> Switch to 3D
        // 9 -> Explosion -> 3D Character -> Panel 10
        if (_activeTypewriterCoroutine != null)
        {
            StopCoroutine(_activeTypewriterCoroutine);
        }
        _activeStoryPanelGameObject.SetActive(false);
        if (_activeStoryPanelNumber == 5)
        {
            Play2D();
        }
        else if (_activeStoryPanelNumber == 8)
        {
            Play3D();
        }
        else if (_activeStoryPanelNumber == 9)
        {
            PlayFinalPanel();
        }
        else
        {
            _activeStoryPanelNumber++;
            GoToStoryPanel(_activeStoryPanelNumber);
        }
    }

    // TODO: Still need skip button functionality
    public void SkipButtonClicked()
    {
        if (_activeTypewriterCoroutine != null)
        {
            StopCoroutine(_activeTypewriterCoroutine);
        }
        _activeStoryPanelGameObject.SetActive(false);
        if (_activeStoryPanelNumber <= 5)
        {
            Play2D();
        }
        else if (_activeStoryPanelNumber <= 8)
        {
            Play3D();
        }
        else if (_activeStoryPanelNumber <= 9)
        {
            PlayFinalPanel();
        }
        // _activeStoryPanelGameObject.SetActive(false);
        // nextButton.SetActive(false);
        // skipButton.SetActive(false);
    }

    public void StartStory()
    {
        PlayerPrefs.SetInt("IsFirstTimePlay", 0);
        _activeStoryPanelNumber = 1;
        GoToStoryPanel(_activeStoryPanelNumber);
    }

    public void GoToStoryPanel(int storyPanelNumber)
    {
        GameManager.Instance.isStoryMode = true;
        CanvasManager.Instance.ShowScores(false);
        var storyPanel = transform.Find($"Panel{storyPanelNumber}");
        if (storyPanel != null)
        {
            _activeStoryPanelGameObject = storyPanel.gameObject;
            _activeStoryPanelGameObject.SetActive(true);
            _activeStoryPanelNumber = storyPanelNumber;
            if (_activeStoryPanelNumber == 9)
            {
                _panel9EffectCoroutine = StartCoroutine(StartPanel9Effect());
            }
            if (_activeStoryPanelNumber == 10)
            {
                CanvasManager.Instance.ShowScores(true);
                character.SetActive(true);
            }
            else
            {
                nextButton.SetActive(true);
                skipButton.SetActive(true);
            }
            var typewriterTextGameObject = storyPanel.Find("TypewriterText");
            if (typewriterTextGameObject == null) return;
            var typewriterText = typewriterTextGameObject.GetComponent<TextMeshProUGUI>();
            if (typewriterText == null) return;
            _activeTypewriterCoroutine = StartCoroutine(Typewriter(typewriterText, typewriterWordsPerMinute));
        }
    }
}
