using UnityEngine;
using System.Collections;
using Tween;

public class ExampleUGUI : MonoBehaviour
{
    public RectTransform ImgObj;
    public RectTransform TxtObj;

	public void StopUGUIAnim(){
        ImgObj.TnStop();
        TxtObj.TnStop();
    }
	public void UguiPosition(){
        TxtObj.TnUguiMove(TxtObj.anchoredPosition3D + Vector3.right * 100, 2f).SetLoopType(LoopType.PingPang);
    }
	public void UguiRotate(){
        TxtObj.TnUguiRotate(new Vector3(0, 90, 0),0.5f).SetLoopType(LoopType.PingPang);
    }
	public void UguiScale(){
        TxtObj.TnUguiScale(new Vector3(2, 2, 2), 1).SetLoopType(LoopType.PingPang);
    }
    public void SizeTest () {
        ImgObj.TnUguiSize(new Vector2(0, 0), new Vector2(100, 100)).SetLoopType(LoopType.PingPang);
    }
	
	public void ColorTest () {
        ImgObj.TnUguiColor(Color.white, Color.blue, 2f).SetLoopType(LoopType.PingPang);
    }

	public void AlphaTest()
	{
	    ImgObj.TnUguiAlpha(1, 0, 2).SetLoopType(LoopType.PingPang);
	}
}
