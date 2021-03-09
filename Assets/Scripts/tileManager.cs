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





    public GameObject expl;


    
    
    void OnEnable()
    {
        spawntiles();
        //checkforgrouped();
        InvokeRepeating("checkforgrouped", 0.05f, 0.05f);
        Invoke("settext", 0.2f);
    }
    void settext()
    {

        CancelInvoke("checkforgrouped");
        score = 0;
        highscore = 0;
        scoretxt.text = score.ToString();
        highscoretxt.text = highscore.ToString();
        isbombactive = false;
        bombtxtobj.transform.position = new Vector3(-30,0,0);
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
        for (int i = 0; i < xgrid; i++)
        {
            for (int j = 0; j < ygrid; j++)
            {
                if (i % 2 == 0)
                {
                    GameObject s = Instantiate(tile, new Vector2(i + tilegapx * i, (j + tilegapy * j) - gap), transform.rotation);
                    ci = Random.Range(0, tilecolors.Length);
                    s.GetComponent<tile>().sprite.color = tilecolors[ci];
                    s.GetComponent<tile>().colorindex = ci;
                    s.GetComponent<tile>().xx = i;
                    s.GetComponent<tile>().yy = j;


                    tiles.Add(s);
                }
                else
                {
                    GameObject s = Instantiate(tile, new Vector2(i + tilegapx * i, j + tilegapy * j), transform.rotation);
                    ci = Random.Range(0, tilecolors.Length);
                    s.GetComponent<tile>().sprite.color = tilecolors[ci];
                    s.GetComponent<tile>().colorindex = ci;
                    s.GetComponent<tile>().xx = i;
                    s.GetComponent<tile>().yy = j;
                    tiles.Add(s);
                }

            }
        }
    }

    public void bombexpolode() 
    {
        isbombactive = false;
        bombtxtobj.transform.position = new Vector3(0, -30, 0);
        bombtxtobj.transform.parent = null;
        /*
        bombtxtobj.transform.position = new Vector3(0,-30,0);
        List<Collider2D> blocks = new List<Collider2D>();
        foreach (GameObject c in tiles)
        {
            if (c.GetComponent<tile>().colorindex==colorindex)
            {
                blocks.Add(c.GetComponent<Collider2D>());
            }
        }
        moveblocksafterexplode(blocks);
        */
    }
    public void setbomb() 
    {
        isbombactive = true;
        bombindex = Random.Range(0,xgrid*ygrid);
        bombtxtobj.transform.position = tiles[bombindex].transform.position;
        bombtxtobj.gameObject.transform.parent = tiles[bombindex].transform;
        bombremaingtime = 9;
        bombcolor = tiles[bombindex].GetComponent<tile>().colorindex;
    }
    public void turnpassed() 
    {
        if (isbombactive==false)
        {
            return;
        }
        bombremaingtime -= 1;
        remainingtimebombtxt.text = bombremaingtime.ToString();
        if (bombremaingtime<=0)
        {
            Debug.Log("Game Over");
            scoretxt.text = "Game Over";
            controls.enabled = false;
            endgamebutton.gameObject.SetActive(true);

            
        }
    }




    public bool checkforgrouped()
    {
        bool exploded = false;
        for (int tile = 0; tile < tiles.Count; tile++)
        {
            float xpos = tiles[tile].transform.position.x;
            xpos = Mathf.Round(xpos * 10.0f) * 0.1f;
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
                    }
                    break;


                case 4:
                    Vector2 middlepoint;

                    middlepoint = (nearbs[0].transform.position +
                   nearbs[1].transform.position +
                   nearbs[2].transform.position +
                   nearbs[3].transform.position) / 4;
                    foreach (Collider2D tl in nearbs)
                    {
                        if (Vector2.Distance(middlepoint, tl.transform.position) > 0.5f)
                        {
                            //  nearbs.Remove(tl);
                            canexplode = false;
                        }
                        else
                        {
                            canexplode = true;
                        }
                    }
                    canexplode = false;
                    break;
                case 5:
                    canexplode = false;
                    break;
                case 6:
                    canexplode = false;
                    break;

                default:
                    canexplode = false;
                    break;
            }
            if (canexplode == true)// patlayan oldu bloklari yeniden diz ve tekrar check et
            {
                exploded = true;
                moveblocksafterexplode(nearbs);
                // tile = 0;
                //return true;
                //checkforgrouped();
                //exploded = true;
                //return checkforgrouped();
                //return exploded;
            }
            else  //patlayan olmadi fonksiyon bitebilir.
            {
                //return false;
                //exploded = true;
            }


            if (exploded == true && tile == tiles.Count - 1)
            {
                // print("bitti ve true dondu");
                return true;
            }
        }
        if (exploded)
        {

            return checkforgrouped();
        }


        return false;

    }






    public void moveblocksafterexplode(List<Collider2D> explodedblocks)
    {
        Vector2 pos = (explodedblocks[0].transform.position + explodedblocks[1].transform.position + explodedblocks[2].transform.position) / 3f;
        //GameObject exploeffect = Instantiate(expl, new Vector2(pos.x, pos.y), Quaternion.identity);
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
                    zeropoint = -0.46f;
                }

                //blocks exploded;

                float distancetonewposition = zeropoint + ygrid * 0.92f;
                //distancetonewposition = 15+c.transform.position.y;
                c.transform.position = new Vector3(c.transform.position.x, distancetonewposition, 0);
                if (isbombactive==true&& c.gameObject == tiles[bombindex])
                {
                    bombexpolode();
                   // isbombactive = false;
                }
               // StartCoroutine(MoveObject(c.gameObject, new Vector3(c.transform.position.x, distancetonewposition, 0), 1f));
                
                score += 5;
                if (score > highscore)
                {
                    highscore = score;
                }
                scoretxt.text = score.ToString();
                highscoretxt.text = highscore.ToString();

                if (score%1000==0)
                {
                    Debug.Log("bomb spawned");
                    setbomb();
                }

                foreach (GameObject tile in tiles)
                {
                    float tilexpos = tile.transform.position.x;
                    float tileypos = tile.transform.position.y;

                    tile.transform.localScale = new Vector3(1, 1, 1);
                    tile.transform.rotation = Quaternion.Euler(0, 0, 0);
                    xpos = Mathf.Round(xpos * 10.0f) * 0.1f;
                    // ypos = Mathf.Round(ypos * 10.0f) * 0.1f;

                    tile.transform.position = new Vector2(tilexpos, tileypos);

                    if (Mathf.Abs(tile.transform.position.x - xposition) <= 0.1f && tile.transform.position.y > yposition)
                    {
                        tile.transform.position = tile.transform.position - new Vector3(0, 0.92f, 0);
                      //  StartCoroutine( MoveObject(tile, tile.transform.position - new Vector3(0, 0.92f, 0), 2f));

                    }
                }
                int newcolorindex = Random.Range(0, tilecolors.Length);
                c.GetComponent<tile>().colorindex = newcolorindex;
                c.GetComponent<tile>().sprite.color = tilecolors[newcolorindex];

                 //c.GetComponent<tile>().colorindex = 10;
                // c.GetComponent<tile>().sprite.color = Color.white;

            }
        }

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
    {
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene("SampleScene");

    }
}
