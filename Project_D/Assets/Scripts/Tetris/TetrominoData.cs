using UnityEngine;
using System.Collections.Generic;

// 테트리미노 종류 정의 (I, J, L, O, S, T, Z)
public enum Tetromino
{
    I, J, L, O, S, T, Z
}

[System.Serializable]
public struct TetrominoData
{
    public Tetromino tetromino; // 테트리미노 종류
    public Vector2Int[] cells;  // 블록을 구성하는 좌표들
    public Vector2Int[,] wallKicks; // 회전 시 벽 충돌 보정 데이터

    public void Initialize()
    {
        // 표준 테트리미노 모양 데이터(4개의 블록)를 할당합니다.
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks[tetromino];
    }
}

public static class Data
{
    public static readonly float Cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float Sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = new float[] { Cos, Sin, -Sin, Cos };

    // 각 테트리미노의 표준 모양 좌표 (4개의 블록으로 구성)
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.I, new Vector2Int[] { new Vector2Int(-1,  1), new Vector2Int( 0,  1), new Vector2Int( 1,  1), new Vector2Int( 2,  1) } },
        { Tetromino.J, new Vector2Int[] { new Vector2Int(-1,  1), new Vector2Int(-1,  0), new Vector2Int( 0,  0), new Vector2Int( 1,  0) } },
        { Tetromino.L, new Vector2Int[] { new Vector2Int( 1,  1), new Vector2Int(-1,  0), new Vector2Int( 0,  0), new Vector2Int( 1,  0) } },
        { Tetromino.O, new Vector2Int[] { new Vector2Int( 0,  1), new Vector2Int( 1,  1), new Vector2Int( 0,  0), new Vector2Int( 1,  0) } },
        { Tetromino.S, new Vector2Int[] { new Vector2Int( 0,  1), new Vector2Int( 1,  1), new Vector2Int(-1,  0), new Vector2Int( 0,  0) } },
        { Tetromino.T, new Vector2Int[] { new Vector2Int( 0,  1), new Vector2Int(-1,  0), new Vector2Int( 0,  0), new Vector2Int( 1,  0) } },
        { Tetromino.Z, new Vector2Int[] { new Vector2Int(-1,  1), new Vector2Int( 0,  1), new Vector2Int( 0,  0), new Vector2Int( 1,  0) } }
    };

    // I 테트리미노용 벽 차기 데이터
    private static readonly Vector2Int[,] WallKicksI = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int( 2, 1) },
        { new Vector2Int(0, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 1), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-2, 0), new Vector2Int( 1, 0), new Vector2Int(-2,-1), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int(-2, 0), new Vector2Int( 1,-2), new Vector2Int(-2, 1) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int( 2, 0), new Vector2Int(-1, 2), new Vector2Int( 2,-1) }
    };

    // 나머지 테트리미노용 벽 차기 데이터
    private static readonly Vector2Int[,] WallKicksJLOSTZ = new Vector2Int[,] {
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1,-1), new Vector2Int(0, 2), new Vector2Int( 1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0,-2), new Vector2Int(-1,-2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(-1,-1), new Vector2Int(0, 2), new Vector2Int(-1, 2) },
        { new Vector2Int(0, 0), new Vector2Int( 1, 0), new Vector2Int( 1, 1), new Vector2Int(0,-2), new Vector2Int( 1,-2) }
    };

    public static readonly Dictionary<Tetromino, Vector2Int[,]> WallKicks = new Dictionary<Tetromino, Vector2Int[,]>()
    {
        { Tetromino.I, WallKicksI },
        { Tetromino.J, WallKicksJLOSTZ },
        { Tetromino.L, WallKicksJLOSTZ },
        { Tetromino.O, WallKicksJLOSTZ },
        { Tetromino.S, WallKicksJLOSTZ },
        { Tetromino.T, WallKicksJLOSTZ },
        { Tetromino.Z, WallKicksJLOSTZ }
    };
}
