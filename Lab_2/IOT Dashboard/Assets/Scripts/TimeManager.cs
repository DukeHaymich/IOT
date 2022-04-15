using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Text time;
    void Start()
    {
        time = GameObject.Find("/Canvas/Header/Time").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time.text = System.DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy");
    }
}
