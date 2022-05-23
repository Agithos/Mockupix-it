using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public int afigitis;
    public Enemies[] enemies = new Enemies[7];
    public GameObject myKoumpia;
    public GameObject KouBerdemata;
    public int Net_ID;
    public int sinolo_paixtwn;
    public GameObject myNetwork;
    public byte stage = 0;
    public bool isafigitis = false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] ola = Resources.FindObjectsOfTypeAll<GameObject>();
        for (int i = 0; i < ola.Length; i++)
        {
            if (ola[i].name == "KOUMPIA")
            {
                myKoumpia = ola[i];
            }
            else if (ola[i].name == "Network")
            {
                myNetwork = ola[i];
            }
            else if (ola[i].name == "Kartes_epiloghs")
            {
                KouBerdemata = ola[i];
            }
            else
            {
                for(int j=0; j<6; j++)
                {
                    if(ola[i].name == j.ToString())
                    {
                        enemies[j] = ola[i].GetComponent<Enemies>();
                        enemies[j].karta = ola[i].GetComponent<Karta>();
                    }
                }
            }
        }
    }

    public void Arxh(int plithos)
    {
        for (int i=0; i<plithos; i++)
        {
            Enemies[] enemies = new Enemies[plithos];
        }
    }
    public void berdemata()
    {
        KouBerdemata.SetActive(true);
        epelekse();
    }
    public void neos_giros()
    {
        KouBerdemata.SetActive(false);
        afigitis++;
        if (afigitis >= sinolo_paixtwn)
        {
            afigitis = 0;
        }

        if (afigitis == Net_ID)
        {
            stage = 4;
            isafigitis = true;
            epelekse();
        }
        else
        {
            isafigitis = false;
           stage = 3;
        }
    }

    public void submit()
    {
        myNetwork.GetComponent<Network>().send_epilogh(stage);
    }
    public void epelekse()
    {
        myKoumpia.SetActive(true);
    }
  

    public void telos()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
