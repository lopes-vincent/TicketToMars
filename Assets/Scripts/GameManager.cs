using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public Person malePrefab;
    public Person femalePrefab;

    public GameObject spawn;

    public Queue<GameObject> shipSeats = new Queue<GameObject>();
    
    public WaitSeat[] waitSeats;
    public WaitSeat currentWaitSeat;
    
    public int scoreTotal = 0;
    
    public int maleCount = 200;
    public int femaleCount = 200;
    
    public int whiteCount = 100;
    public int blackCount = 100;
    public int purpleCount = 100;
    public int greenCount = 100;

    public GameObject moneyGaugePoint;
    public GameObject sexGaugePoint;
    public GameObject whiteEthnieGaugePoint;
    public GameObject blackEthnieGaugePoint;
    public GameObject greenEthnieGaugePoint;
    public GameObject purpleEthnieGaugePoint;

    public GameObject SelectHint;
    public GameObject ValidateHint;
    public GameObject LaunchHint;
    
    public Button FillSeatButton;
    public Button RefuseButton;
    public Button NewShipButton;

    public Animator ship;
    
    private void Start()
    {
        GameObject[] seats = GameObject.FindGameObjectsWithTag("ShipSeat");
        foreach (var seat in seats)
        {
            shipSeats.Enqueue(seat);
        }
        MoveGauges();
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        
        foreach (WaitSeat waitSeat in waitSeats)
        {
            waitSeat.GetComponent<SpriteRenderer>().color = Color.white;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (shipSeats.Count > 0 && hit.collider != null && hit.collider.GameObject().CompareTag("WaitSeat"))
        {
            WaitSeat overWaitSeat = hit.collider.GameObject().GetComponent<WaitSeat>();

            if (Input.GetMouseButtonDown(0))
            {
                SelectHint.SetActive(false);
                ValidateHint.SetActive(true);
                FillSeatButton.interactable = true;
                RefuseButton.interactable = true;
                currentWaitSeat = overWaitSeat;
                if (null == currentWaitSeat.person)
                {
                    currentWaitSeat.person = CreatePerson();
                }

                foreach (WaitSeat waitSeat in waitSeats)
                {
                    if (null != waitSeat.person)
                    {
                        waitSeat.person.GameObject().SetActive(false);
                    }

                    if (!waitSeat.disabled)
                    {
                        waitSeat.gameObject.SetActive(true);
                    }
                }

                currentWaitSeat.GameObject().SetActive(false);
                currentWaitSeat.person.GameObject().SetActive(true);
            }
            
            overWaitSeat.GetComponent<SpriteRenderer>().color = Color.gray;
        }
    }

    public void fillSeat()
    {
        if (null != currentWaitSeat && shipSeats.Count > 0)
        {
            GameObject seat = shipSeats.Dequeue();
            seat.transform.GetChild(0).gameObject.SetActive(true);
            Person person = currentWaitSeat.person;
            float shipCoeficient = 1 + Globals.shipLaunched / 10f;
            Debug.Log(shipCoeficient);
            scoreTotal += (int)(person.score * shipCoeficient);
            Globals.persons.Enqueue(person);
            if (person.isMale)
            {
                maleCount++;
            }
            else
            {
                femaleCount++;
            }

            if (person.ethnicGroup == "white")
            {
                whiteCount++;
            }

            if (person.ethnicGroup == "black")
            {
                blackCount++;
            }

            if (person.ethnicGroup == "green")
            {
                greenCount++;
            }

            if (person.ethnicGroup == "purple")
            {
                purpleCount++;
            }
            
            person.GameObject().SetActive(false);
            currentWaitSeat.disabled = true;
            currentWaitSeat = null;
            SelectHint.SetActive(true);
            ValidateHint.SetActive(false);

            FillSeatButton.interactable = false;
            RefuseButton.interactable = false;

            if (shipSeats.Count == 0)
            {
                Debug.Log("nEW SHIP");
                SelectHint.SetActive(false);
                NewShipButton.interactable = true;
                LaunchHint.SetActive(true);
            }
        }
    }

    public void Refuse()
    {
        Person person = currentWaitSeat.person;
        person.GameObject().SetActive(false);

        currentWaitSeat.GameObject().SetActive(true);
        currentWaitSeat = null;

        SelectHint.SetActive(true);
        ValidateHint.SetActive(false);

        FillSeatButton.interactable = false;
        RefuseButton.interactable = false;
    }

    public void NewShip()
    {
        currentWaitSeat = null;
        GameObject[] seats = GameObject.FindGameObjectsWithTag("ShipSeat");
        foreach (var seat in seats)
        {
            seat.transform.GetChild(0).gameObject.SetActive(false);
            shipSeats.Enqueue(seat);
        }
        
        foreach (WaitSeat waitSeat in waitSeats)
        {
            if (null != waitSeat.person)
            {
                Destroy( waitSeat.person.GameObject());
            }
            waitSeat.person = null;
            waitSeat.gameObject.SetActive(true);
            waitSeat.disabled = false;
        }

        Globals.shipLaunched++;
        MoveGauges();
        SelectHint.SetActive(true);
        NewShipButton.interactable = false;
        LaunchHint.SetActive(false);
        ship.enabled = false;
        ship.enabled = true;
        ship.Play("Ship");
    }

    private void MoveGauges()
    {
        int totalPerson = maleCount + femaleCount;
        float gaugeMax = 63f;
        
        Debug.Log(scoreTotal);
        float moneyCap = 600f;
        // moneyGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, malePercentage * gaugeMax  ,0);
        moneyGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (16f/moneyCap)*scoreTotal,0);
        if (scoreTotal > moneyCap)
        {
            GameOver("Global social score has gone over 600 points");
        }
        else if (scoreTotal < -moneyCap)
        {
            GameOver("Global social score has gone under -600 points");
        }

        float malePercentage = ((float)maleCount / (float)totalPerson);
        Debug.Log(malePercentage);
        sexGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, malePercentage * gaugeMax  ,0);
        if (malePercentage > 0.7)
        {
            GameOver("Male proportion has gone over 70%");
        }
        else if (malePercentage < 0.3)
        {
            GameOver("Female proportion has gone over 70%");
        }

        int ethniGameOverPercentage = 40;
        
        float whitePercentage = ((float)whiteCount / (float)totalPerson) * 100;
        string ethnicLooseMessage = "An ethnic group as gone over "+ethniGameOverPercentage+"% of your population";
        if (whitePercentage > ethniGameOverPercentage)
        {
            GameOver(ethnicLooseMessage);
        } 
        
        whiteEthnieGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, whitePercentage * gaugeMax / ethniGameOverPercentage,0);
        
        float blackPercentage = ((float)blackCount / (float)totalPerson) * 100;
        blackEthnieGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, blackPercentage * gaugeMax / ethniGameOverPercentage,0);
        if (blackPercentage > ethniGameOverPercentage)
        {
            GameOver(ethnicLooseMessage);
        } 

        float greenPercentage = ((float)greenCount / (float)totalPerson) * 100;
        greenEthnieGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, greenPercentage * gaugeMax / ethniGameOverPercentage,0);
        if (greenPercentage > ethniGameOverPercentage)
        {
            GameOver(ethnicLooseMessage);
        } 
        
        float purplePercentage = ((float)purpleCount / (float)totalPerson) * 100;
        purpleEthnieGaugePoint.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, purplePercentage * gaugeMax / ethniGameOverPercentage,0);
        if (purplePercentage > ethniGameOverPercentage)
        {
            GameOver(ethnicLooseMessage);
        } 
    }

    public void GameOver(string reason = "Undefined")
    {
        Globals.GameOverReason = reason;
        SceneManager.LoadScene("GameOver");
    }

    private Person CreatePerson()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        int rand = Random.Range(0, 100);
        Person prefab = rand < 50 ? malePrefab : femalePrefab;

        Person person = Instantiate(prefab, spawn.transform.position, Quaternion.identity);
        person.Build(seed, rand < 50);
        return person;
    }
}
