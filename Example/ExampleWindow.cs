using UnityEngine;
using System.Collections;
using UnityEditor;

public class ExampleWindow : EditorWindow
{

    Object game;

    [MenuItem("Example/自定义窗口")]
    static void AddWindow()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 500, 500);
        ExampleWindow window = (ExampleWindow)EditorWindow.GetWindowWithRect(typeof(ExampleWindow), wr, false, "widow name");
        window.Show();
    }
    /// <summary>
    /// OnGUI is called for rendering and handling GUI events.
    /// This function can be called multiple times per frame (one call per event).
    /// </summary>
    void OnGUI()
    {
        game = EditorGUILayout.ObjectField("Test", game,typeof(Transform),true);
		if(GUILayout.Button("test")){
			if(game!=null){
				// ((Transform)game).
			}
		}
    }
}
