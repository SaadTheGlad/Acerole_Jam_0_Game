using Godot;
using Godot.Collections;
using System;

public partial class SceneManager : Node
{

	[Export] public LevelScene[] levels;

	Dictionary<string, PackedScene> levelsDic = new Dictionary<string, PackedScene>();
	Dictionary<int, PackedScene> levelsDicInt = new Dictionary<int, PackedScene>();

	public static int currentSceneIndex;

	public static SceneManager Instance { get; private set; }

	public override void _EnterTree()
	{
		currentSceneIndex = 0;

		if (Instance != null)
		{
			GD.Print("More than one ", Instance.Name);

		}
		else
		{
			Instance = this;

		}

		SetScenes();
	}

	void SetScenes()
	{
		foreach(LevelScene s in levels)
		{
			levelsDic.Add(s.Name, s.Level);
			int index = System.Array.IndexOf(levels, s);
			levelsDicInt.Add(index, s.Level);
		}
	}

	public PackedScene GetScene(string name)
	{
		if(levelsDic.ContainsKey(name))
		{
            return levelsDic[name];
        }
		else
		{
            GD.PushWarning("Scene you selected does not exist, please check the name");
            return null;
        }

    }

	public PackedScene GetScene(int index)
	{
		if(levelsDicInt.ContainsKey(index))
		{
            return levelsDicInt[index];
        }
        else
		{
            GD.PushWarning("Scene you selected does not exist, please check the index");
            return null;
        }

    }

	public void ChangeSceneTo(PackedScene scene)
	{
		if(scene == null)
		{
			GD.PushError("Did not load the scene because it was null");
		}
		else
		{
            GetTree().ChangeSceneToPacked(scene);
        }

    }
}



