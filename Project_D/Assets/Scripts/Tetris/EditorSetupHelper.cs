#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class EditorSetupHelper
{
    [MenuItem("Tools/Setup Tetris")]
    public static void Setup()
    {
        Sprite blockSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Block.png");

        GameObject blockGo = new GameObject("Block");
        var sr = blockGo.AddComponent<SpriteRenderer>();
        sr.sprite = blockSprite;
        blockGo.transform.localScale = new Vector3(100f/256f, 100f/256f, 1f);

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(blockGo, "Assets/Prefabs/Block.prefab");
        GameObject.DestroyImmediate(blockGo);

        GameObject boardGo = new GameObject("Board");
        
        // Ensure bottom left is nicely positioned
        boardGo.transform.position = new Vector3(-4.5f, -9.5f, 0); 
        var board = boardGo.AddComponent<Board>();

        GameObject pieceGo = new GameObject("ActivePiece");
        pieceGo.transform.SetParent(boardGo.transform);
        var piece = pieceGo.AddComponent<Piece>();

        GameObject gmGo = new GameObject("GameManager");
        var gm = gmGo.AddComponent<GameManager>();

        board.blockPrefab = prefab;
        board.activePiece = piece;
        
        board.tetrominos = new TetrominoData[7];
        for(int i = 0; i < 7; i++) {
            board.tetrominos[i] = new TetrominoData();
            board.tetrominos[i].tetromino = (Tetromino)i;
        }

        board.colors = new Color[] {
            Color.cyan, Color.blue, new Color(1f, 0.5f, 0f), Color.yellow, Color.green, new Color(0.5f, 0f, 0.5f), Color.red
        };

        gm.board = board;

        if (Camera.main != null)
        {
            Camera.main.orthographic = true;
            Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.orthographicSize = 12;
            Camera.main.transform.position = new Vector3(0, 0, -10);
        }

        Debug.Log("Tetris Setup Complete");
    }
}
#endif