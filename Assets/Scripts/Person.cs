using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public GameObject Corpse;
    public GameObject Shirt;
    public GameObject Hair;
    public GameObject OpenMouth;

    public int score = 0;
    public string ethnicGroup;
    public bool isMale;

    public Corpse[] corpses;
    public Part[] shirts;
    public Part[] hairs;
    public Part[] mooths;

    public int personSeed;

    // Start is called before the first frame update
    public void Build(int seed, bool male)
    {
        isMale = male;
        personSeed = seed;
        Random.InitState (seed);
        Corpse selectedCorpse = corpses[Random.Range(0, corpses.Length)];
        Corpse.GetComponent<SpriteRenderer>().sprite = selectedCorpse.sprite;
        ethnicGroup = selectedCorpse.ethnicGroup;
        
        Part selectedMooth = mooths[Random.Range(0, mooths.Length)];
        OpenMouth.GetComponent<SpriteRenderer>().sprite = selectedMooth.sprite;
        score += selectedMooth.value;
        
        Part selectedShirt = shirts[Random.Range(0, shirts.Length)];
        Shirt.GetComponent<SpriteRenderer>().sprite = selectedShirt.sprite;
        score += selectedShirt.value;

        Part selectedHair = hairs[Random.Range(0, hairs.Length)];
        Hair.GetComponent<SpriteRenderer>().sprite = selectedHair.sprite;
        score += selectedHair.value;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
