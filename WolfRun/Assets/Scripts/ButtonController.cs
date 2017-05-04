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
        childText.text = threatText;
        childText.color = threatColor;
    }

    public void OnPointerExit(PointerEventData pointer)
    {
        childText.text = defaultText;
        childText.color = defaultColor;
    }
}
