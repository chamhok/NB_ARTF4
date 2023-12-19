using Scripts.UI.Scene_UI;
using UnityEngine;

namespace Scripts.Scene
{
    public class SampleScene : BaseScene
    {
        protected override bool Initialized()
        {
            if (!base.Initialized()) return false;
            
            // TODO : 인트로 씬 실행시 Context 작성 
            Main.SetCurrentScene(this, Label.GameScene);
            LoadResource();
            KillZone.OnDeath += OpenDeathPopUp;
            return true;
        }

        private void LoadResource()
        {
            if (Main.Resource.LoadIntro)
            {
                // TODO : 로드가 되어있다면, 추가적인 초기화 필요
                Main.UI.SetSceneUI<Game_UI>();
            }
            else
            {
                string sceneType = CurrentScene.ToString();
                Main.Resource.AllLoadAsync<Object>($"{sceneType}", (key, count, totalCount) =>
                {
                    Debug.Log($"[{sceneType}] Load asset {key} ({count}/{totalCount})");
                    if (count < totalCount) return;
                    Main.Resource.LoadIntro = true;
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