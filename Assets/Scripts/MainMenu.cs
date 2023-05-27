using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;

    private bool _mainTitleActive;

    // Start is called before the first frame update
    void Start()
    {
        _mainTitleActive = true;
        StartCoroutine(RainbowText(titleText));
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
        Debug.Log("Coroutine stopped");
    }

    public void StartGame()
    {
        _mainTitleActive = false;
        //StopCoroutine(RainbowText(titleText));
    }
}
