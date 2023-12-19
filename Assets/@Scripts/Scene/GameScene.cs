using System;
using Scripts.UI.Scene_UI;
using UnityEngine;

namespace Scripts.Scene
{
    public class GameScene : BaseScene
    {

        protected override bool Initialized()
        {
            if (!base.Initialized()) return false;
            // TODO : 게임 씬 실행시 Context 작성
            CurrentScene = Label.GameScene;
            LoadResource();
            KillZone.OnDeath += OpenDeathPopUp;
            // TODO : -----------------------
            return true;
        }

        private void LoadResource()
        {
            if (Main.Resource.LoadGame)
            {
                // TODO : 로드가 되어있다면, 추가적인 초기화 필요
                Main.UI.SetSceneUI<Game_UI>();
            }
            else
            {
                string sceneType = CurrentScene.ToString();
                Main.Resource.AllLoadAsync<UnityEngine.Object>($"{sceneType}", (key, count, totalCount) =>
                {
                    Debug.Log($"[{sceneType}] Load asset {key} ({count}/{totalCount})");
                    if (count < totalCount) return;
                    Main.Resource.LoadGame = true;
                    // TODO : 추가적인 초기화 필요
                    Main.UI.SetSceneUI<Game_UI>();
                });
            }
        }



        private void OpenDeathPopUp()
        {
            Main.UI.OpenPopup<DiePanel_Popup>();
        }
    }
}