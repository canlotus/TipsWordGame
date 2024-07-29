using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Say : MonoBehaviour
{

    public string defaultText = "Default Text";

    Text currentText;

    // Start is called before the first frame update
    void Start()
    {
        currentText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        currentText.text = GameLanguage.gl.Say(defaultText);
    }

}
