using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // setup vars
    public GameObject cardPrefab;
    public GameObject rootPrefab;

    // deck vars
    public List<string> deck = new List<string>();
    public int deckSize = 52;
    private string[] suites = { "S", "H", "C", "D" };
    private int columnCount = 7;

    // game state data
    public string cardTheme = "Standard";
    [SerializeField] public List<List<GameObject>> columns = new List<List<GameObject>>();
    [SerializeField] public List<GameObject> col1;
    [SerializeField] public List<GameObject> col2;
    [SerializeField] public List<GameObject> col3;
    [SerializeField] public List<GameObject> col4;
    [SerializeField] public List<GameObject> col5;
    [SerializeField] public List<GameObject> col6;
    [SerializeField] public List<GameObject> col7;
    [SerializeField] public List<GameObject> drawPile;
    [SerializeField] public List<GameObject> activeDrawPile;
    [SerializeField] public List<GameObject> discardPile;
    [SerializeField] public List<GameObject> victorySpades;
    [SerializeField] public List<GameObject> victoryHearts;
    [SerializeField] public List<GameObject> victoryClubs;
    [SerializeField] public List<GameObject> victoryDiamonds;
    public GameObject sourceObj;
    public GameObject targetObj;
    GameObject exitMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        exitMenuUI = GameObject.Find("ExitMenu");
        exitMenuUI.SetActive(false);
        CreateDeck();
        NewGameState(deck);
        DisplayGameState();
        Testing();
    }

    void Testing()
    {
        col1 = columns[0];
        col2 = columns[1];
        col3 = columns[2];
        col4 = columns[3];
        col5 = columns[4];
        col6 = columns[5];
        col7 = columns[6];
        drawPile = columns[7];
        activeDrawPile = columns[8];
        discardPile = columns[9];
        victorySpades = columns[10];
        victoryHearts = columns[11];
        victoryClubs = columns[12];
        victoryDiamonds = columns[13];
    }

    // Update is called once per frame
    void Update()
    {
        if (columns[columnCount + 3].Count + columns[columnCount + 4].Count + columns[columnCount + 5].Count + columns[columnCount + 6].Count == 56)
        {
            TextMeshProUGUI victoryTextUI;
            victoryTextUI = GameObject.Find("Victory").GetComponent<TextMeshProUGUI>();
            if (victoryTextUI.enabled == false)
            {
                victoryTextUI.enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitMenuUI.activeSelf == false)
            {
                exitMenuUI.SetActive(true);
            }
            else
            {
                exitMenuUI.SetActive(false);
            }
        }

            if (Input.GetMouseButtonDown(0))
        {
            // send ray to touch position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            
            // CASE: a collider was hit
            if (hit == true)
            {
                Debug.Log("Card hit");
                GameObject hitObj = hit.collider.gameObject;
                // CASE: selected a root slot
                if (hitObj.GetComponent<ColumnRoot>() != null)
                {
                    HitRoot(hitObj);
                    return;
                }
                // CASE: selected a card
                if (hitObj.GetComponent<Card>() != null)
                {
                    HitCard(hitObj);
                    return;
                }
            }
            // CASE: no collider was hit
            else
            {
                Debug.Log("Miss");
            }
        }
    }


    void HitRoot(GameObject hitObj)
    {
        ColumnRoot hitRoot = hitObj.GetComponent<ColumnRoot>();
        // CASE: no card already selected
        if (sourceObj == null && hitRoot.currentColumn == columnCount)
        {
            RefillDrawPile(hitObj);
        }
        else if (sourceObj == null)
        {
            Debug.Log("Cannot select root as first card");
            return;
        }
        // CASE: a card is already selected
        if (sourceObj != null)
        {
            Debug.Log("Moving cards to a root slot");
            Card sourceCard = sourceObj.GetComponent<Card>();
            List<GameObject> sourceColumn = columns[sourceCard.currentColumn];
            List<GameObject> targetColumn = columns[hitRoot.currentColumn];
            int sourceIndex = sourceColumn.IndexOf(sourceObj);
            // CASE: moving to a victory column root, must be last card in a game column, must be ace
            if (hitRoot.currentColumn >= columnCount + 3 && sourceColumn.Count == sourceIndex + 1 && sourceCard.value == 1)
            {
                GameObject tempCard = sourceColumn[sourceColumn.Count - 1];
                tempCard.GetComponent<Card>().currentColumn = hitRoot.currentColumn;
                sourceColumn.RemoveAt(sourceColumn.Count - 1);
                targetColumn.Add(tempCard);
                // clear card selections
                sourceObj.GetComponent<SpriteRenderer>().color = Color.white;
                sourceObj = null;
                targetObj = null;
            }
            // CASE: moving to a game column root
            else if (hitRoot.currentColumn < columnCount)
            {
                // move copy of selected cards to temp column
                List<GameObject> tempColumn = sourceColumn.GetRange(sourceIndex, sourceColumn.Count - sourceIndex);
                // update all column vars
                foreach (GameObject obj in tempColumn)
                {
                    obj.GetComponent<Card>().currentColumn = hitRoot.currentColumn;
                }
                //clean up and move
                sourceColumn.RemoveRange(sourceIndex, sourceColumn.Count - sourceIndex);
                targetColumn.AddRange(tempColumn);
                // clear card selections
                sourceObj.GetComponent<SpriteRenderer>().color = Color.white;
                sourceObj = null;
                targetObj = null;
            }
            
            // refresh display
            DisplayGameState();
        }
    }

    void HitCard(GameObject hitObj)
    {
        // CASE: card is not selectable
        if (hitObj.GetComponent<Card>().isSelectable == false)
        {
            Debug.Log("Card not selectable");
            return;
        }
        // CASE: no card already selected
        if (sourceObj == null)
        {
            Debug.Log(hitObj.GetComponent<Card>().currentColumn);
            // CASE: draw a new set of cards
            if (hitObj.GetComponent<Card>().currentColumn == columnCount)
            {
                DrawCard(hitObj);
                DisplayGameState();
                return;
            }
            Debug.Log("Selected a source card");
            hitObj.GetComponent<SpriteRenderer>().color = Color.gray;
            sourceObj = hitObj;
            return;
            
        }
        Card sourceCard = sourceObj.GetComponent<Card>();
        Card targetCard = hitObj.GetComponent<Card>();
        SpriteRenderer sourceRenderer = sourceObj.GetComponent<SpriteRenderer>();
        SpriteRenderer targetRenderer = hitObj.GetComponent<SpriteRenderer>();
        List<GameObject> sourceColumn = columns[sourceCard.currentColumn];
        List<GameObject> targetColumn = columns[targetCard.currentColumn];
        int sourceIndex = sourceColumn.IndexOf(sourceObj);
        // CASE: selected same card again
        if (hitObj == sourceObj)
        {
            Debug.Log("Deselected the source card");
            targetRenderer.color = Color.white;
            sourceObj = null;
            return;
        }
        // CASE: a card is already selected
        else
        {
            // CASE: the card is in the victory columns, source is the last card in its game column, and target card value is one less than source
            if (targetCard.currentColumn >= columnCount + 3 && sourceColumn.Count == sourceIndex + 1 && sourceCard.value - 1 == targetCard.value && targetCard.suite == sourceCard.suite)
            {
                Debug.Log("Moving card from game column to card in victory column");
                GameObject tempCard = sourceColumn[sourceColumn.Count - 1];
                tempCard.GetComponent<Card>().currentColumn = targetCard.currentColumn;
                sourceColumn.RemoveAt(sourceColumn.Count - 1);
                targetColumn.Add(tempCard);
                // clear card selections
                sourceRenderer.color = Color.white;
                sourceObj = null;
                targetObj = null;
                // refresh display
                DisplayGameState();
            }
            // CASE: the card is in the game columns
            else if (targetCard.currentColumn < columnCount)
            {
                // CASE: selected a new selectable card
                Debug.Log("Selected a target card");
                // CASE: both cards are red
                if ((sourceCard.suite == "H" || sourceCard.suite == "D") && (targetCard.suite == "H" || targetCard.suite == "D"))
                {
                    Debug.Log("Both cards cannot be black");
                    return;
                }
                // CASE: both cards are black
                else if ((sourceCard.suite == "S" || sourceCard.suite == "C") && (targetCard.suite == "S" || targetCard.suite == "C"))
                {
                    Debug.Log("Both cards cannot be black");
                    return;
                }
                // CASE: both cards are different colors
                else
                {
                    // CASE: source card value is 1 less than target card value
                    if (sourceCard.value + 1 == targetCard.value)
                    {
                        Debug.Log("Both cards have acceptable values");
                        // move copy of selected cards to temp column
                        List<GameObject> tempColumn = sourceColumn.GetRange(sourceIndex, sourceColumn.Count - sourceIndex);
                        // update all column vars
                        foreach (GameObject obj in tempColumn)
                        {
                            obj.GetComponent<Card>().currentColumn = targetCard.currentColumn;
                        }
                        //clean up and move
                        sourceColumn.RemoveRange(sourceIndex, sourceColumn.Count - sourceIndex);
                        targetColumn.AddRange(tempColumn);
                        // clear card selections
                        sourceRenderer.color = Color.white;
                        sourceObj = null;
                        targetObj = null;
                        // refresh display
                        DisplayGameState();
                    }
                    // CASE: source card value is not within 1 of target value
                    else
                    {
                        Debug.Log("Both cards do not have acceptable values");
                        return;
                    }
                }
            }
        }
    }


    // draws a card from the draw pile and adds it to the active draw pile, moves current drawn cards to discard pile
    void DrawCard(GameObject hitObj)
    {
        // get each column for draw/discard handling
        List<GameObject> drawColumn = columns[columnCount];
        List<GameObject> activeDrawColumn = columns[columnCount + 1];
        List<GameObject> discardColumn = columns[columnCount + 2];

        // CASE: click on draw card and stack is full
        discardColumn.AddRange(activeDrawColumn.GetRange(0, activeDrawColumn.Count));
        activeDrawColumn.RemoveRange(0, activeDrawColumn.Count);
        if (drawColumn.Count > 4)
        {
            activeDrawColumn.AddRange(drawColumn.GetRange(drawColumn.Count - 4, 3));
            drawColumn.RemoveRange(drawColumn.Count - 4, 3);
        }
        // CASE: click on draw card and draw column on last 3 or less cards
        else
        {
            activeDrawColumn.AddRange(drawColumn.GetRange(1, drawColumn.Count - 1));
            drawColumn.RemoveRange(1, drawColumn.Count - 1);
        }
        foreach (GameObject obj in drawColumn)
        {
            Card objCard = obj.GetComponent<Card>();
            if (objCard != null)
            {
                objCard.currentColumn = columnCount;
            }
        }
        foreach (GameObject obj in activeDrawColumn)
        {
            Card objCard = obj.GetComponent<Card>();
            if (objCard != null)
            {
                objCard.currentColumn = columnCount + 1;
            }
        }
        foreach (GameObject obj in discardColumn)
        {
            Card objCard = obj.GetComponent<Card>();
            if (objCard != null)
            {
                objCard.currentColumn = columnCount + 3;
            }
        }
    }


    // resets the draw pile if its root is clicked
    void RefillDrawPile(GameObject hitObj)
    {
        // get each column for draw/discard handling
        List<GameObject> drawColumn = columns[columnCount];
        List<GameObject> activeDrawColumn = columns[columnCount + 1];
        List<GameObject> discardColumn = columns[columnCount + 2];
        // reset draw/active/discard piles
        discardColumn.AddRange(activeDrawColumn.GetRange(0, activeDrawColumn.Count));
        activeDrawColumn.RemoveRange(0, activeDrawColumn.Count);
        drawColumn.AddRange(discardColumn.GetRange(0, discardColumn.Count));
        discardColumn.RemoveRange(0, discardColumn.Count);
        foreach (GameObject obj in drawColumn)
        {
            Card objCard = obj.GetComponent<Card>();
            if (objCard != null)
            {
                objCard.currentColumn = columnCount;
            }
        }
        foreach (GameObject obj in activeDrawColumn)
        {
            Card objCard = obj.GetComponent<Card>();
            if (objCard != null)
            {
                objCard.currentColumn = columnCount + 1;
            }
        }
        foreach (GameObject obj in discardColumn)
        {
            Card objCard = obj.GetComponent<Card>();
            if (objCard != null)
            {
                objCard.currentColumn = columnCount + 3;
            }
        }
        // refresh display
        DisplayGameState();
    }


    // creates a card deck string data based on suites/deck size
    public void CreateDeck()
    {
        for (var i = 0; i < deckSize / suites.Length; i++)
        {
            for (var j = 0; j < suites.Length; j++)
            {
                string cardData = suites[j] + (i % (deckSize / suites.Length) + 1).ToString();
                deck.Add(cardData);
            }
        }
        // deck shuffle (Fisher-Yates)
        for (var i = deck.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);
            string temp = deck[i];
            deck[i] = deck[rnd];
            deck[rnd] = temp;
        }
    }


    // sets up Cards and columns using string data from card deck
    public void NewGameState(List<string> deck)
    {
        // create columns (7 for game columns, 3 for draw/active/discard, 4 for victory piles)
        for (var i = 0; i < columnCount + 3 + 4; i++)
        {
            columns.Add(new List<GameObject>());
        }
        // add column roots for game columns
        for (var i = 0; i <= columnCount; i++)
        {
            GameObject newRoot = Instantiate(rootPrefab);
            newRoot.name = "Root" + i.ToString();
            newRoot.GetComponent<ColumnRoot>().currentColumn = i;
            columns[i].Add(newRoot);
        }
        // add column roots for victory piles
        for (var i = 0; i < suites.Length; i++)
        {
            GameObject newRoot = Instantiate(rootPrefab);
            newRoot.name = "Root" + suites[i];
            newRoot.GetComponent<ColumnRoot>().currentColumn = columnCount + 3 + i;
            columns[columnCount + 3 + i].Add(newRoot);
        }
        // create and add Cards to each column
        int cardNum = 0;
        for (var i = 0; i < columnCount; i++)
        {
            for(var j = 0; j <= i; j++)
            {
                GameObject newCard = Instantiate(cardPrefab);
                newCard.name = deck[cardNum];
                newCard.GetComponent<Card>().SetupCard(deck[cardNum], cardTheme, i);
                columns[i].Add(newCard);
                cardNum++;
            }
        }
        // add remaining cards to draw pile
        for (var i = cardNum; i < deckSize; i++)
        {
            GameObject newCard = Instantiate(cardPrefab);
            newCard.name = deck[cardNum];
            newCard.GetComponent<Card>().SetupCard(deck[cardNum], cardTheme, columnCount);
            columns[columnCount].Add(newCard);
            cardNum++;
        }
    }


    // updates display based on game state
    public void DisplayGameState()
    {
        for (var i = 0; i < columnCount; i++)
        {
            // set card x position
            float xpos = -7.5f + i * 2.5f;
            // set card y position
            float yposPrev = -0.5f;
            float yposOffsetVisible = -0.5f;
            float yposOffsetNotVisible= -0.25f;
            float ypos;

            // set root positions
            columns[i][0].transform.position = new Vector3(xpos, yposPrev + yposOffsetNotVisible, 0);

            // set card positions
            for (var j = 1; j < columns[i].Count; j++)
            {
                // first visible card
                if (columns[i][j-1].GetComponent<ColumnRoot>() != null || columns[i][j - 1].GetComponent<Card>().isVisible == false)
                {
                    ypos = yposPrev + yposOffsetNotVisible;
                }
                // visible cards
                else if (columns[i][j].GetComponent<Card>().isVisible)
                {
                    ypos = yposPrev + yposOffsetVisible;
                }
                // hidden cards
                else
                {
                    ypos = yposPrev + yposOffsetNotVisible;
                }
                yposPrev = ypos;

                // set card z position
                float zpos = 0.0f - j * 0.1f;
                columns[i][j].transform.position = new Vector3(xpos, ypos, zpos);
            }

            // set visibility and selectability of last card
            Card lastCard = columns[i][columns[i].Count - 1].GetComponent<Card>();
            if (lastCard != null && lastCard.isVisible == false)
            {
                lastCard.ToggleVisibility();
            }
            if (lastCard != null && lastCard.isSelectable == false)
            {
                lastCard.ToggleSelectable();
            }
        }

        // place draw pile (col 7)
        List<GameObject> drawPile = columns[columnCount];
        for (var i = 0; i < drawPile.Count; i++)
        {
            float xpos = -7.5f;
            float ypos = 3.0f;
            float zpos = 0.0f - i * 0.1f;
            drawPile[i].transform.position = new Vector3(xpos, ypos, zpos);
            Card drawPileCard = drawPile[i].GetComponent<Card>();
            if (drawPileCard != null && drawPileCard.isVisible == true)
            {
                drawPileCard.ToggleVisibility();
            }
        }
        // make top draw card selectable
        Card topDrawCard = drawPile[drawPile.Count - 1].GetComponent<Card>();
        if (topDrawCard != null && topDrawCard.isSelectable == false)
        {
            topDrawCard.ToggleSelectable();
        }

        // place active draw pile (col 8)
        List<GameObject> activeDrawPile = columns[columnCount + 1];
        for (var i = 0; i < activeDrawPile.Count; i++)
        {
            float xpos = -5.0f + i * 0.35f;
            float ypos = 3.0f;
            float zpos = 0.0f - i * 0.1f;
            activeDrawPile[i].transform.position = new Vector3(xpos, ypos, zpos);
            Card activeDrawPileCard = activeDrawPile[i].GetComponent<Card>();
            if (activeDrawPileCard != null && activeDrawPileCard.isVisible == false)
            {
                activeDrawPileCard.ToggleVisibility();
            }
        }
        // make top active draw card selectable
        if (activeDrawPile.Count > 0)
        {
            Card topActiveDrawCard = activeDrawPile[activeDrawPile.Count - 1].GetComponent<Card>();
            if (topActiveDrawCard.isSelectable == false)
            {
                topActiveDrawCard.ToggleSelectable();
            }
        }
        
        // move discarded cards
        List<GameObject> discardPile = columns[columnCount + 2];
        if (discardPile.Count > 0)
        {
            foreach (GameObject obj in discardPile)
            {
                obj.transform.position = new Vector3(50, 50, 50);
            }
        }

        // place victory piles
        List<GameObject> victorySpades = columns[columnCount + 3];
        for (var i = 0; i < victorySpades.Count; i++)
        {
            float xpos = 0.0f;
            float ypos = 3.0f;
            float zpos = 0.0f - i * 0.1f;
            victorySpades[i].transform.position = new Vector3(xpos, ypos, zpos);
        }
        List<GameObject> victoryHearts = columns[columnCount + 4];
        for (var i = 0; i < victoryHearts.Count; i++)
        {
            float xpos = 2.5f;
            float ypos = 3.0f;
            float zpos = 0.0f - i * 0.1f;
            victoryHearts[i].transform.position = new Vector3(xpos, ypos, zpos);
        }
        List<GameObject> victoryClubs = columns[columnCount + 5];
        for (var i = 0; i < victoryClubs.Count; i++)
        {
            float xpos = 5.0f;
            float ypos = 3.0f;
            float zpos = 0.0f - i * 0.1f;
            victoryClubs[i].transform.position = new Vector3(xpos, ypos, zpos);
        }
        List<GameObject> victoryDiamonds = columns[columnCount + 6];
        for (var i = 0; i < victoryDiamonds.Count; i++)
        {
            float xpos = 7.5f;
            float ypos = 3.0f;
            float zpos = 0.0f - i * 0.1f;
            victoryDiamonds[i].transform.position = new Vector3(xpos, ypos, zpos);
        }
    }
}
