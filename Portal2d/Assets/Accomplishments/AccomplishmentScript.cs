using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccomplishmentScript : MonoBehaviour
{
    public string findText;
    public LayerMask playerLayer;
    public GameObject findUI; // ui informing the player that he find a piece of memory
    public Camera cam;
    public Canvas cav;

    // Start is called before the first frame update
    void Start()
    {
        findUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 31) {
            //Debug.Log("trigger UI!");
            findUI.SetActive(true);
            findUI.GetComponent<Text>().text = findText;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 31)
        {
            findUI.SetActive(false);
        }
    }
}
