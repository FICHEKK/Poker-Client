using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dealer : MonoBehaviour {

    public TextMeshProUGUI bestHandText;

    public Image flopCard1;
    public Image flopCard2;
    public Image flopCard3;
    public Image turnCard;
    public Image riverCard;
    
    public Image handCard1;
    public Image handCard2;
    
    void Start() {
        for (int i = 0; i < 20; i++) {
            Deck.Shuffle();

            Card c0 = Deck.GetNextCard();
            Card c1 = Deck.GetNextCard();
            Card c2 = Deck.GetNextCard();
            Card c3 = Deck.GetNextCard();
            Card c4 = Deck.GetNextCard();
            Card c5 = Deck.GetNextCard();
            Card c6 = Deck.GetNextCard();
            
            SevenCardEvaluator evaluator = new SevenCardEvaluator(c0, c1, c2, c3, c4, c5, c6);
            Debug.Log(c0 + " " + c1 + " " + c2  + " " + c3  + " " + c4  + " " + c5  + " " + c6 + " -> " + evaluator.BestHand);
        }
    }

    public void DealHand() {
        Deck.Shuffle();

        Card hc1 = Deck.GetNextCard();
        Card hc2 = Deck.GetNextCard();
        
        Card fc1 = Deck.GetNextCard();
        Card fc2 = Deck.GetNextCard();
        Card fc3 = Deck.GetNextCard();
        Card tc = Deck.GetNextCard();
        Card rc = Deck.GetNextCard();

        handCard1.sprite = LoadSprite(@"Assets\Graphics\Cards\" + hc1 + ".png");
        handCard2.sprite = LoadSprite(@"Assets\Graphics\Cards\" + hc2 + ".png");
        
        flopCard1.sprite = LoadSprite(@"Assets\Graphics\Cards\" + fc1 + ".png");
        flopCard2.sprite = LoadSprite(@"Assets\Graphics\Cards\" + fc2 + ".png");
        flopCard3.sprite = LoadSprite(@"Assets\Graphics\Cards\" + fc3 + ".png");
        turnCard.sprite = LoadSprite(@"Assets\Graphics\Cards\" + tc + ".png");
        riverCard.sprite = LoadSprite(@"Assets\Graphics\Cards\" + rc + ".png");
        
        SevenCardEvaluator evaluator = new SevenCardEvaluator(hc1, hc2, fc1, fc2, fc3, tc, rc);
        bestHandText.text = evaluator.BestHand.HandAnalyser.HandValue.ToString();
    }

    private static Sprite LoadSprite(string filePath) {
        Texture2D tex = LoadPNG(filePath);
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
    
    public static Texture2D LoadPNG(string filePath) {
 
        Texture2D tex = null;
        byte[] fileData;
 
        if (File.Exists(filePath))     {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
