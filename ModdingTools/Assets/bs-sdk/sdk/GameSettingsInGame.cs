using System;

using UnityEngine;
#if game
using ExitGames.Client.Photon;
#endif
[DefaultExecutionOrder(ExecutionOrder.Default)]
public class GameSettingsInGame:bs
{
    public new RoomSettings gameSettings = new RoomSettings (); //gamesettings are set onJoinRoom
    // public void Update()
    // {
    //     if(_Game && _Game.enabled && gameSettings != bs.gameSettings)
    //         LogScreen("game settings doesnt match");
    //     
    // }
    #if UNITY_EDITOR && game
    public override void OnInspectorGUI()
    {
        
//        if(!gameObject.CompareTag(Tag.editorOnly))
//            LabelError("tag should be editor only");

        var enumPopup = (GameType)UnityEditor.EditorGUILayout.EnumPopup(gameSettings.gameType);
        if (gameSettings.gameType != enumPopup)
        {
            gameSettings.gameType = enumPopup;
            gameSettings.map.SelectGameTypeChangedLocal(gameSettings);
            SetDirty();
        }
        base.OnInspectorGUI();
    }
    
    #endif
    
}