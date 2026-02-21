using UnityEngine;
using UnityEditor;

public class FixPieceHierarchy : EditorWindow
{
    [MenuItem("Tools/Fix Piece Hierarchy")]
    public static void Setup()
    {
        Board board = FindObjectOfType<Board>();
        if (board != null)
        {
            Piece piece = board.GetComponent<Piece>();
            if (piece != null)
            {
                // Create a new GameObject for the active piece
                GameObject pieceGO = new GameObject("ActivePiece");
                Piece newPiece = pieceGO.AddComponent<Piece>();
                
                // Copy properties
                newPiece.stepDelay = piece.stepDelay;
                newPiece.lockDelay = piece.lockDelay;

                // Configure AudioSource
                AudioSource oldAudio = board.GetComponent<AudioSource>();
                if (oldAudio != null)
                {
                    AudioSource newAudio = pieceGO.AddComponent<AudioSource>();
                    newAudio.playOnAwake = oldAudio.playOnAwake;
                    newAudio.loop = oldAudio.loop;
                    
                    newPiece.audioSource = newAudio;
                    newPiece.moveSound = piece.moveSound;
                    newPiece.rotateSound = piece.rotateSound;
                    newPiece.lockSound = piece.lockSound;

                    // Remove audio source from Board since it belongs to Piece
                    DestroyImmediate(oldAudio);
                }

                // Update Board reference
                board.activePiece = newPiece;

                // Remove Piece component from Board
                DestroyImmediate(piece);

                EditorUtility.SetDirty(board);
                EditorUtility.SetDirty(pieceGO);

                Debug.Log("Piece hierarchy fixed. ActivePiece is now a separate GameObject.");
            }
            else
            {
                Debug.Log("Piece component not found on Board. It might already be separated.");
            }
        }
        else
        {
            Debug.Log("Board not found in scene.");
        }
    }
}