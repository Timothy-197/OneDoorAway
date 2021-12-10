using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccomplishmentPanel : MonoBehaviour
{
    


    public void ActivateAccomplishmentPanel()
    {
        this.gameObject.SetActive(true);
    }
    public void DeactivateAccomplishmentPanel()
    {
        this.gameObject.SetActive(false);
    }
}
