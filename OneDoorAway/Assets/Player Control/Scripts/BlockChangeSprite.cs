// you can reference this link to check how to change the cplor property of a sprite
// https://answers.unity.com/questions/835759/making-sprites-darker.html
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockChangeSprite : MonoBehaviour
{
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
    }

    void OnDestroy()
    {

    }

    // Update is called once per frame
    private void changeSpriteColor(float r, float g, float b, float a) 
    {
        Debug.Log("Sprite change!");
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
    }
}
