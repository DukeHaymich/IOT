using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectIndicator : MonoBehaviour
{
    // Start is called before the first frame update
    public Image temperature;
    public Image humidity;
    private float time = 0f;
    private bool clockwise = true;
    void Start()
    {
        temperature = GameObject.Find("/Canvas/LayerDashboard/Temperature/Border").GetComponent<Image>();
        humidity = GameObject.Find("/Canvas/LayerDashboard/Humidity/Border").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (DataSession.isConnected)
        {
            if (clockwise)
            {
                time += Time.deltaTime * 0.5f;
            }
            else
            {
                time -= Time.deltaTime * 0.5f;
            }
            temperature.fillAmount = time;
            humidity.fillAmount = time;
        }
        if (time > 1)
        {
            time = 1f;
            clockwise = false;
            temperature.fillClockwise = clockwise;
            humidity.fillClockwise = !clockwise;
        }
        if (time < 0)
        {
            time = 0f;
            clockwise = true;
            temperature.fillClockwise = clockwise;
            humidity.fillClockwise = !clockwise;
        }
    }
}
