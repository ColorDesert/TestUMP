using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.Script
{
    public class Second : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("第二个脚本开始执行onStart");
        }

        // // Update is called once per frame
        // void Update()
        // {
        // }
        private void OnDestroy()
        {
            Debug.Log("第二个游戏场景被销毁");
        }

        public void OnBackAction()
        {
            Debug.Log("点击了返回 加载第一个场景，在Unity中加载一个新场景之前的场景会被销毁");
            SceneManager.LoadScene("First");
        }
        public void OnOneAction()
        {
            Debug.Log("第一个按钮点击事件影响");
        }

        public void OnTwoAction()
        {
            Debug.Log("第二个按钮点击事件影响");
        }

        public void OnThreeAction()
        {
            Debug.Log("第三个按钮点击事件影响");
        }
    }
}