using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private GameObject threeDControls;
    [SerializeField] private GameObject powerupMatchIcon;
    [SerializeField] private GameObject powerupMultiselectIcon;
    [SerializeField] private TextMeshProUGUI multiselectTilesAvailableText;
    [SerializeField] private TextMeshProUGUI multiselectInstructionsText;

    // Start is called before the first frame update
    void Start()
    {
        SwitchTo2D();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchTo3D()
    {
        threeDControls.SetActive(true);
    }

    public void SwitchTo2D()
    {
        threeDControls.SetActive(false);
    }

    public void ShowPowerupIcon(Utilities.PowerupEnum powerup, bool show)
    {
        switch (powerup)
        {
            case Utilities.PowerupEnum.match:
                powerupMatchIcon.SetActive(show);
                break;
            case Utilities.PowerupEnum.multiselect:
                powerupMultiselectIcon.SetActive(show);
                break;
            default:
                break;
        }
    }

    // Method name is used in UI for image click
    public void ShowPowerupInstructions(GameObject powerupInstructions)
    {
        powerupInstructions.SetActive(true);
        GameManager.Instance.isPowerupInstructionsActive = true;
    }

    // Method name is used in UI
    public void HidePowerupInstructions(GameObject powerupInstructions)
    {
        powerupInstructions.SetActive(false);
        GameManager.Instance.isPowerupInstructionsActive = false;
    }

    public void UpdateMultiselectTilesAvailable(int tilesAvailable)
    {
        multiselectTilesAvailableText.SetText(tilesAvailable.ToString());
        multiselectInstructionsText.SetText($"Select up to {tilesAvailable} tiles at once!");
    }
}
