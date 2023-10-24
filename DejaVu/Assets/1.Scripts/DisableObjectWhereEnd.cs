using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectWhereEnd : MonoBehaviour
{
    private void Update()
    {
        gameObject.SetActive(!GameManager.isEnd);
    }
}
