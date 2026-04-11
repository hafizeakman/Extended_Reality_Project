using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CollisionMinigameSystem : MonoBehaviour
{
    public enum SpaceObjectType
    {
        None,
        NeutronStar,
        BlackHole,
        YellowStar,
        MassiveStar,
        WhiteDwarf
    }

    [System.Serializable]
    public class SpaceObjectVisual
    {
        public SpaceObjectType type;
        public Sprite sprite;
    }

    [Header("Result Canvas 1")]
    public Image firstSelectionImage;
    public TMP_Text firstSelectionText;

    [Header("Result Canvas 2")]
    public Image secondSelectionImage;
    public TMP_Text secondSelectionText;

    [Header("Result Canvas 3")]
    public TMP_Text finalResultText;
    public Image finalResultImage;

    [Header("Object Visuals")]
    public SpaceObjectVisual[] objectVisuals;

    [Header("Messages")]
    [TextArea(2, 5)]
    public string defaultMessage = "These two objects do not create the special collision result.";

    [TextArea(2, 5)]
    public string blackHoleMessage = "Two black holes collide and merge, generating an extreme gravitational event.";

    [Header("Special Result Visual")]
    public Sprite blackHoleResultSprite;

    [Header("Rotation Trigger")]
    public Transform objectToRotate;
    public Vector3 rotationAmount = new Vector3(0f, 180f, 0f);
    public float rotationTime = 2f;

    private SpaceObjectType firstSelection = SpaceObjectType.None;
    private SpaceObjectType secondSelection = SpaceObjectType.None;
    private bool specialTriggered = false;

    private void Start()
    {
        ResetSelections();
    }

    // -------- FIRST SELECTION BUTTONS --------

    public void SelectFirstNeutronStar() => SetFirstSelection(SpaceObjectType.NeutronStar);
    public void SelectFirstBlackHole() => SetFirstSelection(SpaceObjectType.BlackHole);
    public void SelectFirstYellowStar() => SetFirstSelection(SpaceObjectType.YellowStar);
    public void SelectFirstMassiveStar() => SetFirstSelection(SpaceObjectType.MassiveStar);
    public void SelectFirstWhiteDwarf() => SetFirstSelection(SpaceObjectType.WhiteDwarf);

    // -------- SECOND SELECTION BUTTONS --------

    public void SelectSecondNeutronStar() => SetSecondSelection(SpaceObjectType.NeutronStar);
    public void SelectSecondBlackHole() => SetSecondSelection(SpaceObjectType.BlackHole);
    public void SelectSecondYellowStar() => SetSecondSelection(SpaceObjectType.YellowStar);
    public void SelectSecondMassiveStar() => SetSecondSelection(SpaceObjectType.MassiveStar);
    public void SelectSecondWhiteDwarf() => SetSecondSelection(SpaceObjectType.WhiteDwarf);

    private void SetFirstSelection(SpaceObjectType type)
    {
        firstSelection = type;

        if (firstSelectionText != null)
            firstSelectionText.text = FormatTypeName(type);

        if (firstSelectionImage != null)
        {
            firstSelectionImage.sprite = GetSpriteForType(type);
            firstSelectionImage.enabled = firstSelectionImage.sprite != null;
        }
    }

    private void SetSecondSelection(SpaceObjectType type)
    {
        secondSelection = type;

        if (secondSelectionText != null)
            secondSelectionText.text = FormatTypeName(type);

        if (secondSelectionImage != null)
        {
            secondSelectionImage.sprite = GetSpriteForType(type);
            secondSelectionImage.enabled = secondSelectionImage.sprite != null;
        }
    }

    public void CalculateResult()
    {
        if (firstSelection == SpaceObjectType.None || secondSelection == SpaceObjectType.None)
        {
            if (finalResultText != null)
                finalResultText.text = "Please choose both objects first.";

            if (finalResultImage != null)
            {
                finalResultImage.sprite = null;
                finalResultImage.enabled = false;
            }

            return;
        }

        bool isBlackHoleCombo =
            firstSelection == SpaceObjectType.BlackHole &&
            secondSelection == SpaceObjectType.BlackHole;

        if (isBlackHoleCombo)
        {
            if (finalResultText != null)
                finalResultText.text = blackHoleMessage;

            if (finalResultImage != null)
            {
                finalResultImage.sprite = blackHoleResultSprite;
                finalResultImage.enabled = blackHoleResultSprite != null;
            }

            if (!specialTriggered)
            {
                specialTriggered = true;

                if (objectToRotate != null)
                    StartCoroutine(RotateObjectOnce());
            }
        }
        else
        {
            if (finalResultText != null)
                finalResultText.text = defaultMessage;

            if (finalResultImage != null)
            {
                finalResultImage.sprite = null;
                finalResultImage.enabled = false;
            }
        }
    }

    public void ResetSelections()
    {
        firstSelection = SpaceObjectType.None;
        secondSelection = SpaceObjectType.None;

        if (firstSelectionText != null)
            firstSelectionText.text = "";

        if (secondSelectionText != null)
            secondSelectionText.text = "";

        if (firstSelectionImage != null)
        {
            firstSelectionImage.sprite = null;
            firstSelectionImage.enabled = false;
        }

        if (secondSelectionImage != null)
        {
            secondSelectionImage.sprite = null;
            secondSelectionImage.enabled = false;
        }

        if (finalResultText != null)
            finalResultText.text = "";

        if (finalResultImage != null)
        {
            finalResultImage.sprite = null;
            finalResultImage.enabled = false;
        }
    }

    private Sprite GetSpriteForType(SpaceObjectType type)
    {
        foreach (SpaceObjectVisual visual in objectVisuals)
        {
            if (visual != null && visual.type == type)
                return visual.sprite;
        }

        return null;
    }

    private string FormatTypeName(SpaceObjectType type)
    {
        switch (type)
        {
            case SpaceObjectType.NeutronStar: return "Neutron Star";
            case SpaceObjectType.BlackHole: return "Black Hole";
            case SpaceObjectType.YellowStar: return "Yellow Star";
            case SpaceObjectType.MassiveStar: return "Massive Star";
            case SpaceObjectType.WhiteDwarf: return "White Dwarf";
            default: return "";
        }
    }

    private IEnumerator RotateObjectOnce()
    {
        Quaternion startRotation = objectToRotate.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(rotationAmount);

        float elapsed = 0f;

        while (elapsed < rotationTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / rotationTime;
            objectToRotate.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        objectToRotate.rotation = targetRotation;
    }
}