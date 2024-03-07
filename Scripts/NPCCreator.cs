using Godot;
using System;

public partial class NPCCreator : Node
{
    [Export] public Node2D Outward;

    [ExportGroup("Colours")]
    [Export] Color[] skinColours;
    [Export] Color[] shirtColours;
    [Export] Color[] hairColours;
    [Export] Color[] lipColours;
    [Export] Color[] eyeColours;

    [ExportGroup("Body Parts")]

    [Export] Texture2D[] heads;
    [Export] Texture2D[] noses;
    [Export] Texture2D[] ears;
    [Export] Texture2D[] hairstyles;
    [Export] Texture2D[] lips;
    [Export] Eyes[] eyes;

    RandomNumberGenerator random = new RandomNumberGenerator();

    public override void _EnterTree()
    {
        SignalsManager.Instance.NPCHasPassed += ApplyBodyAndColours;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.NPCHasPassed -= ApplyBodyAndColours;

    }

    public override void _Ready()
    {
        ApplyBodyAndColours();
    }

    void ApplyBodyAndColours()
    {
        Color skinColour = GetRandomColour(skinColours);

        foreach (var node in Outward.GetChildren())
        {

            if (node is Sprite2D sprite)
            {
                if (sprite.IsInGroup("Skin"))
                {
                    sprite.SelfModulate = skinColour;
                }

                switch (sprite.Name)
                {
                    case "Head":
                        sprite.Texture = GetRandomTexture(heads);
                        break;

                    case "Nose":
                        sprite.Texture = GetRandomTexture(noses);
                        break;

                    case "Ears":
                        sprite.Texture = GetRandomTexture(ears);
                        break;
                    case "Hair":
                        sprite.Texture = GetRandomTexture(hairstyles);
                        sprite.SelfModulate = GetRandomColour(hairColours);
                        break;
                    case "Lips":
                        sprite.Texture = GetRandomTexture(lips);
                        sprite.SelfModulate = GetRandomColour(lipColours);
                        break;
                    case "TShirt":
                        sprite.SelfModulate = GetRandomColour(shirtColours);
                        break;
                }



            }
            else if(node is Node2D)
            {
                //We're in the eye
                Eyes randomEye = GetRandomEye(eyes);

                foreach (var secondaryNode in node.GetChildren())
                {
                    if (secondaryNode.Name == "Iris")
                    {
                        foreach (Sprite2D spriteSecondary in secondaryNode.GetChildren())
                        {
                            if (spriteSecondary.Name == "Colour")
                            {
                                spriteSecondary.Texture = randomEye.irisColour;
                                spriteSecondary.SelfModulate = GetRandomColour(eyeColours);
                            }

                            if (spriteSecondary.Name == "Ink")
                            {
                                spriteSecondary.Texture = randomEye.irisInk;
                            }
                        }
                    }

                    if (secondaryNode.Name == "Sclera")
                    {
                        Sprite2D scleraSprite = (Sprite2D)secondaryNode;
                        scleraSprite.Texture = randomEye.sclera;
                    }

                    if (secondaryNode.Name == "Highlights")
                    {
                        Sprite2D highlightSprite = (Sprite2D)secondaryNode;
                        highlightSprite.Texture = randomEye.highlights;
                    }
                }

            }


        }
    }

    Color GetRandomColour(Color[] colorArray)
    {
        int randomIndex = random.RandiRange(0, colorArray.Length - 1);
        return colorArray[randomIndex];
    }

    Texture2D GetRandomTexture(Texture2D[] textureArray)
    {
        int randomIndex = random.RandiRange(0, textureArray.Length - 1);
        //GD.Print("Called Texture with index: " + randomIndex);
        return textureArray[randomIndex];
    }

    Eyes GetRandomEye(Eyes[] eyesArray)
    {
        int randomIndex = random.RandiRange(0, eyesArray.Length - 1);
        //GD.Print("Called Eye with index: " + randomIndex);
        return eyesArray[randomIndex];
    }
}