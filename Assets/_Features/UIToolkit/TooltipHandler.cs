using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Code from: https://discussions.unity.com/t/creating-tooltips/894849/6

public class TooltipHandler: Singleton<TooltipHandler> {
    //Create and hide a label in the UI Builder and name it "tooltipLabel". 
    //Set its position as 'Absolute'.
    //Set Shrink to 1, Grow to 1, Direction to 'Row', Wrap to 'Wrap', max width 
    //to 10 or 15% or however large you want your tooltip to be, max height to 'none', 
    //and then min height/width to whatever looks good.

    Label tooltipLabel;
    VisualElement root;
    StringObject tooltipText;
    bool tooltipVisible;

    Coroutine showTooltipCoroutine;
    const float tooltipDelay = 0.5f; // delay in seconds

    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
        tooltipLabel = root.Q<Label>("tooltipLabel");
        tooltipLabel.visible = false;
    }

    private void Update() {
        if (tooltipVisible) {
            tooltipLabel.text = tooltipText.text;
        }
    }

    public void OnElementMouseOver(StringObject _tooltipText = null) {
        if (showTooltipCoroutine != null) { // Stop old one
            StopCoroutine(showTooltipCoroutine);
        }
        tooltipText = _tooltipText;
        showTooltipCoroutine = StartCoroutine(ShowTooltipAfterDelay(tooltipDelay));
    }

    private IEnumerator ShowTooltipAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        tooltipVisible = true;

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        float uiCanvasWidth = root.resolvedStyle.width;
        float uiCanvasHeight = root.resolvedStyle.height;

        float mouseXPosition = Input.mousePosition.x; // Relative to Screen.width
        float mouseYPosition = Input.mousePosition.y; // Relative to Screen.height

        float canvasScaleFactor = root.resolvedStyle.width / Screen.width; // Scale between "a reference resolution of a UI document canvas (1920x1080)" and Screen resolution
        float mouseXPosition_Scaled = Input.mousePosition.x * canvasScaleFactor; // root.resolvedStyle.width (a reference resolution of a UI document canvas, usually 1920x1080)
        float mouseYPosition_Scaled = Input.mousePosition.y * canvasScaleFactor; // root.resolvedStyle.height

        Vector2 textSize = tooltipLabel.MeasureTextSize(tooltipText.text, tooltipLabel.resolvedStyle.maxWidth.value, VisualElement.MeasureMode.AtMost, 0, VisualElement.MeasureMode.Undefined);
        textSize /= canvasScaleFactor; // Scale to Screen sizes
        tooltipLabel.visible = true;

        if (mouseXPosition >= screenWidth - textSize.x) { // In screen resolution units
            tooltipLabel.style.left = (screenWidth - textSize.x - 5) * canvasScaleFactor; // In reference canvas resolution units (1920x1080)
        } else {
            tooltipLabel.style.left = (mouseXPosition + 5) * canvasScaleFactor;
        }

        if (mouseYPosition >= screenHeight - textSize.y) {
            tooltipLabel.style.top = (screenHeight - mouseYPosition);
            tooltipLabel.style.bottom = (mouseYPosition - textSize.y - 15) * canvasScaleFactor;
        } else {
            tooltipLabel.style.bottom = (mouseYPosition) * canvasScaleFactor;
            tooltipLabel.style.top = (screenHeight - mouseYPosition - textSize.y - 15) * canvasScaleFactor;
        }

        tooltipLabel.text = tooltipText.text;
    }

    public void OnElementMouseLeave() {
        if (showTooltipCoroutine != null) {
            StopCoroutine(showTooltipCoroutine);
            showTooltipCoroutine = null;
        }

        tooltipVisible = false;
        tooltipLabel.visible = false;
    }
}

//You'll need this if you want to dynamically update a tooltip while it is visible. Strings are immutable, so they can't be changed once assigned. Wrapping a string in a class gets around this.
public class StringObject {
    public string text;

    public StringObject() {

    }
    public StringObject(string text) {
        this.text = text;
    }
}
