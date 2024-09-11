using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CartScroll : MonoBehaviour
{
    public GameObject item;
    public Transform contents;

    void Start()
    {
        for(int i = 0; i <= 30; i++)
        {
            Instantiate(item, contents);
        }
    }
}
