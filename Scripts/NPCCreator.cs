using Godot;
using System;

public partial class NPCCreator : Node
{
    [Export] public Color[] skinColours;
    [Export] public Color[] shirtColours;
    [Export] public Color[] hairColours;
    [Export] public Color[] lipColours;
    [Export] public Color[] eyeColours;


    public Node2D NPC;

    public override void _EnterTree()
    {
        ApplyColours();
    }

    void ApplyColours()
    {
        foreach (var node in NPC.GetChildren())
        {
            if(node is Sprite2D sprite)
            {
                if (sprite.IsInGroup("Skin"))
                {
                    sprite.SelfModulate = GetRandomColour(skinColours);
                }

                if (sprite.Name == "TShirt")
                {
                    sprite.SelfModulate = GetRandomColour(shirtColours);
                }

                if (sprite.IsInGroup("Hair"))
                {
                    sprite.SelfModulate = GetRandomColour(hairColours);
                }

                if (sprite.Name == "Lips")
                {
                    sprite.SelfModulate = GetRandomColour(lipColours);
                }
            }
            else
            {
                //We're in the eye
                foreach(var secondaryNode in node.GetChildren())
                {
                    if(secondaryNode.Name == "Iris")
                    {
                        foreach(Sprite2D spriteSecondary in secondaryNode.GetChildren())
                        {
                            if(spriteSecondary.Name == "Colour")
                            {
                                spriteSecondary.SelfModulate = GetRandomColour(eyeColours);
                            }
                        }
                    }
                }
            }


        }
    }

    Color GetRandomColour(Color[] colorArray)
    {
        int randomIndex = (int)(GD.Randi() % colorArray.Length);
        Color randomColour = shirtColours[randomIndex];
        return randomColour;
    }
}
