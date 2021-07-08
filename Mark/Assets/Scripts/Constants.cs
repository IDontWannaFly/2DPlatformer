using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static class Layers{
        public const string DEFAULT = "Default";
        public const string TRANSPARENT_FX = "TransparentFX";
        public const string INGORE_RAYCAST = "Ignore Raycast";
        public const string GROUND = "Ground";
        public const string WATER = "Water";
        public const string UI = "UI";
        public const string PLAYER = "Player";
        public const string ENEMY = "Enemy";
        public const string BACKGROUND = "Background";
        public const string DETECTOR = "Detactor";
        public const string ENVIRONMENT = "Environment";
    }

    public static class Tags{
        public const string UNTAGGED = "Untagged";
        public const string RESPAWN = "Respawn";
        public const string FINISH = "Finish";
        public const string EDITOR_ONLY = "EditorOnly";
        public const string MAIN_CAMERA = "MainCamera";
        public const string PLAYER = "Player";
        public const string GAME_CONTROLLER = "GameController";
        public const string DOOR = "Door";
        public const string ATTACK_POINT = "AttackPoint";
    }
}
