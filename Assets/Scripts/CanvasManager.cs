using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
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
        transform.Find("3DControls").gameObject.SetActive(true);
    }

    public void SwitchTo2D()
    {
        transform.Find("3DControls").gameObject.SetActive(false);
    }

    public void ShowPowerupIcon(Utilities.PowerupEnum powerup, bool show)
    {
        switch (powerup)
        {
            case Utilities.PowerupEnum.match:
                transform.Find("Powerups").Find("Match").gameObject.SetActive(show);
                break;
            case Utilities.PowerupEnum.multiselect:
                transform.Find("Powerups").Find("Multiselect").gameObject.SetActive(show);
                break;
            default:
                break;
        }
    }

    // Method name is used in UI
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
}
