using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class tileManager : MonoBehaviour
{
    public controls controls;
    public int xgrid;
    public int ygrid;
    public Color[] tilecolors;
    public GameObject tile;
    public float gap;
    public float tilegapx;
    public float tilegapy;
    public int ci;
    public List<GameObject> tiles;
    public float tiledropspeed;
    public TMP_Text scoretxt;
    public TMP_Text highscoretxt;

    public int score;
    public int highscore;


    public int bombindex;
    public int bombremaingtime;
    public bool isbombactive;
    public int bombcolor;
    public GameObject bombtxtobj;
    public TMP_Text remainingtimebombtxt;

    public Button endgamebutton;




    public bomb bomb;
    public GameObject expl;


    
    
    void OnEnable()
    {
        spawntiles();
        //check matched blocks after tile spawn
        //InvokeRepeating("checkforgrouped", 0.05f, 0.05f);
        checkforgrouped();
        Invoke("setvaluesafterinitialize", 0.2f);
    }
    void setvaluesafterinitialize()
    {

       // CancelInvoke("checkforgrouped");
        score = 0;
        highscore = 0;
        scoretxt.text = score.ToString();
        highscoretxt.text = highscore.ToString();


        /*You are trying to create a MonoBehaviour using the 'new' keyword.  This is not allowed.  
         * MonoBehaviours can only be added using AddComponent().
         * 
         * attached bomb class to bomb object and call setbombvalues instad of creating object instance and call constructor
         */
        bomb.setbombvalues(Random.Range(0,xgrid*ygrid),9,false,xgrid*ygrid);
       // isbombactive = false;
      //  bombtxtobj.transform.position = new Vector3(-30,0,0);
    }



    void Update()
    {

    
        if (Input.touchCount > 3)
        {
            spawntiles();
            checkforgrouped();
        }

    }
    void spawntiles()
    {
        /*
#if UNITY_EDITOR
        if (tiles.Count > 0)
        {
            foreach (GameObject g in tiles)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    DestroyImmediate(g.gameObject);
                };
            }
            tiles.Clear();
        }

#endif
        */
         //Create tiles by x y values
        for (int i = 0; i < xgrid; i++)
        {
            for (int j = 0; j < ygrid; j++)
            {
                if (i % 2 == 0)
                {
                    GameObject s = Instantiate(tile, new Vector2(i + tilegapx * i, (j + tilegapy * j) + gap), transform.rotation);
                    ci = Random.Range(0, tilecolors.Length);
                    s.GetComponent<tile>().sprite.color = tilecolors[ci];
                    s.GetComponent<tile>().colorindex = ci;
                   // s.GetComponent<tile>().xx = i;
                   // s.GetComponent<tile>().yy = j;


                    tiles.Add(s);
                }
                else
                {
                    GameObject s = Instantiate(tile, new Vector2(i + tilegapx * i, j + tilegapy * j), transform.rotation);
                    ci = Random.Range(0, tilecolors.Length);
                    s.GetComponent<tile>().sprite.color = tilecolors[ci];
                    s.GetComponent<tile>().colorindex = ci;
                   // s.GetComponent<tile>().xx = i;
                   // s.GetComponent<tile>().yy = j;
                    tiles.Add(s);
                }

            }
        }
    }





    public void turnpassed()   //after user input in controls.cs
    {
        if (bomb.isbombactive==false)
        {
            return;
        }
        bomb.updatebombvalues();
        /*
        bomb.bombremaingtime -= 1;
        remainingtimebombtxt.text = bombremaingtime.ToString();

        */
        //game ended, play again button setted active
        if (bomb.bombremaingtime<=0) 
        {
            Debug.Log("Game Over");
            scoretxt.text = "Game Over";
            controls.enabled = false;
            endgamebutton.gameObject.SetActive(true);

            
        }
    }


    /*   Check all the tiles one by one if ther is any same color near
     *   check if the distance between tiles are lower than specific values to  (3 blocks can be Y or l shape )
     *    if colors and distance calculation are good then send these tiles as list to moveblocksafterexplode function.
     *    
     *    in case of matched tiles, check again as the new tiles can be matched again    (4 5 group disabled as the requirements says 3 hexagon)
     */
    public bool checkforgrouped()
    {
        refresh();
        Debug.Log("chek worked");
        for (int tile = 0; tile < tiles.Count; tile++)
        {

            float xpos = tiles[tile].transform.position.x;
            xpos = Mathf.Round(xpos * 10.0f) * 0.1f;
            //Debug.Log("x position of tile:"+tiles[tile].transform.position.x+" int version:"+xpos);
            tiles[tile].transform.position = new Vector2(xpos, tiles[tile].transform.position.y);
            if (tiles[tile].GetComponent<tile>().colorindex == 10)
            {
                continue;
            }
            List<Collider2D> nearbs = new List<Collider2D>();
            Collider2D[] nearblocks = Physics2D.OverlapCircleAll(tiles[tile].transform.position, 0.6f);
            for (int i = 0; i < nearblocks.Length; i++)
            {
                if (nearblocks[i].GetComponent<tile>().colorindex == tiles[tile].GetComponent<tile>().colorindex)
                {
                    nearbs.Add(nearblocks[i]);

                }
            }
            /*
             * 
             */
            bool canexplode = false;
            switch (nearbs.Count)
            {
                case 3:
                    if (Vector2.Distance(nearbs[0].transform.position, nearbs[1].transform.position) > 1.1f ||
                        Vector2.Distance(nearbs[0].transform.position, nearbs[2].transform.position) > 1.1f ||
                        Vector2.Distance(nearbs[1].transform.position, nearbs[2].transform.position) > 1.1f)
                    {
                    }
                    else
                    {
                        canexplode = true;
                      //  Debug.Log("3 tile group matched");
                    }
                    break;


                case 4:
                    Vector2 middlepoint;
                    List<Collider2D> tempnearbs = new List<Collider2D>();
                    middlepoint = (nearbs[0].transform.position +
                   nearbs[1].transform.position +
                   nearbs[2].transform.position +
                   nearbs[3].transform.position) / 4;
                    foreach (Collider2D tl in nearbs)
                    {
                        if (Vector2.Distance(middlepoint, tl.transform.position) > 0.4f)
                        {
                            //  nearbs.Remove(tl);
                            canexplode = false;
                        }
                        else
                        {
                            //tempnearbs.Add(tl);
                            canexplode = true;
                          //  Debug.Log("4 tile group matched");


                        }
                    }
                    //nearbs = tempnearbs;
                    if (nearbs.Count<3)
                    {
                       // canexplode = false;
                    }
                    canexplode = false;
                    break;
                case 5:
                    canexplode = false;
                    break;
                case 6:
                    canexplode = true;
                    break;

                default:
                    canexplode = false;
                    break;
            }
            if (canexplode == true)// do movements of tiles after matched tiles
            {
                //StartCoroutine( waitandcall(nearbs));

                moveblocksafterexplode(nearbs);
                
                return true;
            }
            else  //no tiles matched
            {

            }
        }



        return false;

    }
    IEnumerator waitandcall(float time) 
    {
        yield return new WaitForSeconds(time);
        checkforgrouped();

    }




    /*
     * like object pooling move the mathced tiles to top and give new color
     * move down the other tiles in same x position of matched tile  
     */
    public void moveblocksafterexplode(List<Collider2D> explodedblocks)
    {
        StartCoroutine(movetilesup(explodedblocks));

    }
    IEnumerator movetilesup(List<Collider2D> explodedblocks) 
    {
        foreach (Collider2D c in explodedblocks)
        {
            if (c != null)
            {
                float yposition = c.transform.position.y;
                float xposition = c.transform.position.x;


                float zeropoint = 0;
                float xpos = xposition / 0.8f;

                if (xpos % 2 == 0)
                {
                    zeropoint = gap;
                }

                //tile explode part; move them to up and add score
                float distancetonewposition = zeropoint + ygrid * gap * 2f;
                 c.transform.position = new Vector3(c.transform.position.x, distancetonewposition, 0);
                //StartCoroutine(MoveObject(c.gameObject, new Vector3(c.transform.position.x, distancetonewposition, 0), 1f));
                foreach (GameObject tile in tiles)
                {

                    float tilexpos = tile.transform.position.x;
                    float tileypos = tile.transform.position.y;

                    //tile.transform.localScale = new Vector3(1, 1, 1);
                    tile.transform.rotation = Quaternion.Euler(0, 0, 0);
                    xpos = Mathf.Round(xpos * 10.0f) * 0.1f;
                    // ypos = Mathf.Round(ypos * 10.0f) * 0.1f;

                    tile.transform.position = new Vector2(tilexpos, tileypos);

                    //move all the tiles that on same x position to down by one height of tile

                    if (Mathf.Abs(tile.transform.position.x - xposition) <= 0.15f && tile.transform.position.y >= yposition)
                    {
                        // tile.transform.position = tile.transform.position - new Vector3(0, gap*2f , 0);
                        StartCoroutine(MoveObject(tile, tile.transform.position - new Vector3(0, gap * 2f, 0), tiledropspeed));
                    }

                }

                if (bomb.isbombactive == true && c.gameObject == tiles[bomb.bombindex])
                {
                    Debug.Log("bomb defused");
                    bomb.bombexpolode();
                    //bombexpolode();
                }

                score += 5;
                if (score > highscore)
                {
                    highscore = score;
                }
                scoretxt.text = score.ToString();
                highscoretxt.text = highscore.ToString();

                if (score % 1000 == 0)
                {
                    Debug.Log("bomb spawned");
                    //setbomb();
                    int newbombindex = Random.Range(0, xgrid * ygrid);
                    bomb.setbomb(newbombindex, 9, tiles[newbombindex]);
                }
               
                int newcolorindex = Random.Range(0, tilecolors.Length);
                //Debug.Log("tile exploded, color:"+ c.GetComponent<tile>().colorindex+" /  new color :"+newcolorindex);
                c.GetComponent<tile>().colorindex = newcolorindex;
                c.GetComponent<tile>().sprite.color = tilecolors[newcolorindex];

                //For test, make matched tiles white and change color index to not match again with other white ones.
                //c.GetComponent<tile>().colorindex = 10;
                // c.GetComponent<tile>().sprite.color = Color.white;

            }

            yield return new WaitForSeconds(0.25f);//0.44

        }
        StartCoroutine(waitandcall(0f)); //0.3

    }


    IEnumerator MoveObject(GameObject source, Vector3 target, float overTime)
    {

        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            source.transform.position = Vector3.Lerp(source.transform.position, target, (Time.time - startTime) / overTime);
            yield return null;
        }
        source.transform.position = target;

    }



    //disable higlight effect of tiles and detach from cursor gameobject
    public void refresh()
    {
        foreach (GameObject g in tiles)
        {
            g.GetComponent<tile>().sprite.sortingOrder = 1;
            g.GetComponent<tile>().isselectedsprite.color = Color.black;
            g.GetComponent<tile>().isselectedsprite.sortingOrder = 0;
            g.transform.parent = null;
        }
    }

    public void playagain() 
    {        SceneManager.LoadScene("SampleScene");

    }
}
