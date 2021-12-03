using Assets.Scripts.CharacterControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Combat
{
    public class HealthBar: MonoBehaviour
    {
        [SerializeField] public float width = 2;
        [SerializeField] public GameObject character;
        private GameObject _leftBorder;
        private GameObject _rightBorder;
        private GameObject _topBorder;
        private GameObject _bottomBorder;
        private GameObject _fill;
        private float _currentHealthPercent;
        private ControllableCharacter _character;
        private Color32 _red;
        private Color32 _amber;
        private Color32 _green;

        private void Start()
        {
            _leftBorder = transform.Find("LeftBorder").gameObject;
            _rightBorder = transform.Find("RightBorder").gameObject;
            _topBorder = transform.Find("TopBorder").gameObject;
            _bottomBorder = transform.Find("BottomBorder").gameObject;
            _fill = transform.Find("Fill").gameObject;
            _character = character.GetComponent<ControllableCharacter>();

            _red = new Color32(255, 33, 26, 255);
            _amber = new Color32(255, 226, 26, 255);
            _green = new Color32(0, 178, 26, 255);

            DrawBorder();
        }
        private void Update()
        {
            _currentHealthPercent = _character.currentHP / _character.maxHP;
            UpdateFill();
            UpdateColors();
        }
        
        private void DrawBorder()
        {
            _leftBorder.transform.localPosition = new Vector3(
               0 - (width / 2), _leftBorder.transform.localPosition.y, _leftBorder.transform.localPosition.z);
            _rightBorder.transform.localPosition = new Vector3(
               (width / 2), _rightBorder.transform.localPosition.y, _rightBorder.transform.localPosition.z);
            _bottomBorder.transform.localScale = new Vector3(
                width, _bottomBorder.transform.localScale.y, _bottomBorder.transform.localScale.z);
            _topBorder.transform.localScale = new Vector3(
                width, _topBorder.transform.localScale.y, _topBorder.transform.localScale.z);
        }
        private void UpdateColors()
        {
            Color32 assignment = _green;
            if (_currentHealthPercent < .25) assignment = _red;
            else if (_currentHealthPercent < .5) assignment = _amber;

            _leftBorder.GetComponent<SpriteRenderer>().color = assignment;
            _rightBorder.GetComponent<SpriteRenderer>().color = assignment;
            _topBorder.GetComponent<SpriteRenderer>().color = assignment;
            _bottomBorder.GetComponent<SpriteRenderer>().color = assignment;
            _fill.GetComponent<SpriteRenderer>().color = assignment;
        }
        private void UpdateFill()
        {
            float fillWidth = width * _currentHealthPercent;
            float left = 0 - (width / 2);
            float right = left + fillWidth;
            float x = (right + left) / 2;
            
            _fill.transform.localPosition = new Vector3(x, _fill.transform.localPosition.y, _fill.transform.localPosition.z);
            _fill.transform.localScale = new Vector3(fillWidth, _fill.transform.localScale.y, _fill.transform.localScale.z);
        }
    }
}
