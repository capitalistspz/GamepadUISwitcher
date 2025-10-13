using System.IO;
using UnityEngine;
using UnityEngine.U2D;

namespace GamepadUISwitcher;

public static class FaceButtonSprites
{
    public class Buttons
    {
        public Sprite a;
        public Sprite b;
        public Sprite x;
        public Sprite y;
    }

    public class PlaystationButtons
    {
        public Sprite cross;
        public Sprite circle;
        public Sprite triangle;
        public Sprite square;
    }
    
    public static Buttons nintendo;
    public static PlaystationButtons playstation;
    public static Buttons xbox;
    
    public static void LoadSpritesFromUIManager()
    {
        var skins = UIManager.instance.uiButtonSkins;
        xbox.a = skins.a;
        xbox.b = skins.b;
        xbox.x = skins.x;
        xbox.y = skins.y;

        playstation.cross = skins.ps4x;
        playstation.circle = skins.ps4circle;
        playstation.square = skins.ps4square;
        playstation.triangle = skins.ps4triangle;

        nintendo.a = skins.switchHidA;
        nintendo.b = skins.switchHidB;
        nintendo.x = skins.switchHidX;
        nintendo.y = skins.switchHidY;
    }
    static FaceButtonSprites()
    {
        playstation = new PlaystationButtons();
        xbox = new Buttons();
        nintendo = new Buttons();
        //Reload();
        LoadSpritesFromUIManager();
    }
}