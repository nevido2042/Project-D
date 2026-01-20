using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Click_Increase_Text : MonoBehaviour
{
    public TMP_Text text;
    int cnt = 0;

    public InputActionAsset inputActions;
    InputAction clickAction;

    void Awake()
    {
        // Action Map과 Action 이름은 Input Action Asset에 맞게 수정
        clickAction = inputActions.FindActionMap("Player").FindAction("Click");
    }

    void OnEnable()
    {
        clickAction.Enable();
        clickAction.performed += OnClick;
    }

    void OnDisable()
    {
        clickAction.performed -= OnClick;
        clickAction.Disable();
    }

    void OnClick(InputAction.CallbackContext context)
    {
        cnt++;
        text.text = cnt.ToString();
    }
}
