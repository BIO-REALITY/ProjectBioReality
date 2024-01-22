using UnityEngine;
using TMPro;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BluetoothManager : MonoBehaviour
{

    public GameObject _buttonExemple;
    private GameObject _bleContent;

    private TextMeshProUGUI _statuText;
    private TextMeshProUGUI _statuTextConnection;
    
    Vector2 position = new(0, 100);


    // Start is called before the first frame update
    void Start()
    {

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

#if UNITY_2020_2_OR_NEWER
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.CoarseLocation)
            || !Permission.HasUserAuthorizedPermission(Permission.FineLocation)
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
            || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
            Permission.RequestUserPermissions(new string[] {
                Permission.CoarseLocation,
                Permission.FineLocation,
                "android.permission.BLUETOOTH_SCAN",
                "android.permission.BLUETOOTH_ADVERTISE",
                "android.permission.BLUETOOTH_CONNECT"
            });
#endif
#endif
        
        _bleContent = GameObject.Find("Content");
        _statuText = GameObject.Find("_statuText").GetComponent<TextMeshProUGUI>();
        _statuTextConnection = GameObject.Find("_statuTextConnection").GetComponent<TextMeshProUGUI>();
        
        BluetoothService.CreateBluetoothObject();
        string[] devices = BluetoothService.GetBluetoothDevices();

        foreach (var device in devices)
        {
            InstantiateButtons(device);
        }

        _statuText.text = $"Bluetooth active : {devices.Length} devices";
    }


    void InstantiateButtons(string device)
    {
        var button = Instantiate(_buttonExemple, _bleContent.transform);

        button.GetComponent<RectTransform>().anchoredPosition = position;
        button.GetComponentInChildren<TextMeshProUGUI>().text = device;
        
        button.GetComponent<Button>().onClick.AddListener(() => ConnectBluetooth(device));
        
        position.y -= 100;
    }


    void ConnectBluetooth(string name)
    {
        var isConnected = BluetoothService.StartBluetoothConnection(name);
        _statuTextConnection.text = $"Connection to {name} : {isConnected}";
        if (isConnected) SceneManager.LoadScene("Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
