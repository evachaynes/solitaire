using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Card : MonoBehaviour
{
    public int currentColumn; // card columns = 0-6, draw = 7, activedraw = 8, discard = 9, victory = 10-13
    public string suite;
    public int value;
    public string cardTheme;
    private Sprite[] cardSprites;
    public Sprite cardFront;
    public Sprite cardBack;
    public bool isSelectable = false;
    public bool isVisible = false;

    public void SetupCard(string cardData, string theme, int column)
    {
        currentColumn = column;
        suite = cardData[0].ToString();
        value = Int32.Parse(cardData.Remove(0, 1));
        cardTheme = theme;
        cardSprites = SetCardTheme(cardTheme);
        cardFront = cardSprites[0];
        cardBack = cardSprites[1];
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        spriteRenderer.sprite = cardBack;
        collider.size = spriteRenderer.bounds.size; 
    }

    public void ToggleVisibility()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (isVisible == true)
        {
            spriteRenderer.sprite = cardBack;
            isVisible = false;
        }
        else
        {
            spriteRenderer.sprite = cardFront;
            isVisible = true;
        }
    }

    public void ToggleSelectable()
    {
        if (isSelectable == true)
        {
            isSelectable = false;
        }
        else
        {
            isSelectable = true;
        }
    }

    public Sprite[] SetCardTheme(string cardTheme)
    {
        string cardFrontLookup = "";
        // get value string
        if (value == 1)
        {
            cardFrontLookup += "ace_of_";
        }
        if (value == 2)
        {
            cardFrontLookup += "2_of_";
        }
        if (value == 3)
        {
            cardFrontLookup += "3_of_";
        }
        if (value == 4)
        {
            cardFrontLookup += "4_of_";
        }
        if (value == 5)
        {
            cardFrontLookup += "5_of_";
        }
        if (value == 6)
        {
            cardFrontLookup += "6_of_";
        }
        if (value == 7)
        {
            cardFrontLookup += "7_of_";
        }
        if (value == 8)
        {
            cardFrontLookup += "8_of_";
        }
        if (value == 9)
        {
            cardFrontLookup += "9_of_";
        }
        if (value == 10)
        {
            cardFrontLookup += "10_of_";
        }
        if (value == 11)
        {
            cardFrontLookup += "jack_of_";
        }
        if (value == 12)
        {
            cardFrontLookup += "queen_of_";
        }
        if (value == 13)
        {
            cardFrontLookup += "king_of_";
        }
        // get suite string
        if (suite == "S")
        {
            cardFrontLookup += "spades";
        }
        if (suite == "H")
        {
            cardFrontLookup += "hearts";
        }
        if (suite == "C")
        {
            cardFrontLookup += "clubs";
        }
        if (suite == "D")
        {
            cardFrontLookup += "diamonds";
        }
        Sprite frontSprite = Resources.Load<Sprite>(cardTheme + "/" + cardFrontLookup);
        Sprite backSprite = Resources.Load<Sprite>(cardTheme + "/" + "card_back");
        return new Sprite[] { frontSprite, backSprite };
    }
}
