using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonitorScreen : MonoBehaviour
{
    public GameObject screenRoot;
    public Image screenImage;
    public TMP_Text screenText;

    public Sprite earthSprite;
    public Sprite blackHoleSprite;

    public string earthText = "Earth";
    public string blackHoleText = "Black Hole";

    private bool screenOn = false;
    private bool showingEarth = true;

    void Start()
    {
        UpdateScreen();
    }

    public void ToggleScreen()
    {
        if (!screenOn)
        {
            screenOn = true;
        }
        else
        {
            showingEarth = !showingEarth;
        }

        UpdateScreen();
    }

    public void TurnOn()
    {
        screenOn = true;
        UpdateScreen();
    }

    public void TurnOff()
    {
        screenOn = false;
        UpdateScreen();
    }

    public void ShowEarth()
    {
        screenOn = true;
        showingEarth = true;
        UpdateScreen();
    }

    public void ShowBlackHole()
    {
        screenOn = true;
        showingEarth = false;
        UpdateScreen();
    }

    private void UpdateScreen()
    {
        if (screenRoot != null)
            screenRoot.SetActive(screenOn);

        if (!screenOn) return;

        if (showingEarth)
        {
            if (screenImage != null)
                screenImage.sprite = earthSprite;

            if (screenText != null)
                screenText.text = earthText;
        }
        else
        {
            if (screenImage != null)
                screenImage.sprite = blackHoleSprite;

            if (screenText != null)
                screenText.text = blackHoleText;
        }
    }
}