
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeLineController : MonoBehaviour
{
    public bool isTimeLine = false; // タイムラインがアクティブかどうかのフラグ。
    PlayableDirector activeDirector; // 現在アクティブなPlayableDirector。
    PlayableDirector currentDirector; // 現在のPlayableDirector。
    public GameObject go; // 操作するGameObject。

    private void Update()
    {
        // スペースキーが押されたとき、かつタイムラインがアクティブな場合、タイムラインを再開する。
        if (Input.GetKeyDown(KeyCode.Space) && isTimeLine)
        {
            ResumeTimeline();
        }
        // 現在のディレクターが設定されていて、再生中でない場合、GameObjectを破棄し、新しい日を開始する。
        if (currentDirector != null)
        {
            if (currentDirector.state != PlayState.Playing && !isTimeLine)
            {
                Destroy(go);
                StartCoroutine(SystemInstanceManager._Instance.timeManager.StartNewDay());
            }
        }
    }

    // タイムラインの再生を開始する。
    public void StartTimeLine(PlayableDirector whichOne)
    {
        currentDirector = whichOne;
    }

    // タイムラインを一時停止する。
    public void PauseTimeline(PlayableDirector whichOne)
    {
        activeDirector = whichOne;
        activeDirector.Pause();
        try
        {
            // ダイアログUIを非表示にする。
            UIManager._Instance.dialogueUI.Hide();
        }
        catch
        {
        }
    }

    // タイムラインの再生を再開する。
    public void ResumeTimeline()
    {
        isTimeLine = false;
        activeDirector.Resume();
        if (TImeLineInstanceManager._Instance.ReadingCanvas.gameObject.activeSelf)
        {
            // 読み込み用のキャンバスを非表示にする。
            TImeLineInstanceManager._Instance.ReadingCanvas.SetActive(false);
        }
    }
}