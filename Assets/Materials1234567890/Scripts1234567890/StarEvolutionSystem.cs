using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StarEvolutionSystem : MonoBehaviour
{
    [System.Serializable]
    public class StarResult
    {
        [Header("Combination")]
        [Range(1, 5)] public int mass;
        [Range(1, 5)] public int time;

        [Header("Display Content")]
        public string title;

        [TextArea(2, 5)]
        public string description;

        public Sprite image;
    }

    [Header("Control Canvas")]
    public Slider massSlider;
    public Slider timeSlider;

    [Header("Result Canvas")]
    public TMP_Text resultTitleText;
    public TMP_Text resultDescriptionText;
    public Image resultImage;

    [Header("All 25 Results")]
    public StarResult[] results = new StarResult[25];

    public void ShowResult()
    {
        int selectedMass = Mathf.RoundToInt(massSlider.value);
        int selectedTime = Mathf.RoundToInt(timeSlider.value);

        StarResult match = FindResult(selectedMass, selectedTime);

        if (match != null)
        {
            resultTitleText.text = match.title;
            resultDescriptionText.text = match.description;

            if (match.image != null)
            {
                resultImage.sprite = match.image;
                resultImage.enabled = true;
            }
            else
            {
                resultImage.sprite = null;
                resultImage.enabled = false;
            }
        }
        else
        {
            resultTitleText.text = "Unknown Result";
            resultDescriptionText.text = "No matching result found.";
            resultImage.sprite = null;
            resultImage.enabled = false;
        }
    }

    private StarResult FindResult(int massValue, int timeValue)
    {
        foreach (StarResult result in results)
        {
            if (result != null && result.mass == massValue && result.time == timeValue)
            {
                return result;
            }
        }

        return null;
    }
}