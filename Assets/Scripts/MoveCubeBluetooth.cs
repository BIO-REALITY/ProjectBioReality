using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;


public class MoveCubeBluetooth : MonoBehaviour
{

    [SerializeField] private GameObject _cube;
    
    private TextMeshProUGUI _datasBluetooth;

    private GameObject _drone;
    private GameObject _camera;
    private GameObject _cameraGameObj;

    private Rigidbody _droneRb;
    
    float timeElapsed;

    private DatasSended firstDatas;
    // Start is called before the first frame update
    void Start()
    {
        _datasBluetooth = GameObject.Find("_datasBluetooth").GetComponent<TextMeshProUGUI>();
        _drone = GameObject.Find("drone");
        _camera = GameObject.Find("Main Camera");
        _cameraGameObj = GameObject.Find("CameraPosition");

        _droneRb = _drone.GetComponent<Rigidbody>();

        InstantiateRandomCubes();
    }

    // Update is called once per frame
    void Update()
    {
      
        DronePosition();
        CameraPosition();

    }


    private DatasSended GetBleDatas()
    {
        string datain =  BluetoothService.ReadFromBluetooth();
        if (datain.Length < 1) return null;
        return JsonConvert.DeserializeObject<DatasSended>(datain);
    }
    
    
    private void DronePosition()
    {
        var dataSended = GetBleDatas();
        if (dataSended == null) return;
        firstDatas = firstDatas == null ? dataSended : firstDatas;
        
        _datasBluetooth.text = $"x : {dataSended.x} y : {dataSended.y} z : {dataSended.z}\n acc : {dataSended.acceleration}";

        var position = new Vector3();

        int x = dataSended.x > 10 ?  1 : dataSended.x < -10 ? - 1 : 0;
        int y = dataSended.y > 10 ?  1 : dataSended.y < -10 ? - 1 : 0;
        int z = dataSended.acceleration > firstDatas.acceleration * 1.5f ? 1 : 0;
        
        position.x = x;
        position.y = y;
        position.z = z;
        
        Quaternion rotation = Quaternion.Euler(20 * -y, 0, 20 * -x);
        _drone.transform.rotation = rotation;//Quaternion.Lerp(_drone.transform.rotation, rotation, timeElapsed / 0.8f);

        if (position == Vector3.zero)
        {
            _droneRb.velocity = Vector3.zero;
            _droneRb.angularVelocity = Vector3.zero;
        }
        
        _droneRb.AddForce(position * 50);
        
        timeElapsed += Time.deltaTime;

    }
    private void CameraPosition()
    {
        Quaternion lookOnLook = Quaternion.LookRotation(_drone.transform.position - _camera.transform.position);

        var objPos = _cameraGameObj.transform.position;
        //_camera.transform.position = new Vector3(objPos.x, objPos.y + 8, objPos.z - 15); 
        _camera.transform.rotation = Quaternion.Slerp( _camera.transform.rotation, lookOnLook, Time.deltaTime * 10);
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, objPos, Time.deltaTime);
    }


    void InstantiateRandomCubes()
    {
        for (int i = 0; i < 500; i++)
        {
            var randX = Random.Range(0, 1000);
            var randY = Random.Range(0, 1000);
            var randSizeX = Random.Range(1, 10);
            var randSizeY = Random.Range(1, 10);
            var cube = Instantiate(_cube);
            cube.transform.position = new Vector3(randX, randSizeY*0.5f, randY);
            cube.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            cube.transform.localScale = new Vector3(randSizeX, randSizeY, randSizeX);
        }
    }
}


public class DatasSended
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    
    [JsonProperty("acc_cmd")]
    public int acceleration { get; set; }
}