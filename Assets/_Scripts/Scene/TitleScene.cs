using System.Collections;
using UnityEngine;

public class TitleScene : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }

    public void StartGame()
    {
        Manager.Scene.LoadScene("GameScene");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
