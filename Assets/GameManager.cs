using UnityEngine;
using UnityEngine.UI;

public enum State
{
    nothing,
    X,
    O
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public State[,] stateGrid = new State[3, 3];
    public GameObject[] Buttons;

    private string PlayerSymbol = "X";
    private string AISymbol = "0";

    public bool AIWin = false;
    public bool isGameOver = false;
    public bool isGameTied = false;

    void Awake()
    {
        instance = this;
    }
    public void RestartGame()
    {
        SetGridDefault();
        SetButtonDefault();
        SetParametersDefault();
    }
    public void SetGridDefault()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
                stateGrid[x, y] = State.nothing;
        }
    }
    public void SetButtonDefault()
    {
        for (int x = 0; x < 9; x++)
            Buttons[x].GetComponentInChildren<Text>().text = "";
    }
    public void SetParametersDefault()
    {
        AIWin = false;
        isGameOver = false;
        isGameTied = false;
    }
    public void Click(Vector2Int position)
    {
        if (stateGrid[position.x, position.y] == State.nothing && !isGameOver)
        {
            Buttons[position.x * 3 + position.y].GetComponentInChildren<Text>().text = PlayerSymbol;
            stateGrid[position.x, position.y] = State.X;
            GameOver(stateGrid, false);
            if (!isGameOver)
            {
                AIPlay();
                GameOver(stateGrid, true);
            }
        }
    }
    public void SetGameOver(bool AImove)
    {
        AIWin = AImove;
        isGameOver = true;
    }
    public State[,] CopyGrid(State[,] gridToCopy, int X, int Y, bool AITurn)
    {
        State[,] gridToReturn = new State[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if(x == X && y == Y)
                    gridToReturn[x, y] = AITurn ? State.O : State.X;
                else
                    gridToReturn[x, y] = gridToCopy[x, y];
            }
        }
        return gridToReturn;
    }
    public void AIPlay()
    {
        int BestMoveX = 0;
        int BestMoveY = 0;
        int BestValue = -10;

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (stateGrid[x, y] == State.nothing)
                {
                    int Value = MiniMax(CopyGrid(stateGrid, x, y, true), false);

                    if (Value > BestValue)
                    {
                        BestMoveX = x;
                        BestMoveY = y;
                        BestValue = Value;
                    }
                }
            }
        }
        Buttons[BestMoveX * 3 + BestMoveY].GetComponentInChildren<Text>().text = AISymbol;
        stateGrid[BestMoveX, BestMoveY] = State.O;
    }
    public int MiniMax(State[,] grid, bool AIturn)
    {
        GameOver(grid, !AIturn);
        if (isGameOver)
        {
            if (AIWin)
            {
                SetParametersDefault();
                return 1;
            }
            else if (isGameTied)
            {
                SetParametersDefault();
                return 0;
            }
            else
            {
                SetParametersDefault();
                return -1;
            }
        }
        else if (AIturn)
        {
            int maxValue = -1;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (grid[x, y] == State.nothing)
                    {
                        int Value = MiniMax(CopyGrid(grid, x, y, AIturn), !AIturn);

                        if (Value > maxValue)
                            maxValue = Value;
                    }
                }
            }
            return maxValue;
        }
        else
        {
            int minValue = 1;

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    if (grid[x, y] == State.nothing)
                    {
                        int Value = MiniMax(CopyGrid(grid, x, y, AIturn), !AIturn);

                        if (Value < minValue)
                            minValue = Value;
                    }
                }
            }
            return minValue;
        }
    }
    public void GameOver(State[,] grid, bool AImove)
    {
        if (TestRows(grid))
            SetGameOver(AImove);

        else if (GridFull(grid))
        {
            isGameTied = true;
            isGameOver = true;
        }
    }
    public bool GridFull(State[,] grid)
    {
        foreach (State testState in grid)
        {
            if (testState == State.nothing)
                return false;
        }
        return true;
    }
    public bool TestRows(State[,] grid)
    {
        for (int x = 0; x < 3; x++)
        {
            if (grid[x, 0] == grid[x, 1] && grid[x, 1] == grid[x, 2] && grid[x, 0] != State.nothing)
                return true;
        }
        return TestColumns(grid);
    }
    public bool TestColumns(State[,] grid)
    {
        for (int x = 0; x < 3; x++)
        {
            if (grid[0, x] == grid[1, x] && grid[1, x] == grid[2, x] && grid[0, x] != State.nothing)
                return true;
        }
        return TestDiagonals(grid);
    }
    public bool TestDiagonals(State[,] grid)
    {
        if (grid[0, 0] == grid[1, 1] && grid[1, 1] == grid[2, 2] && grid[0, 0] != State.nothing)
            return true;
        if (grid[2, 0] == grid[1, 1] && grid[1, 1] == grid[0, 2] && grid[2, 0] != State.nothing)
            return true;
        return false;
    }
}