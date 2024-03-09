using Godot;
using System;

public partial class DialogueData : Node
{
    public static DialogueData Instance { get; private set; }

    public string objectName = "PLACEHOLDER NAME";
    public string colourName = "PLACEHOLDER COLOUR";

    public bool isMissing;
    public bool isRotated;
    public bool isDiscoloured;
    public bool isDuplicated;
    public bool isAnomaly;
    public bool selectedCorrectObject;

    public override void _EnterTree()
    {

        SignalsManager.Instance.MakeMissing += MakeMissing;
        SignalsManager.Instance.MakeRotated += MakeRotated;
        SignalsManager.Instance.MakeDiscoloured += MakeDiscoloured;
        SignalsManager.Instance.MakeDuplicated += MakeDuplicated;
        SignalsManager.Instance.SetName += SetObjectName;
        SignalsManager.Instance.EnableAbberation += MakeAbberated;
        SignalsManager.Instance.DisableAbberation += MakeUnAbberated;
        SignalsManager.Instance.SelectedCorrect += SelectedCorrect;
        SignalsManager.Instance.SelectedIncorrect += SelectedInCorrect;

        if (IsInstanceValid(Instance))
        {
            GD.Print("More than one ", Instance.Name);

        }
        else
        {
            Instance = this;
        }
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.MakeMissing -= MakeMissing;
        SignalsManager.Instance.MakeRotated -= MakeRotated;
        SignalsManager.Instance.MakeDiscoloured -= MakeDiscoloured;
        SignalsManager.Instance.MakeDuplicated -= MakeDuplicated;
        SignalsManager.Instance.SetName -= SetObjectName;
        SignalsManager.Instance.EnableAbberation -= MakeAbberated;
        SignalsManager.Instance.DisableAbberation -= MakeUnAbberated;
        SignalsManager.Instance.SelectedCorrect -= SelectedCorrect;
        SignalsManager.Instance.SelectedIncorrect -= SelectedInCorrect;

    }

    public void MakeAbberated() => isAnomaly = true;
    public void MakeUnAbberated() => isAnomaly = false;

    public void SelectedCorrect() => selectedCorrectObject = true;
    public void SelectedInCorrect() => selectedCorrectObject = false;

    public void MakeMissing()
    {
        ResetBools();
        isMissing = true;
    }
    public void MakeRotated()
    {
        ResetBools();
        isRotated = true;
    }
    public void MakeDiscoloured()
    {
        ResetBools();
        isDiscoloured = true;
    }
    public void MakeDuplicated()
    {
        ResetBools();
        isDuplicated = true;
    }

    public void SetObjectName(string name)
    {
        objectName = name;
    }

    void ResetBools()
    {
        isMissing = false;
        isRotated = false;
        isDiscoloured = false;
        isDuplicated = false;
    }


}
