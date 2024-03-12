using Godot;
using System;

public partial class InfractionManager : Node
{
    public int numOfInfractions;

    public void IncreaseInfraction()
    {
        numOfInfractions++;
        if (numOfInfractions >= 2)
        {
            GD.Print("Lost");
            // do fired thing
        }

    }
}
