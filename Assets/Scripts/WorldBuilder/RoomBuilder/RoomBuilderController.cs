using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Assets.Scripts.Events;
using System.IO;
using System;

namespace Assets.Scripts.WorldBuilder.RoomBuilder
{
    public enum ConfirmAction { NONE, OVERWRITE_SAVE, DISCARD_LOAD, DISCARD_NEW }
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
        private ConfirmAction _confirmAction = ConfirmAction.NONE;
        private string _confirmWarning = string.Empty;


        #region UI elements
        private Button _btnCloseLoadDialog;
        private Button _btnCloseConfirmDialog;
        private Button _btnCloseNewRoomDialog;
        private Button _btnCloseSaveDialog;
        private Button _btnCloseSaveSuccessDialog;
        private Button _btnConfirm;
        private Button _btnLoadRoom;
        private Button _btnNewRoom;
        private Button _btnOpenLoadDialog;
        private Button _btnOpenNewDialog;
        private Button _btnOpenSaveDialog;
        private Button _btnSaveRoom;
        private Button _btnTileEditMode;
        private Button _btnDoorEditMode;
        private DropdownField _dropDownSelectRoomSave;
        private Label _lblConfirmWarning;
        private TextField _textFileName;
        private TextField _textRoomName;
        private TextField _textRoomWidth;
        private TextField _textRoomHeight;
        private VisualElement _veConfirmDialog;
        private VisualElement _veMainUi;
        private VisualElement _veLoadDialog;
        private VisualElement _veNewRoomDialog;
        private VisualElement _veSaveRoomDialog;
        private VisualElement _veSaveSuccessDialog;
        private VisualElement _veHighlightTileEditMode;
        private VisualElement _veHighlightDoorEditMode;
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
            _veConfirmDialog.visible = false;
            _veSaveSuccessDialog.visible = false;
            _veNewRoomDialog.visible = false;
        }
        private void InitializeUIComponents()
        {
            // associate UI components to class vars
            var root = GetComponent<UIDocument>().rootVisualElement;
            _veMainUi = root.Q<VisualElement>("veMainUi");

            // editor mode
            _btnTileEditMode = root.Q<Button>("btnTileEditMode");
            _btnDoorEditMode = root.Q<Button>("btnDoorEditMode");
            _veHighlightTileEditMode = root.Q<VisualElement>("veHighlightTileEditMode");
            _veHighlightDoorEditMode = root.Q<VisualElement>("veHighlightDoorEditMode");

            // saving 
            _btnCloseSaveDialog = root.Q<Button>("btnCloseSaveDialog");
            _btnCloseSaveSuccessDialog = root.Q<Button>("btnCloseSaveSuccessDialog");
            _btnOpenSaveDialog = root.Q<Button>("btnOpenSaveDialog");
            _btnSaveRoom = root.Q<Button>("btnSaveRoom");
            _textFileName = root.Q<TextField>("textFileName");
            _textRoomName = root.Q<TextField>("textRoomName");
            _veSaveRoomDialog = root.Q<VisualElement>("veSaveRoomDialog");
            _veSaveSuccessDialog = root.Q<VisualElement>("veSaveSuccessDialog");

            // loading
            _btnCloseLoadDialog = root.Q<Button>("btnCloseLoadDialog");
            _btnLoadRoom = root.Q<Button>("btnLoadRoom");
            _btnOpenLoadDialog = root.Q<Button>("btnOpenLoadDialog");
            _dropDownSelectRoomSave = root.Q<DropdownField>("dropDownSelectRoomSave");
            _veLoadDialog = root.Q<VisualElement>("veLoadDialog");

            // new room
            _btnOpenNewDialog = root.Q<Button>("btnOpenNewDialog");
            _btnCloseNewRoomDialog = root.Q<Button>("btnCloseNewRoomDialog");
            _btnNewRoom = root.Q<Button>("btnNewRoom");
            _textRoomWidth = root.Q<TextField>("textRoomWidth");
            _textRoomHeight = root.Q<TextField>("textRoomHeight");
            _veNewRoomDialog = root.Q<VisualElement>("veNewRoomDialog");

            // confirm
            _btnCloseConfirmDialog = root.Q<Button>("btnCloseConfirmDialog");
            _btnConfirm = root.Q<Button>("btnConfirm");
            _lblConfirmWarning = root.Q<Label>("lblConfirmWarning");
            _veConfirmDialog = root.Q<VisualElement>("veConfirmDialog");

            // set initial visual state for things
            HideUIDialogs();
            _veHighlightTileEditMode.visible = true;
            _veHighlightDoorEditMode.visible = false;


            PopulateRoomSaveDropdownChoices();

            // wire in the events
            _btnCloseConfirmDialog.clicked += btnCloseConfirmDialog_Click;
            _btnCloseLoadDialog.clicked += btnCloseLoadDialog_Click;
            _btnCloseNewRoomDialog.clicked += btnCloseNewRoomDialog_Click;
            _btnCloseSaveDialog.clicked += btnCloseSaveDialog_Click;
            _btnCloseSaveSuccessDialog.clicked += btnCloseSaveSuccessDialog_Click;
            _btnConfirm.clicked += btnConfirm_Click;
            _btnLoadRoom.clicked += btnLoadRoom_Click;
            _btnOpenLoadDialog.clicked += btnOpenLoadDialog_Click;
            _btnOpenNewDialog.clicked += btnOpenNewDialog_Click;
            _btnOpenSaveDialog.clicked += btnOpenSaveDialog_Click;
            _btnSaveRoom.clicked += btnSaveRoom_Click;
            _btnTileEditMode.clicked += btnTileEditMode_Click;
            _btnDoorEditMode.clicked += btnDoorEditMode_Click;
            _btnNewRoom.clicked += btnNewRoom_Click;
            
            
        }

        private bool IsPanelOpen()
        {
            if(_veLoadDialog.visible) return true;
            if (_veSaveRoomDialog.visible) return true;
            if (_veConfirmDialog.visible) return true;
            if (_veSaveSuccessDialog.visible) return true;
            if (_veNewRoomDialog.visible) return true;
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
        private void btnCloseConfirmDialog_Click()
        {
            _veConfirmDialog.visible = false;
        }
        private void btnCloseSaveSuccessDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnCloseLoadDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnCloseNewRoomDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnCloseSaveDialog_Click()
        {
            HideUIDialogs();
        }
        private void btnConfirm_Click()
        {
            if (_confirmAction == ConfirmAction.OVERWRITE_SAVE)
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
            else if (_confirmAction == ConfirmAction.DISCARD_LOAD)
            {
                HideUIDialogs();
                _veLoadDialog.visible = true;
            }
            else if (_confirmAction == ConfirmAction.DISCARD_NEW)
            {
                HideUIDialogs();
                _veNewRoomDialog.visible = true;
            }
            _confirmAction = ConfirmAction.NONE;
        }
        private void btnDoorEditMode_Click()
        {
            _editMode = EditMode.DOOR;
            _veHighlightTileEditMode.visible = false;
            _veHighlightDoorEditMode.visible = true;
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
        private void btnNewRoom_Click()
        {
            int width = int.Parse(_textRoomWidth.text);
            int height = int.Parse(_textRoomHeight.text);

            RoomSave emptySave = new RoomSave()
            {
                tileWidth = (int)(Globals.tileWidthInUnityMeters * Globals.pixelsInAUnityMeter),
                tileHeight = (int)(Globals.tileHeightInUnityMeters * Globals.pixelsInAUnityMeter),
                roomName = "New room",
                fileName = "New room",
                roomWidth = width,
                roomHeight = height,
            };

            _room = new RoomBeingBuilt(emptySave, gridLinePrefab, gridParent, tileSet, tileParent, mouseTriggerSquare);
            _room.SetRoomDimensions(width, height);
            _room.DrawRoom(_editMode);
            _hasUnsavedChanges = true;
            HideUIDialogs();
            StartCoroutine(MoveCameraToRoomCenter());
        }
        private void btnOpenLoadDialog_Click()
        {
            if (_hasUnsavedChanges)
            {
                _veConfirmDialog.visible = true;
                _lblConfirmWarning.text = "Warning! There are unsaved changes. Do you want to discard them?";
                _confirmAction = ConfirmAction.DISCARD_LOAD;
            }
            else
            {
                HideUIDialogs();
                _veLoadDialog.visible = true;
            }
        }
        private void btnOpenNewDialog_Click()
        {
            if (_hasUnsavedChanges)
            {
                _veConfirmDialog.visible = true;
                _lblConfirmWarning.text = "Warning! There are unsaved changes. Do you want to discard them?";
                _confirmAction = ConfirmAction.DISCARD_NEW;
            }
            else
            {
                HideUIDialogs();
                _veNewRoomDialog.visible = true;
            }
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
                _veConfirmDialog.visible = true;
                _lblConfirmWarning.text = "Warning! A room with that name already existis. Do you want to overwrite it?";
                _confirmAction = ConfirmAction.OVERWRITE_SAVE;
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
        private void btnTileEditMode_Click()
        {
            _editMode = EditMode.TILE;
            _veHighlightTileEditMode.visible = true;
            _veHighlightDoorEditMode.visible = false;
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
