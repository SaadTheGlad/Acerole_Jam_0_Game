using Godot;
using System;

public partial class NPCCreator : Node
{
    [Export] public Node2D Outward;
    [Export] public DialogueHolder npcHolder;

    [ExportGroup("Colours")]
    [Export] Color[] skinColours;
    [Export] Color[] shirtColours;
    [Export] Color[] hairColours;
    [Export] Color[] lipColours;
    [Export] Color[] eyeColours;

    [ExportCategory("Body Parts")]
    [ExportGroup("Male")]
    [Export] Texture2D[] m_heads;
    [Export] BodyType[] m_bodyTypes;
    [Export] Texture2D[] m_noses;
    [Export] Texture2D[] m_ears;
    [Export] Texture2D[] m_hairstyles;
    [Export] Texture2D[] mustaches;
    [Export] Texture2D[] m_lips;
    [Export] Eyes[] m_eyes;
    [ExportGroup("Female")]
    [Export] Texture2D[] f_heads;
    [Export] BodyType[] f_bodyTypes;
    [Export] Texture2D[] f_noses;
    [Export] Texture2D[] f_ears;
    [Export] Texture2D[] f_hairstyles;
    [Export] Texture2D[] f_lips;
    [Export] Eyes[] f_eyes;

    [ExportCategory("Names")]
    [ExportGroup("First Names")]
    [Export] public string[] maleFirstNames;
    [Export] public string[] femaleFirstNames;
    [ExportGroup("Last Names")]
    [Export] public string[] maleLastNames;
    [Export] public string[] femaleLastNames;

    RandomNumberGenerator random = new RandomNumberGenerator();

    public bool manOrWoman;

    public override void _EnterTree()
    {

        SignalsManager.Instance.NPCHasPassed += CreateNPC;
        SignalsManager.Instance.NPCHasPassed += SelectRandomName;
    }

    public override void _ExitTree()
    {
        SignalsManager.Instance.NPCHasPassed -= CreateNPC;
        SignalsManager.Instance.NPCHasPassed -= SelectRandomName;

    }

    public override void _Ready()
    {
        //CreateNPC();
        SelectRandomName();
    }

    void CreateNPC()
    {
        float randomNumber = random.RandiRange(0, 100);
        if(randomNumber < 50f)
        {
            //male
            manOrWoman = true;

            Color skinColour = GetRandomColour(skinColours);
            Color hairColour = GetRandomColour(hairColours);
            BodyType bodyType = GetRandomBodyType(m_bodyTypes);

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
                            sprite.Texture = GetRandomTexture(m_heads);
                            break;

                        case "Nose":
                            sprite.Texture = GetRandomTexture(m_noses);
                            break;

                        case "Ears":
                            sprite.Texture = GetRandomTexture(m_ears);
                            break;
                        case "Hair":
                            sprite.Texture = GetRandomTexture(m_hairstyles);
                            sprite.SelfModulate = hairColour;
                            break;
                        case "Lips":
                            sprite.Texture = GetRandomTexture(m_lips);
                            sprite.SelfModulate = GetRandomColour(lipColours);
                            break;
                        case "TShirt":
                            sprite.SelfModulate = GetRandomColour(shirtColours);
                            sprite.Texture = bodyType.tshirt;
                            break;
                        case "Body":
                            sprite.Texture = bodyType.body;
                            break;
                        case "Mustache":
                            sprite.Texture = GetRandomTexture(mustaches);
                            sprite.SelfModulate = hairColour;
                            break;
                    }



                }
                else if (node is Node2D)
                {
                    //We're in the eye
                    Eyes randomEye = GetRandomEye(m_eyes);

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

                            var eyebrowNode = secondaryNode.GetChild(0);
                            Sprite2D eyebrowsSprite = (Sprite2D)eyebrowNode;
                            eyebrowsSprite.Texture = randomEye.eyebrows;
                            eyebrowsSprite.SelfModulate = hairColour;

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
        else if(randomNumber >= 50f)
        {
            manOrWoman = false;

            Color skinColour = GetRandomColour(skinColours);
            Color hairColour = GetRandomColour(hairColours);
            BodyType bodyType = GetRandomBodyType(f_bodyTypes);

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
                            sprite.Texture = GetRandomTexture(f_heads);
                            break;

                        case "Nose":
                            sprite.Texture = GetRandomTexture(f_noses);
                            break;

                        case "Ears":
                            sprite.Texture = GetRandomTexture(f_ears);
                            break;
                        case "Hair":
                            sprite.Texture = GetRandomTexture(f_hairstyles);
                            sprite.SelfModulate = hairColour;
                            break;
                        case "Lips":
                            sprite.Texture = GetRandomTexture(f_lips);
                            sprite.SelfModulate = GetRandomColour(lipColours);
                            break;
                        case "TShirt":
                            sprite.SelfModulate = GetRandomColour(shirtColours);
                            sprite.Texture = bodyType.tshirt;
                            break;
                        case "Body":
                            sprite.Texture = bodyType.body;
                            break;
                        case "Mustache":
                            sprite.Texture = null;
                            break;
                    }



                }
                else if (node is Node2D)
                {
                    //We're in the eye
                    Eyes randomEye = GetRandomEye(f_eyes);

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

                            var eyebrowNode = secondaryNode.GetChild(0);
                            Sprite2D eyebrowsSprite = (Sprite2D)eyebrowNode;
                            eyebrowsSprite.Texture = randomEye.eyebrows;
                            eyebrowsSprite.SelfModulate = hairColour;

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


    }

    void SelectRandomName()
    {
        if(manOrWoman)
        {
            int randomFirstIndex = random.RandiRange(0, maleFirstNames.Length - 1);
            int randomLastIndex = random.RandiRange(0, maleLastNames.Length - 1);
            npcHolder.Name = maleFirstNames[randomFirstIndex] + " " + maleLastNames[randomLastIndex];
            GD.Print("Man");
        }
        else
        {
            int randomFirstIndex = random.RandiRange(0, femaleFirstNames.Length - 1);
            int randomLastIndex = random.RandiRange(0, femaleLastNames.Length - 1);
            npcHolder.Name = femaleFirstNames[randomFirstIndex] + " " + femaleLastNames[randomLastIndex];
            GD.Print("Female");

        }


        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SetNPCName, npcHolder.Name);
        SignalsManager.Instance.EmitSignal(SignalsManager.SignalName.SetSFXName, npcHolder.sfxName);
    }

    Color GetRandomColour(Color[] colorArray)
    {
        int randomIndex = random.RandiRange(0, colorArray.Length - 1);
        return colorArray[randomIndex];
    }

    Texture2D GetRandomTexture(Texture2D[] textureArray)
    {
        int randomIndex = random.RandiRange(0, textureArray.Length - 1);
        
        return textureArray[randomIndex];
    }

    Eyes GetRandomEye(Eyes[] eyesArray)
    {
        int randomIndex = random.RandiRange(0, eyesArray.Length - 1);
        //GD.Print("Called Eye with index: " + randomIndex);
        return eyesArray[randomIndex];
    }

    BodyType GetRandomBodyType(BodyType[] bodyArray)
    {
        int randomIndex = random.RandiRange(0, bodyArray.Length - 1);
        return bodyArray[randomIndex];

    }
}