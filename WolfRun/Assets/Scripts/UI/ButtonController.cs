using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string threatText;
    public Color threatColor;

    private Text childText;
    private string defaultText;
    private Color defaultColor;

    private void Start()
    {
        childText = GetComponentInChildren<Text>();
        defaultText = childText.text;
        defaultColor = childText.color;
    }

    public void OnPointerEnter(PointerEventData pointer)
    {
        SetBloodEffect(true);
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        SetBloodEffect(false);
    }

    private void OnDisable()
    {
        SetBloodEffect(false);
    }

    private void SetBloodEffect(bool shouldSet)
    {
        if( !childText )
        {
            return;
        }

        childText.text = shouldSet ? threatText : defaultText;
        childText.color = shouldSet ? threatColor : defaultColor;
    }
}
