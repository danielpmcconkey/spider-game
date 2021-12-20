using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Events
{
    public class UIController : MonoBehaviour
    {
        public Button btnStart;
        public Button btnRoomBuilder;

        private void Start()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            btnStart = root.Q<Button>("BtnStart");
            btnRoomBuilder = root.Q<Button>("BtnRoomBuilder");

            btnStart.clicked += btnStart_Click;
            btnRoomBuilder.clicked += btnRoomBuilder_Click;
        }
        private void btnStart_Click()
        {
            SceneManager.LoadScene("GamePlay");
        }
        private void btnRoomBuilder_Click()
        {
            SceneManager.LoadScene("RoomBuilder");
        }
        
    }
}
