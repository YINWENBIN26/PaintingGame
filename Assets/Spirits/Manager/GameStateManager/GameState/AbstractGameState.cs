using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームの状態を管理するためのインターフェースです。
public interface IGameState
{
    // 現在のゲームの状態を取得するプロパティです。
    GameState gameState { get; }

    // 現在の状態で実行されるべき処理を定義するメソッドです。
    public abstract void Execute();

    // 現在の状態から抜ける際に実行される処理を定義するメソッドです。
    public abstract void Exit();

    // 特定の状態に入る際に実行される処理を定義するメソッドです。
    public abstract void Enter();
}
