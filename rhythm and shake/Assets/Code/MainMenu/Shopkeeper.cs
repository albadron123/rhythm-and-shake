using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Net;
using System;
using System.IO;
using System.Net.Http;
using System.ComponentModel;

/// TODO:
/// * Correct to understand when server sends wav and when mpeg (NOTE: mp3 is not supported w/t unity basic Audio things
/// * Assume that downloaded tracks can be deleted and/or save them in a secure-folder (now we assume that player wouldn't delete tracks)
/// * Save downloaded tracks in player prefs (as TD is not persistent to restart)
/// * Request https page (first,and that http page???)
/// * Sell tracks from editor
/// * in FinishDownload function the renaming is due to the fact that tracks are found in the file system by the name of tracks. Should redo it later and then clean up the FinishDownload
/// 


public class Shopkeeper : MonoBehaviour
{
    [SerializeField]
    LevelSelection ls;
    [SerializeField]
    TransportedData td;

    [SerializeField]
    GameObject[] downloadEntries = new GameObject[3];
    [SerializeField]
    GameObject[] downloadedSigns = new GameObject[3];
    [SerializeField]
    TMPro.TMP_Text[] downloadTexts = new TMPro.TMP_Text[3];

    int[] fixed_prices = {500, 750, 1250};

    [SerializeField]
    bool[] bought = new bool[3] { false, false, false };
    int downloadingId;


    UnityWebRequestAsyncOperation jsonAsyncOperation = null;
    UnityWebRequestAsyncOperation audioAsyncOperation = null;
    string getSongJsonRequest = "http://rhythmandshake.online/request/getsongjson.php";
    string getSongAudioRequest = "http://rhythmandshake.online/request/getsongaudio.php";


    [SerializeField]
    TMPro.TMP_Text dialogueText;

    [SerializeField]
    GameObject sellingScreen;
    [SerializeField]
    GameObject buyingScreen;
    [SerializeField]
    GameObject welcomeScreen;
    [SerializeField]
    GameObject loadingScreen;

    [SerializeField]
    Transform loadingTransform;

    string[] buyingText = new string[] {
        "Пока ты думаешь, зачем зашел в мой магазин, скажу, что сегодня наш офис сделал  на 3 плаката больше, чем всегда. Им выдали премию и увеличили норму на 3 плаката.",
        "Не приходи больше ко мне в однотонной футболке - ты выглядишь слишком бунтарски по-сравнению с моими будничными неоновыми звездами.",
        "Не забывай, что музыки на моих дисках как-бы не существует. Если тебя спросят - ты делаешь из них смузи или украшаешь ими дом.",
        "Забавно, нас разделяют миллиарды световых витков, но мои приятели смогли найти тебя по привязке плеера к гугл-аккаунту. ",
        "Я кстати устроился в наш офис маркетологом. Мы, конечно, делаем только гос заказы, но маркетологи все равно получают зарплату больше, чем от моих дисков",
        };
    string[] objectSelectText = new string[] {
        "Я отыскал этот диск на помойке, отмыл и теперь продаю", 
        "Это товар премиум-семента! Не трогай ручками!",
        "Сегодня в офисном шредере я нашел осколки, и когда склеил их, увидел, что это диск. Точно в единственном экземпляре, и совсем недорого!",
        "Я подрался с роботом-мусорщиком за гору хлама, в которой нашёл этот бриллиант. Отдаю тебе его почти даром, найди ему достойное применение",
        "Ко мне приехал брат из из соседней галактики, и в качестве сувенира привез старый телевизор с DVD-кассетой внутри. Хочешь купить трек с не?",
         };
    string[] welcomeText = new string[] { 
        "Привет! Здесь ты найдешь треки на любой вкус!",
        "Сегодня мои диски назвали подставкой для чая, так что быстрее говори, зачем пришел, я не в настроении",
        "Привет, путник! Между прочим, ты сейчас в самом крутом магазине галактики - мне даже выдали грант на развитие бизнеса",
        "Привет! Зацени мою шапку - урвал на распродаже галактического мусора!",
        "Я все никак не пойму, ты такой большой любитель музыки или просто скучал по моей харизме? Так или иначе, привет!"};
    string[] sellRestricted = new string[] {
        "В прошлый раз ты продал мне кусок стекляшки вместо нормального диска - целая семья слаймов осталась без завтрака! Больше я ничего у тебя не куплю",
        "От вибраций на твоем прошлом диске у меня заболела голова. Ты это специально? В ближайшие дни даже не пытайся мне что-то продать",
        "В твоем прошлом треке выбрации шли с запозданием в несколько временных витков. Ребята из офиса не хотят такое покупать, значит и я не буду. Переделай и приходи в следующий раз",
        "Своим прошлым диском ты подорвал мою репутацию, он был некачественным! Я у тебя не буду ничего покупать, пока ты не придумаешь, как все исправить"
        };
    string[] boughtLines = new string[]
    {
        "Слушай, ты купил самый популярный мой диск! Именно такие берут муравьеды с соседнего астероида. Кажется, они используют их как подушку.",
        "Поздравляю с покупкой! Кстати, если ты принесешь мне банку растворимого кофе, в следующий раз я сделаю тебе скидку",
        "Честно говоря, я и не думал продать этот диск - для моих постоянных клиентов он жестковат",
        "Приятного прослушивания! Я поставляю новые диски каждый временной виток, так что заглядывай за новым в любое время!",
        "Вибрации от этого диска заставляют танцевать даже меня. Кстати, могу показать!"
        };
    string[] sellingText = new string[] { "Хм.. я могу попробовать взять это... если продам, то отдам деньги X)" };
    string[] soldText = new string[] { "Продано! Возвратов не принимаем!", "А у вас хороший музыкальных вкус!" };

    string[] freeTracks = new string[] 
    { 
        "Слушай, в честь 53 века со дня нового витка нашей цивилизации, я провожу акцию - держи этот диск за 0 денег!",
        "Помнишь медузу-клептоманку? Она принесла мне этот диск, и брать за него деньги нечестно. Забирай просто так!",
        "У меня сегодня день рождения! И я, конечно, рискую остаться без денег, но забирай диск просто так, в честь праздника!",
        "Сегодня ко мне зашла покупательница и швырнула в меня диском, сказав, что он несвежий. А ему всего 60 световых витков! Но раз так вышло, может, ты заберешь?",
        "Вибрации на этом диске слишком сложные для меня. Давай сделку - ты мне их расшифруешь, а я его тебе подарю?"
    };

    string[] youHaveNoMoney = new string[]
    {
        "У тебя недостаточно осколков, чтобы купить этот диск. Осколки важны! За осколки я вымениваю новые диски у рогатых слаймов! Нет осколков - нет дисков!",
        "Ты думаешь, что я отдам тебе диск просто так? Не дождешься!",
        "Просто так не отдам!"
    };


    public enum ShopState { welcome, buying, selling };
    public static ShopState shopState = ShopState.welcome;


    public void DownloadSongCompleted(UnityEngine.AsyncOperation op)
    {
        //Check if download fails in here!!!!!!!!!!!!!
        //Check if download fails in here!!!!!!!!!!!!!
        //Check if download fails in here!!!!!!!!!!!!!
        //Check if download fails in here!!!!!!!!!!!!!
        //Check if download fails in here!!!!!!!!!!!!!
        if (!jsonAsyncOperation.isDone || !audioAsyncOperation.isDone)
            return;
        Debug.Log("Completed!");
        File.WriteAllBytes(Application.persistentDataPath + "/songs/shop-track" + downloadingId + ".mp3", audioAsyncOperation.webRequest.downloadHandler.nativeData.ToArray());
        File.WriteAllBytes(Application.persistentDataPath + "/shop-track" + downloadingId + ".json", jsonAsyncOperation.webRequest.downloadHandler.nativeData.ToArray());
        Debug.Log("Completed2!");
        StartCoroutine(FinishDownload("shop-track" + downloadingId, AudioType.MPEG));
    }
    
    IEnumerator FinishDownload(string fileName, AudioType audioType)
    {
        using (UnityWebRequest www1 = UnityWebRequestMultimedia.GetAudioClip("file://" + Application.persistentDataPath + "/songs/" + fileName + ".mp3", audioType))
        {
            yield return www1.SendWebRequest();

            if (www1.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www1.error);
            }
            else
            {
                Song song = new Song();
                MyUtitities.LoadSongFromJSON(ref song, jsonAsyncOperation.webRequest.downloadHandler.text);
                song.audio = DownloadHandlerAudioClip.GetContent(www1);
                ls.AddCustomSong(song, fileName);

                AudioInfo ai = new AudioInfo();
                ai.name = fileName + ".mp3";
                ai.clip = DownloadHandlerAudioClip.GetContent(www1);

                JsonInfo ji = new JsonInfo();
                ji.name = fileName + ".json";
                ji.json = jsonAsyncOperation.webRequest.downloadHandler.text;

                File.Move(Application.persistentDataPath + "/songs/" + ai.name, Application.persistentDataPath + "/songs/" + song.songName + ".mp3"); 
                File.Move(Application.persistentDataPath + "/" + ji.name, Application.persistentDataPath + "/" + song.songName + ".json"); 

                td.detectedAudioFiles.Add(ai);
                td.detectedJsonFiles.Add(ji);
                Debug.Log("Completed3!");
                TransportedData.downloaded[downloadingId-1] = true;
                downloadingId = -1;
                CloseAllScreenObjects();
                OpenBuyingScreen();
            }
        }
    }
    

    private UnityWebRequestAsyncOperation GetSongDataAsync(string request, int songID)
    {
        WWWForm requestForm = new WWWForm();
        requestForm.AddField("songID", songID.ToString());
        UnityWebRequest www = UnityWebRequest.Post(request, requestForm);
        return www.SendWebRequest();
    }

    void DownloadTrack(int trackID)
    {
        jsonAsyncOperation = GetSongDataAsync(getSongJsonRequest, trackID);
        jsonAsyncOperation.completed += new Action<UnityEngine.AsyncOperation>(DownloadSongCompleted);
        audioAsyncOperation = GetSongDataAsync(getSongAudioRequest, trackID);
        audioAsyncOperation.completed += new Action<UnityEngine.AsyncOperation>(DownloadSongCompleted);
    }

    public void DownloadTrackButton(int trackID)
    {
        if (!bought[trackID - 1])
        {
            if (ScoringSystem.GetScore() >= fixed_prices[trackID - 1])
            {
                ScoringSystem.SetScore(-fixed_prices[trackID - 1]);
                downloadTexts[trackID - 1].text = "load";
                bought[trackID - 1] = true;
                downloadingId = trackID;
                CloseAllScreenObjects();
                loadingScreen.SetActive(true);
                UpdateLine(boughtLines[UnityEngine.Random.Range(0, boughtLines.Length)]);
                DownloadTrack(trackID);
            }
            else
            {
                if (UnityEngine.Random.value < 0.9)
                {
                    UpdateLine(youHaveNoMoney[UnityEngine.Random.Range(0, youHaveNoMoney.Length)]);
                }
                else
                {
                    downloadTexts[trackID - 1].text = "load";
                    bought[trackID - 1] = true;
                    downloadingId = trackID;
                    CloseAllScreenObjects();
                    loadingScreen.SetActive(true);
                    UpdateLine(freeTracks[UnityEngine.Random.Range(0, freeTracks.Length)]);
                    DownloadTrack(trackID);
                }
            }
        }
        else if (bought[trackID - 1])
        {
            CloseAllScreenObjects();
            loadingScreen.SetActive(true);
            UpdateLine(boughtLines[UnityEngine.Random.Range(0, boughtLines.Length)]);
            DownloadTrack(trackID);
        }
    }


    void Start()
    {
        //DownloadTrack(1);
    }

    void Update()
    {
        if (loadingScreen.activeSelf == true)
        {
            if (jsonAsyncOperation != null && audioAsyncOperation != null)
            {
                if (!(jsonAsyncOperation.isDone && audioAsyncOperation.isDone))
                    loadingTransform.localScale = new Vector3(audioAsyncOperation.progress, 1, 1);
            }
        }
    }


    void UpdateLine(string text)
    {
        dialogueText.text = text;
    }

    public void SelectTrackToUpdateLine()
    {

        UpdateLine(objectSelectText[UnityEngine.Random.Range(0, objectSelectText.Length)]);
    }

    public void SelectSellWhenRestricted()
    {
        UpdateLine(sellRestricted[UnityEngine.Random.Range(0, sellRestricted.Length)]);
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
        for (int i = 0; i < 3; ++i)
        {
            downloadEntries[i].SetActive(!TransportedData.downloaded[i]);
            downloadedSigns[i].SetActive(TransportedData.downloaded[i]);
        }
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
        loadingScreen.SetActive(false);
    }
}
