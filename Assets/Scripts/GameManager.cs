using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public float PlayerSpeed;
    private GameObject Player;
    private int TotalLives;
    private float TotalSeconds;

    [Header("World")]
    public Vector2 LimitStage;
    public Transform GridWorld;
    public GameObject BaseWorld;
    public GameObject GeneralCanvas;
    private WorldCreator.WorldProperties CurrentWorld;
    public GameObject PauseMenu;
    public GameObject UiGame;
    public Text LivesTxt;
    public Text TimeTxt;
    public GameObject _resetScreen;

    [Header("Enemy")]
    public Vector2 StartEnemyPosition;
    private List<EnemyProperties> SavedEnemies;
    private float EnemySpeed;
    private float TotalEnemySpeed;


    public enum EnemyDirection { RIGHT, LEFT }
    private EnemyDirection Direction;

    public class EnemyProperties
    {
        public GameObject EnemyObj;
        public EnemyCreator.EnemyProperties Enemy;

        public EnemyProperties(GameObject _obj, EnemyCreator.EnemyProperties _enemy)
        {
            // Zuweisung der GOs an Parameter
            EnemyObj = _obj;
            Enemy = _enemy;
        }

    }


    void Start()
    {
        PrintMenu();
        PauseMenu.SetActive(false);
        _resetScreen.SetActive(false);
    }


    void Update()
    {
        if (Player != null)
        {
            PlayerMovement();
            if (SavedEnemies.Count > 0)
            {
                EnemyMovement();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 0;
                PauseMenu.SetActive(true);

                //ClearStage();
            }

            if (Time.timeScale == 0)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                     ClearStage();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                     Time.timeScale = 1;
                     PauseMenu.SetActive(false);
                }
            }

            TotalSeconds -= Time.deltaTime;
            if (TotalSeconds <= 0)
            {
                print("Time Out");
                ClearStage();
            }
        }

        // UI In Game
        LivesTxt.text = "Lives: " + TotalLives;                                                             // hier braucht man kein getComponent<Text>.text
        TimeTxt.text = ((int)TotalSeconds).ToString();
    }

    // Menü erstellen
    void PrintMenu()
    {
        // "Löschen" des Menüs bei erneutem Spielen                                                     // Grund: Menü wird dupliziert
        for (int i = GridWorld.childCount - 1; i >= 0; i--)
        {
            Destroy(GridWorld.GetChild(i).gameObject);
        }

        GeneralCanvas.SetActive(true);                                                                  // Unity -> GameManager - Header: World - General Canvas = Canvas

        for (int i = 0; i < WorldCreator.Worlds.Count; i++)
        {
            GameObject NewWorld = Instantiate(BaseWorld, GridWorld);
            NewWorld.transform.Find("Name").GetComponent<Text>().text = WorldCreator.Worlds[i].Name;
            NewWorld.transform.Find("Lives").GetComponent<Text>().text = "Lives: " + WorldCreator.Worlds[i].Lives;
            NewWorld.transform.Find("Seconds").GetComponent<Text>().text = "Seconds: " + WorldCreator.Worlds[i].Seconds;
            NewWorld.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Worlds/" + WorldCreator.Worlds[i].ID);

            int TempId = WorldCreator.Worlds[i].ID;
            NewWorld.GetComponent<Button>().onClick.AddListener(delegate { PrintStage(TempId); });

            // Vergleich der Sterne
            int TotalStars = 0;
            if (PlayerPrefs.HasKey("World_" + WorldCreator.Worlds[i].ID) == true)
            {
                TotalStars = PlayerPrefs.GetInt("World_" + WorldCreator.Worlds[i].ID);
            }
            NewWorld.transform.Find("Stars").GetComponent<Text>().text = "Stars: " + TotalStars + " / 3";


            //int TotalStars = PlayerPrefs.GetInt("World_" + WorldCreator.Worlds[i].ID);
        }
    }

    // Erzeugen aller Objekte aufm Spielbildschrim
    void PrintStage(int _idWorld)
    {
        // Menü "loswerden"
        GeneralCanvas.SetActive(false);
        UiGame.SetActive(true);
        _resetScreen.SetActive(false);


        // Konfiguration World
        CurrentWorld = WorldCreator.GetWorldByID(_idWorld);
        TotalLives = CurrentWorld.Lives;                                                                // Referenzierung der Leben des Spielers mit der aktuellen Spielwelt
        TotalSeconds = CurrentWorld.Seconds;


        // Konfiguration Player
        Player = GameObject.CreatePrimitive(PrimitiveType.Quad);                                        // Erstellung eines primitiven GOs als Player
        Player.name = "Player";                                                                         // Instanzierung (0, 0, 0) und Benennung des GOs als Player
        Player.transform.position = new Vector2(0, -7);                                                 // Instanzierung des Player mit den Koordinaten 0, -7
        Destroy(Player.GetComponent<MeshCollider>());                                                   // Eleminierung des MeshColliders
        //Player.AddComponent<BoxCollider2D>();                                                         // Hinzufügen der Komponente BoxCollider2D an den Player -> dafür benötigt man einen IEnumerator (Corotine)
        StartCoroutine(AddCollider(Player));
        Player.GetComponent<Renderer>().enabled = false;
        Player.tag = "Player";                                                                          // Tag "Player" an Player hinzufügen

        GameObject SpritePlayer = new GameObject("Player Sprite");
        SpritePlayer.transform.SetParent(Player.transform);
        SpritePlayer.transform.localPosition = new Vector2(0, 0);
        SpritePlayer.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        SpritePlayer.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Player");


        // Konfiguration Enemy
        SavedEnemies = new List<EnemyProperties>();
        TotalEnemySpeed = 0.7f;


        for (int i = 0; i < CurrentWorld.EnemiesID.Count; i++)
        {
            for (int j = 0; j < CurrentWorld.Columns; j++)
            {
                GameObject NewEnemy = GameObject.CreatePrimitive(PrimitiveType.Quad);
                NewEnemy.name = "Enemy_" + CurrentWorld.EnemiesID[i];
                NewEnemy.transform.localScale = new Vector3(2f, 2f, 2f);
                NewEnemy.transform.position = new Vector2(StartEnemyPosition.x + (j * 3f), StartEnemyPosition.y - (i * 2f));
                Destroy(NewEnemy.GetComponent<MeshCollider>());
                StartCoroutine(AddCollider(NewEnemy));
                NewEnemy.GetComponent<MeshRenderer>().enabled = false;
                NewEnemy.tag = "Enemy";

                GameObject SpriteEnemy = new GameObject(NewEnemy.name);
                SpriteEnemy.transform.SetParent(NewEnemy.transform);
                SpriteEnemy.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                SpriteEnemy.transform.localPosition = new Vector2(0, 0);
                SpriteEnemy.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CurrentWorld.EnemiesID[i] + "/" + CurrentWorld.EnemiesID[i] + "_1");


                // Enemy Animation
                NewEnemy.AddComponent<AnimationControl>().InitialAnimation(SpriteEnemy.GetComponent<SpriteRenderer>(), new List<Sprite>(Resources.LoadAll<Sprite>(CurrentWorld.EnemiesID[i].ToString())), 0.4f, AnimationType.LOOP);

                // Enemy zur Liste hinzufügen
                EnemyCreator.EnemyProperties TempEnemy = EnemyCreator.GetEnemyByID(CurrentWorld.EnemiesID[i]);
                SavedEnemies.Add(new EnemyProperties(NewEnemy, TempEnemy));
            }
        }
    }

    // Löschen aller Objekte aufm Spielbildschrim
    private void ClearStage()
    {
        PauseMenu.SetActive(false);

        // Enemy
        for (int i = 0; i < SavedEnemies.Count; i++)
        {
            Destroy(SavedEnemies[i].EnemyObj);
        }
        SavedEnemies = new List<EnemyProperties>();                                             // "Entsorgung" der Enemies in einer Liste

        // Player
        Destroy(Player);

        // Bullet
        GameObject[] AllBullets = GameObject.FindGameObjectsWithTag("Bullet");                  // Array
        for (int i = 0; i < AllBullets.Length; i++)
        {
            Destroy(AllBullets[i]);
        }

        PrintMenu();

    }

    // 
    IEnumerator AddCollider(GameObject _obj)
    {
        yield return new WaitForSeconds(0.1f);
        if (_obj != null)
        {
            _obj.AddComponent<BoxCollider2D>().isTrigger = true;
            _obj.AddComponent<Rigidbody2D>().gravityScale = 0;
        }
    }


    // Playermovement
    private void PlayerMovement()
    {
        Player.transform.Translate(Vector2.right * PlayerSpeed * Input.GetAxis("Horizontal") * Time.deltaTime);             // Playerbewegung auf einer Achse links <-> rechts
        Vector2 CurrentPos = Player.transform.position;                                                                     // Grenzen des Players [Instanzierung Vector2]
        CurrentPos.x = Mathf.Clamp(CurrentPos.x, LimitStage.x, LimitStage.y);
        Player.transform.position = CurrentPos;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateBullet(BulletType.PLAYER, Player.transform.position, 12);
        }

    }

    // Enemymovement
    private void EnemyMovement()
    {
        EnemySpeed += Time.deltaTime;

        if (EnemySpeed >= TotalEnemySpeed)
        {
            for (int i = 0; i < SavedEnemies.Count; i++)
            {
                switch (Direction)
                {
                    case EnemyDirection.RIGHT:
                        SavedEnemies[i].EnemyObj.transform.position = new Vector2(SavedEnemies[i].EnemyObj.transform.position.x + 1, SavedEnemies[i].EnemyObj.transform.position.y);
                        break;
                    case EnemyDirection.LEFT:
                        SavedEnemies[i].EnemyObj.transform.position = new Vector2(SavedEnemies[i].EnemyObj.transform.position.x - 1, SavedEnemies[i].EnemyObj.transform.position.y);
                        break;
                }

                // SavedEnemies[i].transform.position = new Vector2(SavedEnemies[i].transform.position.x + 1, SavedEnemies[i].transform.position.y);  
            }


            // Enemy schießt willkürlich
            int RandomEnemy = Random.Range(0, SavedEnemies.Count);

            CreateBullet(BulletType.ENEMY, SavedEnemies[RandomEnemy].EnemyObj.transform.position, 10);

            // Grenzen prüfen
            CheckEnemyLimits();
            EnemySpeed = 0;
        }
    }

    private void MoveEnemiesDown()
    {
        TotalEnemySpeed -= 0.1f;                                                            // Gegner werden um 0.1f schneller mit jeder Linie die sie weiter nach unten gehn
        if (TotalEnemySpeed <= 0.2f)
        {
            TotalEnemySpeed = 0.2f;
        }

        for (int i = 0; i < SavedEnemies.Count; i++)
        {
            SavedEnemies[i].EnemyObj.transform.position = new Vector2(SavedEnemies[i].EnemyObj.transform.position.x, SavedEnemies[i].EnemyObj.transform.position.y - 1);

            if (SavedEnemies[i].EnemyObj.transform.position.y <= -5)
            {
                ClearStage();
                break;
            }
        }
    }

    private void CheckEnemyLimits()
    {
        for (int i = 0; i < SavedEnemies.Count; i++)
        {
            if (SavedEnemies[i].EnemyObj.transform.position.x <= LimitStage.x)
            {
                Direction = EnemyDirection.RIGHT;
                MoveEnemiesDown();
                break;
            }
            if (SavedEnemies[i].EnemyObj.transform.position.x >= LimitStage.y)
            {
                Direction = EnemyDirection.LEFT;
                MoveEnemiesDown();
                break;
            }

            // untere Spielgrenze
            if (SavedEnemies[i].EnemyObj.transform.position.y <= -6)
            {
                print("Niederlage");
                break;
            }
        }
    }

    private void CreateBullet(BulletType _type, Vector2 _pos, float _speed)
    {
        GameObject NewBullet = GameObject.CreatePrimitive(PrimitiveType.Quad);                          // Neue Bullet als Komponente (GameObjekt) erzeugen
        NewBullet.name = "Bullet";                                                                      // Benennung, Tag erzeugen
        NewBullet.transform.localScale = new Vector3(0.5f, 1f, 1f);                                     // Skalierung der Komponente
        NewBullet.transform.localPosition = _pos;                                                       // Instanzierung / Positionierung der Komponente
        Destroy(NewBullet.GetComponent<MeshCollider>());                                                // MeshCollider des GameObjects zerstören
        StartCoroutine(AddCollider(NewBullet));                                                         // Hinzufügen des BoxCollider mit einer Coroutine ans GO
        NewBullet.GetComponent<MeshRenderer>().enabled = false;                                         // Deaktivierung des MeshRenderer [bei SpriteRenderer gäbe es ein Fehler -> weiße Quadrate in Game Scene]
        NewBullet.tag = "Bullet";                                                                       // Tag zuweisen

        GameObject SpriteBullet = new GameObject("Sprite Bullet");                                      // Erzeugung eines neuen GOs mit dem Namen "Sprite Bullet"
        SpriteBullet.transform.SetParent(NewBullet.transform);                                          // Wer ist der Parent des GO (Clone?)
        SpriteBullet.transform.localScale = new Vector3(5f, 2.5f, 2.5f);                                // Skalierung der Komponente
        SpriteBullet.transform.localPosition = new Vector2(0, 0);                                       // Instanzierung / Positionierung der Komponente
        SpriteBullet.AddComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Bullet");          // Hinzufügen eines SpriteRenderers, Inhalte des Ordners (Resources, Sprite) mit dem Namen "Bullet" laden 


        // Bullet umdrehen
        if (_type == BulletType.ENEMY)
        {
            SpriteBullet.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        NewBullet.AddComponent<BulletControl>().InitBullet(_type, _speed, this);                        // this braucht man für die Funktion GetDamageEnemy
        Destroy(NewBullet, 3);
    }

    // Schaden - Enemy
    public void GetDamageEnemy(GameObject _obj)
    {
        for (int i = 0; i < SavedEnemies.Count; i++)
        {
            if (SavedEnemies[i].EnemyObj == _obj)
            {
                SavedEnemies[i].Enemy.Lives--;
                if (SavedEnemies[i].Enemy.Lives <= 0)
                {
                    CreateExplosion(SavedEnemies[i].EnemyObj.transform.position);
                    Destroy(SavedEnemies[i].EnemyObj);
                    SavedEnemies.RemoveAt(i);
                }
            }
        }

        if (SavedEnemies.Count == 0)
        {
            //
            CheckStars();
            ClearStage();
        }
    }

    // Schaden - Player
    public void GetDamagePlayer()
    {
        TotalLives--;
        if (TotalLives <= 0)
        {
            CreateExplosion(Player.transform.position);
            ClearStage();
        }
    }

    // Stars
    void CheckStars()
    {
        float result = (TotalSeconds / CurrentWorld.Seconds) * 3;                                         // wieviel Zeit bleibt mir noch und wie viele Sterne bekomme ich
        int TotalStars = (int)result + 1;

        if (PlayerPrefs.HasKey("World_" + CurrentWorld.ID) == false)
        {
            // Speicherung der Daten
            PlayerPrefs.SetInt("World_" + CurrentWorld.ID, TotalStars);
        }
        else
        {
            int TempStars = PlayerPrefs.GetInt("World_" + CurrentWorld.ID);
            if (TotalStars > TempStars)
            {
                PlayerPrefs.SetInt("World_" + CurrentWorld.ID, TotalStars);
            }
        }
    }


    // Explosion
    void CreateExplosion(Vector2 _pos)
    {
        GameObject NewExplosion = new GameObject("Explosion");
        NewExplosion.transform.position = _pos;
        NewExplosion.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        NewExplosion.AddComponent<SpriteRenderer>();
        NewExplosion.AddComponent<AnimationControl>().InitialAnimation(NewExplosion.GetComponent<SpriteRenderer>(), new List<Sprite>(Resources.LoadAll<Sprite>("Explosion")), 0.05f, AnimationType.ONCE);
        Destroy(NewExplosion, 2);
    }

    public void ResetValues()
    {
        PlayerPrefs.DeleteAll();
        PrintMenu();
        ResetScreen();
    }

    public void ResetScreen()
    {
        _resetScreen.gameObject.SetActive(!_resetScreen.gameObject.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
