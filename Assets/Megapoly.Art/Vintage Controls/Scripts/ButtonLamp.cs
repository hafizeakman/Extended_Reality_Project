using UnityEngine;

public class ButtonLamp : MonoBehaviour
{
    public enum eColor
    {
        Red,
        Yellow,
        Green,
        Blue,
    }

    public bool on;
    public Transform lamp;
    public eColor lightColor;

    private Renderer rend;

    void Start()
    {
        rend = lamp.GetComponent<Renderer>();
        SetLampState(); // set initial state
    }

    
    public void TurnOn()
    {
        on = true;
        SetLampState();
    }

    public void TurnOff()
    {
        on = false;
        SetLampState();
    }

    // optional toggle
    public void Toggle()
    {
        on = !on;
        SetLampState();
    }

    void SetLampState()
    {
        if (rend == null) return;

        if (on)
        {
            Color emissionColor = Color.black;

            switch (lightColor)
            {
                case eColor.Red:
                    emissionColor = new Color(3f, 0f, 0f);
                    break;
                case eColor.Yellow:
                    emissionColor = new Color(3f, 2f, 0f);
                    break;
                case eColor.Green:
                    emissionColor = new Color(0f, 3f, 0f);
                    break;
                case eColor.Blue:
                    emissionColor = new Color(0f, 1f, 3f);
                    break;
            }

            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", emissionColor);
        }
        else
        {
            rend.material.SetColor("_EmissionColor", Color.black);
        }
    }
}