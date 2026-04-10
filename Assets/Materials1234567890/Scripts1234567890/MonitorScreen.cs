using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorScreen : MonoBehaviour
{
    public GameObject screenRoot;
    public Image screenImage;
    public TMP_Text mainText;
    public TMP_Text statusText;

    public Sprite earthSprite;
    public Sprite blackHoleSprite;

    void Start()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);

        if (mainText != null)
            mainText.text = "";

        if (statusText != null)
            statusText.text = "";
    }

    public void TurnScreenOn()
    {
        if (screenRoot != null)
            screenRoot.SetActive(true);
    }

    public void TurnScreenOff()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);
    }

    public void ShowEarth()
    {
        TurnScreenOn();

        if (screenImage != null)
        {
            screenImage.enabled = true;
            screenImage.sprite = earthSprite;
        }

        if (mainText != null)
            mainText.text = "Earth";
    }

    public void ShowBlackHole()
    {
        TurnScreenOn();

        if (screenImage != null)
        {
            screenImage.enabled = true;
            screenImage.sprite = blackHoleSprite;
        }

        if (mainText != null)
            mainText.text = "Black Hole";
    }

    public void ShowStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    public void ClearStatus()
    {
        if (statusText != null)
            statusText.text = "";
    }
}