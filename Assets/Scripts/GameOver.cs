using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject personContainer;
    
    public Person malePrefab;
    public Person femalePrefab;
    
    public GameObject reason;
    public GameObject shipCount;
    
    public 

    void Start()
    {
        reason.GetComponent<TextMeshProUGUI>().text = Globals.GameOverReason;
        shipCount.GetComponent<TextMeshProUGUI>().text = Globals.shipLaunched + " ships launched !";
        Debug.Log(Globals.persons.Count);
        SpawnColony();
    }

    private void SpawnColony()
    {
        Vector2 gridSize = new Vector2(85, 50);
        for (int y = 0; y < gridSize.y; y += 5)
        {
            for (int x = 0; x < gridSize.x; x += 5)
            {
                if (Globals.persons.Count == 0)
                {
                   return;
                }

                Person dequeued = Globals.persons.Dequeue();
                Debug.Log(dequeued.personSeed);
                Debug.Log(dequeued.isMale);
                Person prefab = dequeued.isMale ? malePrefab : femalePrefab;
                Person person = Instantiate(prefab,new Vector3(0, 0, 0), Quaternion.identity, personContainer.transform);
                person.transform.localPosition = new Vector3(x/5f, y/5f, 0);
                person.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                person.Build(dequeued.personSeed, dequeued.isMale);
            }
        }
    }

    public void Restart()
    {
        Globals.shipLaunched = 0;
        Globals.persons = new Queue<Person>();
        SceneManager.LoadScene("Game");
    }
}
