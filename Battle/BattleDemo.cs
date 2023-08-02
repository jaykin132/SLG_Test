using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzTool;
using ET.Battle;
using Arbor;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BattleDemo : MonoBehaviour
{
	public static BattleDemo main = null;


    // Start is called before the first frame update
    void Start()
    {
		main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BattleDemo))]
public class BattleDemoEditor : Editor
{
	protected BattleDemo action => target as BattleDemo;

	public override void OnInspectorGUI()
	{
		EZM.DrawScriptSelector(action);

		if (GUILayout.Button("Battle Start"))
		{
			BattleSystem.main.Init();
		}
	}
}
#endif