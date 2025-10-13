using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace GamepadUISwitcher;

public class FaceButtonSprites
{
    public (Sprite a, Sprite b, Sprite x, Sprite y) nintendo;
    public (Sprite cross, Sprite circle, Sprite triangle, Sprite square) playstation;
    public (Sprite a, Sprite b, Sprite x, Sprite y) xbox;

    public FaceButtonSprites()
    {
        foreach (var sprite in Resources.FindObjectsOfTypeAll<Sprite>())
        {
            switch (sprite.name)
            {
                case "controller__0012_Switch_Y":
                    nintendo.y = sprite;
                    break;
                case "controller__013_Switch_X":
                    nintendo.x = sprite;
                    break;
                case "controller__0014_Switch_B":
                    nintendo.b = sprite;
                    break;
                case "controller_0015_Switch_A":
                    nintendo.a = sprite;
                    break;
                case "PS4_button_skins_circle":
                    playstation.circle = sprite;
                    break;
                case "PS4_button_skins_square":
                    playstation.square = sprite;
                    break;
                case "PS4_button_skins_triangle":
                    playstation.triangle = sprite;
                    break;
                case "PS4_button_skins_x":
                    playstation.cross = sprite;
                    break;
                case "controller_button_skins_0020_B":
                    xbox.b = sprite;
                    break;
                case "controller_button_skins_0021_A":
                    xbox.a = sprite;
                    break;
                case "controller_button_skins_0022_Y":
                    xbox.y = sprite;
                    break;
                case "controller_button_skins_0023_X":
                    xbox.x = sprite;
                    break;
            }

            break;
        }
    }
}