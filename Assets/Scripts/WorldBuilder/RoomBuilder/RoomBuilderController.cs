using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Assets.Scripts.Events;
using System.IO;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    public class RoomBuilderController : MonoBehaviour
    {
        [SerializeField] public GameObject buildingZone;
        [SerializeField] public GameObject tileSet;
        [SerializeField] public LineRenderer gridLinePrefab;
        [SerializeField] public GameObject gridParent;
        [SerializeField] public GameObject tileParent;
        [SerializeField] public GameObject mouseTriggerSquare;
        [SerializeField] public UnityEngine.Camera mainCamera;
        [SerializeField] public float cameraSpeed = 10;

        private RoomSave[] _roomSaves;
        private bool _isMouseDown = false;
        private int _lastTileFlipped = -1;
        private bool _hasUnsavedChanges;
        private EditMode _editMode;
        private RoomBeingBuilt _room;

        private bool _hasUndrawnAssets;


        #region UI elements
        private Button _btnCloseLoadDialog;
        private Button _btnCloseConfirmOverwriteDialog;
        private Button _btnCloseSaveDialog;
        private Button _btnCloseSaveSuccessDialog;
        private Button _btnConfirmOverwrite;
        private Button _btnLoadRoom;
        private Button _btnOpenLoadDialog;
        private Button _btnSaveRoom;
        private Button _btnOpenSaveDialog;
        private DropdownField _dropDownSelectRoomSave;
        private TextField _textFileName;
        private TextField _textRoomName;
        private VisualElement _veConfirmOverwriteDialog;
        private VisualElement _veMainUi;
        private VisualElement _veLoadDialog;
        private VisualElement _veSaveRoomDialog;
        private VisualElement _veSaveSuccessDialog;
        #endregion

        // Start is called before the first frame update
        void Start()
        {

            GameEvents.current.onBuilderSquareMouseDown += OnBuilderSquareMouseDown;
            GameEvents.current.onBuilderSquareMouseEnter += OnBuilderSquareMouseEnter;

            _roomSaves = RoomBuilderHelper.GetAllRoomSaves();

            InitializeUIComponents();
            _hasUndrawnAssets = false;
            _hasUnsavedChanges = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonUp("Fire1")) OnBuilderSquareMouseUp();

            if (_room != null && _hasUndrawnAssets)
            {
                _room.DrawRoom(_editMode);
                _hasUndrawnAssets = false;
            }
            MoveCamera();            
        }

        private void HideUIDialogs()
        {
            _veLoadDialog.visible = false;
            _veSaveRoomDialog.visible = false;
            _veConfirmOverwriteDialog.visible = false;
            _veSaveSuccessDialog.visible = false;
        }
        private void InitializeUIComponents()
        {
            // associate UI components to class vars
            var root = GetComponent<UIDocument>().rootVisualElement;
            _veMainUi = root.Q<VisualElement>("veMainUi");

            // saving 
            _btnCloseConfirmOverwriteDialog = root.Q<Button>("btnCloseConfirmOverwriteDialog");
            _btnCloseSaveDialog = root.Q<Button>("btnCloseSaveDialog");
            _btnCloseSaveSuccessDialog = root.Q<Button>("btnCloseSaveSuccessDialog");
            _btnConfirmOverwrite = root.Q<Button>("btnConfirmOverwrite");
            _btnOpenSaveDialog = root.Q<Button>("btnOpenSaveDialog");
            _btnSaveRoom = root.Q<Button>("btnSaveRoom");
            _textFileName = root.Q<TextField>("textFileName");
            _textRoomName = root.Q<TextField>("textRoomName");
            _veSaveRoomDialog = root.Q<VisualElement>("veSaveRoomDialog");
            _veConfirmOverwriteDialog = root.Q<VisualElement>("veConfirmOverwriteDialog");
            _veSaveSuccessDialog = root.Q<VisualElement>("veSaveSuccessDialog");

            // loading
            _btnCloseLoadDialog = root.Q<Button>("btnCloseLoadDialog");
            _btnLoadRoom = root.Q<Button>("btnLoadRoom");
            _btnOpenLoadDialog = root.Q<Button>("btnOpenLoadDialog");
            _dropDownSelectRoomSave = root.Q<DropdownField>("dropDownSelectRoomSave");
            _veLoadDialog = root.Q<VisualElement>("veLoadDialog");


            // set initial visual state for things
            HideUIDialogs();


            PopulateRoomSaveDropdownChoices();

            // wire in the events
            _btnLoadRoom.clicked += btnLoadRoom_Click;
            _btnOpenLoadDialog.clicked += btnOpenLoadDialog_Click;
            _btnCloseLoadDialog.clicked += btnCloseLoadDialog_Click;
            _btnSaveRoom.clicked += btnSaveRoom_Click;
            _btnOpenSaveDialog.clicked += btnOpenSaveDialog_Click;
            _btnCloseSaveDialog.clicked += btnCloseSaveDialog_Click;
            _btnCloseSaveSuccessDialog.clicked += btnCloseSaveSuccessDialog_Click;
        }
        private bool IsPanelOpen()
        {
            if(_veLoadDialog.visible == true) return true;
            if (_veSaveRoomDialog.visible == true) return true;
            if (_veConfirmOverwriteDialog.visible == true) return true;
            if (_veSaveSuccessDialog.visible == true) return true;
            return false;
        }
        private void MoveCamera()
        {
            float inputH = Input.GetAxisRaw("Horizontal");
            float inputV = Input.GetAxisRaw("Vertical");
            if (inputH != 0f)
            {
                mainCamera.transform.position = new Vector3(
                    mainCamera.transform.position.x + (inputH * cameraSpeed * Time.deltaTime),
                    mainCamera.transform.position.y,
                    mainCamera.transform.position.z);
            }
            if (inputV != 0f)
            {
                mainCamera.transform.position = new Vector3(
                    mainCamera.transform.position.x,
                    mainCamera.transform.position.y + (inputV * cameraSpeed * Time.deltaTime),
                    mainCamera.transform.position.z);
            }
        }
        private IEnumerator MoveCameraToRoomCenter()
        {
            const int startX = 0;
            const int startY = 0;

            float xWidth = (_room.roomWidth + 2) * _room.tileWidth / Globals.pixelsInAUnityMeter;
            float yHeight = (_room.roomHeight + 2) * _room.tileHeight / Globals.pixelsInAUnityMeter;
            float xCenter = startX + (xWidth / 2);
            float yCenter = (startY + (yHeight / 2) * -1);

            Vector3 camTarget = new Vector3(xCenter, yCenter, mainCamera.transform.position.z);

            while (Vector3.Distance(camTarget, mainCamera.transform.position) > 0.2f)
            {
                float step = cameraSpeed * 2 * Time.deltaTime;
                mainCamera.transform.position = Vector3.MoveTowards(
                    mainCamera.transform.position, camTarget, step);
                yield return 0;
            }
        }
        private void PopulateRoomSaveDropdownChoices()
        {
            _dropDownSelectRoomSave.choices = _roomSaves.OrderBy(x => x.roomName).Select(y => y.roomName).ToList();
        }
        private bool Save()
        {
            // todo: make the save path directory a config value
            const string pathDir = @"E:\Unity Projects\SpiderPocGit\Assets\Resources\RoomTemplates";
            string fileName = _textFileName.text;
            string json = RoomBuilderHelper.Serialize(_room, fileName);

            try
            {
                File.WriteAllText(Path.Combine(pathDir, fileName + ".txt"), json);

                _hasUnsavedChanges = false;
                return true;
            }
            catch (System.Exception)
            {
                // todo: create a fail message dialog
                return false;
            }
        }

        #region events
        private void btnCloseConfirmOverwriteDialog_Click()
        {
            _veConfirmOverwriteDialog.visible = false;
        }
        private void btnCloseSaveSuccessDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnCloseLoadDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnCloseSaveDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnConfirmOverwrite_Click()
        {
            _room.roomName = _textRoomName.text;
            if (Save())
            {
                HideUIDialogs();
                _veSaveSuccessDialog.visible = true;
            }
            else
            {
                // todo: create a fail message dialog
            }
        }
        private void btnLoadRoom_Click()
        {
            string nameSelected = _dropDownSelectRoomSave.text;
            
            if(nameSelected != string.Empty)
            {
                RoomSave save = _roomSaves.Where(x => x.roomName == nameSelected).FirstOrDefault();
                _room = new RoomBeingBuilt(save, gridLinePrefab, gridParent, tileSet, tileParent, mouseTriggerSquare);
                _room.DrawRoom(_editMode);
                _hasUnsavedChanges = false;
                StartCoroutine(MoveCameraToRoomCenter());
                _textRoomName.SetValueWithoutNotify(_room.roomName);
                _textFileName.SetValueWithoutNotify(_room.roomName);

            }
            _veLoadDialog.visible = false;
        }
        private void btnOpenLoadDialog_Click()
        {
            HideUIDialogs();
            _veLoadDialog.visible = true;
        }
        private void btnOpenSaveDialog_Click()
        {
            HideUIDialogs();
            _veSaveRoomDialog.visible = true;
        }
        private void btnSaveRoom_Click()
        {
            string roomName = _textRoomName.text;
            string fileName = _textFileName.text;
            RoomSave existingSave = _roomSaves.Where(x => x.roomName == roomName || x.fileName == fileName).FirstOrDefault();
            if (existingSave.roomName != null)
            {
                _veConfirmOverwriteDialog.visible = true;
            }
            else
            {
                _room.roomName = roomName;
                if (Save())
                {
                    HideUIDialogs();
                    _veSaveSuccessDialog.visible = true;
                }
                else
                {
                    // todo: create a fail message dialog
                }

            }
        }
        private void OnBuilderSquareMouseDown(Vector2 position)
        {
            _isMouseDown = true;
            if (!IsPanelOpen())
            {
                int tileNum = _room.GetTileNumbFromMousePosition(position);
                _room.ReverseTile(tileNum);
                _lastTileFlipped = tileNum;
                _hasUnsavedChanges = true;
                _hasUndrawnAssets = true;
            }
        }
        private void OnBuilderSquareMouseEnter(Vector2 position)
        {            
            if (!IsPanelOpen())
            {
                if (_isMouseDown)
                {
                    int tileNum = _room.GetTileNumbFromMousePosition(position);
                    if (tileNum != _lastTileFlipped)
                    {
                        _room.ReverseTile(tileNum);
                        _lastTileFlipped = tileNum;
                        _hasUnsavedChanges = true;
                        _hasUndrawnAssets = true;
                    }
                }                
            }
        }
        private void OnBuilderSquareMouseUp()
        {
            _isMouseDown = false;
            _lastTileFlipped = -1;
        }
        #endregion
    }
}
