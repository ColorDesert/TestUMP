using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class Hello : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Hello 脚本被加载了");
            //Debug.Log(UnityTools.FormatMoney(new decimal(12.88),2,"HK$"));
        }

        // // Update is called once per frame
        // void Update()
        // {
        //
        // }
        private void OnDestroy()
        {
            Debug.Log("第一个游戏场景被销毁");
        }

        public void OnTestClick()
        {
            Debug.Log("按钮被点击了！");
            SceneManager.LoadScene("Second");
        }
    }
}