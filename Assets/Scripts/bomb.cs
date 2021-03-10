using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class bomb : MonoBehaviour
{

    public int bombindex; 
    //no need to use getter/setters
    public int bombremaingtime;
    public bool isbombactive;
    public int bombcolor;
    public int tilesize;
    public GameObject bombtxtobj;
    public TMP_Text remainingtimebombtxt;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bomb(int bombindex, int bombremaingtime, bool isbombactive, int bombcolor, int tilesize)
    {
        this.bombindex = bombindex;
        this.bombremaingtime = bombremaingtime;
        this.isbombactive = isbombactive;
        this.bombcolor = bombcolor;
        this.tilesize = tilesize;
    }

    public void setbombvalues(int bombindex, int bombremaingtime, bool isbombactive, int tilesize)
    {
        this.bombindex = bombindex;
        this.bombremaingtime = bombremaingtime;
        this.isbombactive = isbombactive;
        this.tilesize = tilesize;
    }

    public void bombexpolode()    // bomb defused-
    {
        isbombactive = false;
        bombtxtobj.transform.position = new Vector3(0, -30, 0);
        bombtxtobj.transform.parent = null;
    }
    public void setbomb(int bombindex,int bombremaingtime, GameObject tile)   //every 1000 score, set bomb active
    {
        isbombactive = true;
        this.bombindex = bombindex;
        bombtxtobj.transform.position = tile.transform.position;
        bombtxtobj.gameObject.transform.parent = tile.transform;
        this.bombremaingtime = bombremaingtime;
        bombcolor = tile.GetComponent<tile>().colorindex;
    }
    public void updatebombvalues() 
    {
        bombremaingtime -= 1;
        remainingtimebombtxt.text = bombremaingtime.ToString();
    }
}
