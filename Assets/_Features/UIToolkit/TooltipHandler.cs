using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Code from: https://discussions.unity.com/t/creating-tooltips/894849/6

public class TooltipHandler: Singleton<MonoBehaviour> {
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
        if (showTooltipCoroutine != null) {
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
        float mouseXPosition = Input.mousePosition.x;
        float mouseYPosition = Input.mousePosition.y;

        Vector2 textSize = tooltipLabel.MeasureTextSize(tooltipText.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined);
        tooltipLabel.visible = true;

        if (mouseXPosition >= screenWidth - (textSize.x)) {
            tooltipLabel.style.left = screenWidth - textSize.x - 5;
        } else {
            tooltipLabel.style.left = mouseXPosition + 5;
        }

        if (mouseYPosition >= screenHeight - textSize.y) {
            tooltipLabel.style.top = screenHeight - mouseYPosition;
            tooltipLabel.style.bottom = mouseYPosition - textSize.y - 15;
        } else {
            tooltipLabel.style.bottom = mouseYPosition;
            tooltipLabel.style.top = screenHeight - mouseYPosition - textSize.y - 15;
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
