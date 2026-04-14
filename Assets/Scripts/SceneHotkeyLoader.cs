using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneHotkeyLoader : MonoBehaviour
{
    [SerializeField] private string sceneName = "IntroSlides";
    [SerializeField] private Key triggerKey = Key.P;
    [SerializeField] private bool requireShift;
    [SerializeField] private bool requireCtrl;
    [SerializeField] private bool requireAlt;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (!keyboard[triggerKey].wasPressedThisFrame)
        {
            return;
        }

        if (!AreModifierKeysPressed(keyboard))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("SceneHotkeyLoader: configure o nome da cena a ser carregada.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private bool AreModifierKeysPressed(Keyboard keyboard)
    {
        bool shiftPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
        bool ctrlPressed = keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed;
        bool altPressed = keyboard.leftAltKey.isPressed || keyboard.rightAltKey.isPressed;

        if (requireShift && !shiftPressed)
        {
            return false;
        }

        if (requireCtrl && !ctrlPressed)
        {
            return false;
        }

        if (requireAlt && !altPressed)
        {
            return false;
        }

        return true;
    }
}
