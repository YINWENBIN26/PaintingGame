using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetState : IGameState
{
    public GameState gameState => GameState.Setting;
    public void Enter()
    {
    }

    public void Execute()
    {
    }

    public void Exit()
    {

    }
}
