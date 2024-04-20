using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System;
using System.ComponentModel;

public class Shopkeeper : MonoBehaviour
{
    string filename = "FW7API9F8RPG7DP.jpg";
    string uri = "https://content.instructables.com/FW7/API9/F8RPG7DP/";


    [SerializeField]
    TMPro.TMP_Text dialogueText;

    [SerializeField]
    GameObject sellingScreen;
    [SerializeField]
    GameObject buyingScreen;
    [SerializeField]
    GameObject welcomeScreen;

    string[] buyingText = new string[] { "Купи этот диск!", "Я отыскал этот диск на помойке, отмыл и теперь продаю", "Это товар премиум-семента! Не трогай ручками!" };
    string[] welcomeText = new string[] { "Добро пожаловать!\nЗдесь есть треки на любой вкус", "Вам что-нибудь подсказать? У нас вчера был завоз." };
    string[] sellingText = new string[] { "Хм.. я могу попробовать взять это... если продам, то отдам деньги X)" };
    string[] soldText = new string[] { "Продано! Возвратов не принимаем!", "А у вас хороший музыкальных вкус!" };

    public enum ShopState { welcome, buying, selling };
    public static ShopState shopState = ShopState.welcome;


    public AsyncCompletedEventHandler DownloadFileCompleted()
    {
        Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
        {
            if (e.Error != null)
            {
                throw e.Error;
                Debug.Log("ERROR");
            }
            else
            {
                Debug.Log("SUCCESS");
            }
        };
        return new AsyncCompletedEventHandler(action);
    }


    void Start()
    {
        WebClient myWebClient = new WebClient();
        // Concatenate the domain with the Web resource filename.
        string myStringWebResource = uri + filename;

        myWebClient.DownloadFileCompleted += DownloadFileCompleted();
        //Console.WriteLine("Downloading File \"{0}\" from \"{1}\" .......\n\n", fileName, myStringWebResource);
        // Download the Web resource and save it into the current filesystem folder.
        myWebClient.DownloadFileAsync(new Uri(myStringWebResource), filename);
    }

    void Update()
    {

    }


    void UpdateLine(string text)
    {
        dialogueText.text = text;
    }

    public void OpenShopWithCurrentState()
    {
        switch (shopState)
        {
            case ShopState.welcome: OpenWelcomeScreen(); break;
            case ShopState.buying: OpenBuyingScreen(); break;
            case ShopState.selling: OpenSellingScreen(); break;
        }
    }

    public void OpenWelcomeScreen()
    {
        shopState = ShopState.welcome;
        UpdateLine(welcomeText[UnityEngine.Random.Range(0, welcomeText.Length)]);
        CloseAllScreenObjects();
        welcomeScreen.SetActive(true);
    }

    public void OpenBuyingScreen()
    {
        shopState = ShopState.buying;
        UpdateLine(buyingText[UnityEngine.Random.Range(0, buyingText.Length)]);
        CloseAllScreenObjects();
        buyingScreen.SetActive(true);
    }

    public void OpenSellingScreen()
    {
        shopState = ShopState.selling;
        UpdateLine(sellingText[UnityEngine.Random.Range(0, sellingText.Length)]);
        CloseAllScreenObjects();
        sellingScreen.SetActive(true);
    }

    public void CloseAllScreenObjects()
    {
        welcomeScreen.SetActive(false);
        buyingScreen.SetActive(false);
        sellingScreen.SetActive(false);
    }
}
