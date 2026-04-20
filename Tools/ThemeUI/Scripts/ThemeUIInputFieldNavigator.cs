using UnityEngine;
using UnityEngine.UI;           // For Selectable
using TMPro;                    // For TMP_InputField
using UnityEngine.EventSystems;

public class ThemeUIInputFieldNavigator : MonoBehaviour
{
    [Tooltip("List your input fields in the order you want to tab through them")]
    public TMP_InputField[] inputFields;

    private EventSystem eventSystem;
    bool isShowing = false;

    public void OnShow()
    {
        isShowing = true;
        eventSystem = EventSystem.current;

        // Optional: Auto-focus the first field when the scene starts
        if (inputFields.Length > 0)
        {
            inputFields[0].Select();
            inputFields[0].ActivateInputField();
        }
    }

    public void OnHide()
    {
        isShowing = false;
    }

    void Update()
    {
        if (!isShowing)
            return;

        if (eventSystem.currentSelectedGameObject == null)
            return;

        // Check for Tab (forward and backward with Shift+Tab)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NavigateToNextField(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
        }

        // Check for Enter / Return (you can remove this if you only want Tab)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            NavigateToNextField(false);   // always go forward on Enter
        }
    }

    private void NavigateToNextField(bool goBackward)
    {
        GameObject currentGO = eventSystem.currentSelectedGameObject;
        TMP_InputField currentField = currentGO ? currentGO.GetComponent<TMP_InputField>() : null;

        if (currentField == null) return;

        // Find the index of the current field
        int currentIndex = System.Array.IndexOf(inputFields, currentField);
        if (currentIndex < 0) return;

        int nextIndex;

        if (goBackward)
        {
            nextIndex = (currentIndex - 1 + inputFields.Length) % inputFields.Length;
        }
        else
        {
            nextIndex = (currentIndex + 1) % inputFields.Length;
        }

        TMP_InputField nextField = inputFields[nextIndex];

        // This is important for TMP_InputField!
        nextField.Select();
        nextField.ActivateInputField();
    }
}