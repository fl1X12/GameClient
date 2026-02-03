using System;

using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    public int index;
    public int value;
    Image selfImageRef;
    Button SelfButtonRef;
    [SerializeField]public Sprite[] cellImages;

    void Awake()
    {
        selfImageRef= GetComponent<Image>();
        SelfButtonRef = GetComponent<Button>();
    }
    public void Initialize(int index)
    {
        this.index=index;
        this.value=-1;
        SelfButtonRef.interactable=true;
    }

    public void AttachEvent(Action<int> OnClickCallBack)
    {
        SelfButtonRef.onClick.AddListener(()=>OnClickCallBack(GetIndex()));
    }


    public void PerformMove(int token)
    {
        selfImageRef.sprite=cellImages[token];
        this.value=token;
        SelfButtonRef.interactable=false;
    }

    public int GetIndex()
    {
        return this.index;
    }
   
}
