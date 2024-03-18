using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override IEnumerator LoadingRoutine()
    {
        Manager.Game.Test();
        Manager.Game.CreatePool();
        yield return null;
    }
}
