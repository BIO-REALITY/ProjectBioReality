using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveCubeBluetooth : MonoBehaviour
{

    private TextMeshProUGUI _datasBluetooth;

    private GameObject _cubeTest;
    // Start is called before the first frame update
    void Start()
    {
        _datasBluetooth = GameObject.Find("_datasBluetooth").GetComponent<TextMeshProUGUI>();
        _cubeTest = GameObject.Find("_cubeTest");
    }

    // Update is called once per frame
    void Update()
    {
        string datain =  BluetoothService.ReadFromBluetooth();
        if (datain.Length < 1) return;
        var dataSended = JsonConvert.DeserializeObject<DatasSended>(datain);
        _datasBluetooth.text = $"x : {dataSended.x}\ny : {dataSended.y}\nz : {dataSended.z}";

        var position = _cubeTest.transform.position;
        position.y = dataSended.y/2;
        position.x = dataSended.x / 2;
        _cubeTest.transform.position = position;

    }
}


public class DatasSended
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public List<float> handString { get; set; }
}