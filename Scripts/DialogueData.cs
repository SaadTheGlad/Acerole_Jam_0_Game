using Godot;
using System;
using System.Text.RegularExpressions;

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
        SignalsManager.Instance.SendColour += SetColourName;

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
        SignalsManager.Instance.SendColour -= SetColourName;


    }

    public void MakeAbberated() => isAnomaly = true;
    public void MakeUnAbberated() => isAnomaly = false;

    public void SelectedCorrect() => selectedCorrectObject = true;
    public void SelectedInCorrect() => selectedCorrectObject = false;

    public void MakeMissing()
    {
        ResetBools();
        isMissing = true;
        GD.Print("isMissing");
    }
    public void MakeRotated()
    {
        ResetBools();
        isRotated = true;
        GD.Print("isRotated");

    }
    public void MakeDiscoloured()
    {
        ResetBools();
        isDiscoloured = true;
        GD.Print("isColoured");

    }
    public void MakeDuplicated()
    {
        ResetBools();
        isDuplicated = true;
        GD.Print("isDuplicated");

    }

    public void SetObjectName(string name)
    {
        var lowerCase = name.ToLower();
        var output = Regex.Replace(lowerCase, @"[\d-]", string.Empty);
        objectName = output;
    }

    public void SetColourName(string name)
    {
        colourName = name;
    }

    void ResetBools()
    {
        isMissing = false;
        isRotated = false;
        isDiscoloured = false;
        isDuplicated = false;
    }


}
