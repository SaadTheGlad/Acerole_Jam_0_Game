using Godot;
using System;

public partial class NPCCreator : Node
{
    [Export] public Node2D NPC;

    [ExportGroup("Colours")]
    [Export] Color[] skinColours;
    [Export] Color[] shirtColours;
    [Export] Color[] hairColours;
    [Export] Color[] lipColours;
    [Export] Color[] eyeColours;

    [ExportGroup("Body Parts")]
    [Export] Texture2D[] heads;
    [Export] Texture2D[] lips;
    [Export] Texture2D[] noses;
    [Export] Texture2D[] hairstyles;
    [Export] Texture2D[] ears;
    [Export] Eyes[] eyes;
        



    public override void _EnterTree()
    {
        ApplyBodyAndColours();
    }

    void ApplyBodyAndColours()
    {
        Color skinColour = GetRandomColour(skinColours);

        foreach (var node in NPC.GetChildren())
        {
            if(node is Sprite2D sprite)
            {


                if (sprite.IsInGroup("Skin"))
                {
                    sprite.SelfModulate = skinColour;
                }

                if (sprite.Name == "TShirt")
                {
                    sprite.SelfModulate = GetRandomColour(shirtColours);
                }

                if(sprite.Name == "Nose")
                {
                    sprite.Texture = GetRandomTexture(noses);
                }

                if(sprite.Name == "Head")
                {
                    sprite.Texture = GetRandomTexture(heads);
                }

                if(sprite.Name == "Ears")
                {
                    sprite.Texture = GetRandomTexture(ears);
                }

                if (sprite.IsInGroup("Hair"))
                {
                    sprite.Texture = GetRandomTexture(hairstyles);
                    sprite.SelfModulate = GetRandomColour(hairColours);
                }

                if (sprite.Name == "Lips")
                {
                    sprite.Texture = GetRandomTexture(lips);
                    sprite.SelfModulate = GetRandomColour(lipColours);
                }
            }
            else
            {
                //We're in the eye
                Eyes randomEye = GetRandomEye(eyes);

                foreach(var secondaryNode in node.GetChildren())
                {
                    if(secondaryNode.Name == "Iris")
                    {
                        foreach(Sprite2D spriteSecondary in secondaryNode.GetChildren())
                        {
                            if(spriteSecondary.Name == "Colour")
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

                    if(secondaryNode.Name == "Sclera")
                    {
                        Sprite2D scleraSprite = (Sprite2D)secondaryNode;
                        scleraSprite.Texture = randomEye.sclera;
                    }

                    if(secondaryNode.Name == "Highlights")
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
        int randomIndex = (int)(GD.Randi() % (colorArray.Length));
        Color randomColour = colorArray[randomIndex];
        return randomColour;
    }

    Texture2D GetRandomTexture(Texture2D[] textureArray)
    {
        int randomIndex = (int)(GD.Randi() % (textureArray.Length));
        Texture2D randomTexture = textureArray[randomIndex];
        return randomTexture;
    }

    Eyes GetRandomEye(Eyes[] eyesArray)
    {
        int randomIndex = (int)(GD.Randi() % (eyesArray.Length));
        Eyes randomEye = eyesArray[randomIndex];
        return randomEye;
    }
}
