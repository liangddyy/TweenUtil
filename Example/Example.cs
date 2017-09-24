using UnityEngine;
using System.Collections;
using Tween;

public class Example : MonoBehaviour
{
    public Transform cubeTrans;
    public Transform sphereTrans;
    public AnimationCurve Curve;
    public MyHandles pathTest;


    public void StopSphereTrans()
    {
        sphereTrans.TnStop();
    }

    public void PathLinerMoveEase()
    {
        sphereTrans.TnPathMove(null, pathTest.nodePoints, 5)
            .SetEase(Ease.InOutQuint)
            .SetLoopType(LoopType.PingPang);
    }

    public void PathLinerMove()
    {
        sphereTrans.TnPathMove(null, pathTest.nodePoints, 5).SetLoopType(LoopType.PingPang);
    }
    // 直线
    public void PathBreakMoveEase()
    {
        sphereTrans.TnPathMove(null, pathTest.nodePoints, 5, pathType: PathType.PathBreak)
            .SetEase(Ease.InOutQuint)
            .SetLoopType(LoopType.PingPang);
    }

    public void PathBreakMove()
    {
        sphereTrans.TnPathMove(null, pathTest.nodePoints, 4, pathType: PathType.PathBreak);
    }


    public void StopCubeTans()
    {
//        cubeTrans.TnReverse();
        cubeTrans.TnStop();
    }

    TweenScript test;

    public void PositionEaseTest()
    {
        cubeTrans.TnMove(null, cubeTrans.position - Vector3.right * 3, 2f).SetLoopType(LoopType.PingPang).SetEase(Ease.OutBounce);
//        cubeTrans.TnMove(null, Vector3.zero, 5f).SetLoopType(LoopType.PingPang).SetEase(Curve);
    }
    public void TestPosition()
    {
        if (test != null)
        {
            Debug.Log("use unRecycle script");
            test.Restart();
            return;
        }
        test = cubeTrans.TnLocalMove(null, cubeTrans.localPosition + Vector3.right * 3, 2f)
            .SetLoopType(LoopType.Once)
            .SetRecyclable(false);
    }

    public void TestRotate()
    {
        cubeTrans.TnRotate(Vector3.zero, new Vector3(0, 360, 0), 2).SetLoopType(LoopType.Loop);
    }

    public void TestScale()
    {
        cubeTrans.TnScale(Vector3.one, new Vector3(2, 2, 2), 2).SetLoopType(LoopType.PingPang);
    }
}