using UnityEngine;
using UnityEngine.UI;

public class MobileInputHandler : MonoBehaviour
{
    private Board board;

    private void Awake()
    {
        SetupButtons();
        FindBoard();
    }

    private void SetupButtons()
    {
        AddListenerToButton("LeftButton", OnLeftButton);
        AddListenerToButton("RightButton", OnRightButton);
        AddListenerToButton("RotateButton", OnRotateButton);
        AddListenerToButton("SoftDropButton", OnSoftDropButton);
        AddListenerToButton("HardDropButton", OnHardDropButton);
    }

    private void AddListenerToButton(string childName, UnityEngine.Events.UnityAction action)
    {
        Transform t = transform.Find(childName);
        if (t != null)
        {
            Button b = t.GetComponent<Button>();
            if (b != null)
            {
                b.onClick.RemoveAllListeners();
                b.onClick.AddListener(action);
                Debug.Log($"MobileInputHandler: Linked {childName} successfully.");
            }
            else
            {
                Debug.LogWarning($"MobileInputHandler: Button component not found on {childName}");
            }
        }
        else
        {
            Debug.LogWarning($"MobileInputHandler: Child {childName} not found.");
        }
    }

    private void FindBoard()
    {
        board = GameObject.FindObjectOfType<Board>();
        if (board == null)
        {
            Debug.LogWarning("MobileInputHandler: Board not found in scene!");
        }
        else
        {
            Debug.Log("MobileInputHandler: Board found successfully.");
        }
    }

    public void OnLeftButton()
    {
        Debug.Log("MobileInputHandler: Left Button Pressed");
        if (board == null) FindBoard();
        if (board != null && board.activePiece != null)
        {
            board.activePiece.MoveLeft();
        }
    }

    public void OnRightButton()
    {
        Debug.Log("MobileInputHandler: Right Button Pressed");
        if (board == null) FindBoard();
        if (board != null && board.activePiece != null)
        {
            board.activePiece.MoveRight();
        }
    }

    public void OnRotateButton()
    {
        Debug.Log("MobileInputHandler: Rotate Button Pressed");
        if (board == null) FindBoard();
        if (board != null && board.activePiece != null)
        {
            board.activePiece.RotateClockwise();
        }
    }

    public void OnSoftDropButton()
    {
        Debug.Log("MobileInputHandler: Soft Drop Button Pressed");
        if (board == null) FindBoard();
        if (board != null && board.activePiece != null)
        {
            board.activePiece.MoveDown();
        }
    }

    public void OnHardDropButton()
    {
        Debug.Log("MobileInputHandler: Hard Drop Button Pressed");
        if (board == null) FindBoard();
        if (board != null && board.activePiece != null)
        {
            board.activePiece.TriggerHardDrop();
        }
    }
}
