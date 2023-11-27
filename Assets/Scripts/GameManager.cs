using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Gameplay Settings")]
    [SerializeField]
    private float _levelDuration;
    [SerializeField]
    private float _maxProgress;
    [SerializeField]
    private EnemyFactory[] _enemies;

    [Header("Gameplay Properties")]
    [SerializeField]
    private int _currentLevel;
    [SerializeField]
    private float _currentDuration;
    [SerializeField]
    private float _currentProgress;
    [SerializeField]
    private int _maxEnemies;
    [SerializeField]
    private float _totalDamage;
    [SerializeField]
    private PerkFactory _perkFactory;

    [Header("Menu HUD")]
    [SerializeField]
    private CanvasGroup _menuHud;
    [SerializeField]
    private Button _playButton;

    [Header("Gameplay HUD")]
    [SerializeField]
    private CanvasGroup _gameplayHud;
    [SerializeField]
    private Image _hpIcon;
    [SerializeField]
    private Sprite _hpSprite;
    [SerializeField]
    private Transform _hpRoot;
    [SerializeField]
    private TMP_Text _timerText;
    [SerializeField]
    private TMP_Text _levelText;
    [SerializeField]
    private TMP_Text _levelTextPopup;
    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private CanvasGroup _damageEffect;

    [Header("Passive Perk HUD")]
    [SerializeField]
    private TMP_Text _criticalRateText;
    [SerializeField]
    private TMP_Text _flameRateText;
    [SerializeField]
    private TMP_Text _iceRateText;
    [SerializeField]
    private TMP_Text _bombRateText;

    [Header("Perk HUD")]
    [SerializeField]
    private CanvasGroup _perkHud;
    [SerializeField]
    private Button _perk1;
    [SerializeField]
    private Button _perk2;
    [SerializeField]
    private Button _perk3;

    [Header("Result HUD")]
    [SerializeField]
    private CanvasGroup _resultHud;
    [SerializeField]
    private TMP_Text _lastLevelText;
    [SerializeField]
    private TMP_Text _totalDamageText;
    [SerializeField]
    private Button _replayButton;

    public bool isPlay;

    private Player player;

    [Header("Spawn Setting")]
    [SerializeField]
    private double currentWeights = 200f;
    private double accumulatedWeights;
    private System.Random rand = new System.Random();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        _menuHud.alpha = 1f;
        _menuHud.interactable = true;
        _menuHud.blocksRaycasts = true;

        _gameplayHud.alpha = 0f;
        _gameplayHud.interactable = false;
        _gameplayHud.blocksRaycasts = false;

        _perkHud.alpha = 0f;
        _perkHud.interactable = false;
        _perkHud.blocksRaycasts = false;

        _resultHud.alpha = 0f;
        _resultHud.interactable = false;
        _resultHud.blocksRaycasts = false;

        _playButton.onClick.AddListener(Play);
        _replayButton.onClick.AddListener(Replay);

        CalculateWeights();
        Transition.instance.FadeOut();
    }

    void Update()
    {
        ProcessLevel();
    }

    private void Play()
    {
        _menuHud.interactable = false;
        _menuHud.LeanAlpha(0, 0.3f).setOnComplete(() =>
        {
            _menuHud.gameObject.SetActive(false);
        });

        _gameplayHud.interactable = true;
        _gameplayHud.blocksRaycasts = true;
        _gameplayHud.LeanAlpha(1, 0.3f);

        StartCoroutine(InitLevel());
    }

    private IEnumerator InitLevel()
    {
        _currentDuration = _levelDuration;
        _currentProgress = 0f;

        UpdateTimerHud();
        UpdateProgressBar();

        _levelText.text = _currentLevel.ToString("00");
        _levelTextPopup.text = "Wave " + _currentLevel.ToString();
        _levelTextPopup.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        _levelTextPopup.gameObject.SetActive(false);
        
        isPlay = true;
        SpawnCheck();
    }

    private void ProcessLevel()
    {
        if (!isPlay) return;

        _currentDuration -= Time.deltaTime;
        UpdateTimerHud();

        if (_currentProgress >= _maxProgress)
        {
            EndLevel();
        }

        if (_currentDuration <= 0 && _currentProgress < _maxProgress)
        {
            Gameover();
        }
    }

    private void EndLevel()
    {
        isPlay = false;

        StartCoroutine(RandomPerk());

        _gameplayHud.interactable = false;
        _gameplayHud.LeanAlpha(0, 0.3f);

        _perkHud.interactable = true;
        _perkHud.blocksRaycasts = true;
        _perkHud.LeanAlpha(1, 0.3f);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        _currentDuration = 0f;
        UpdateTimerHud();

        _currentLevel++;
        currentWeights += 10f;
        if (currentWeights >= accumulatedWeights)
        {
            currentWeights = accumulatedWeights;
        }
    }

    private IEnumerator RandomPerk()
    {
        List<Perk> perks = new List<Perk>();
        for (int i = 0; i < 3; i++)
        {
            Perk perk = null;

            do
            {
                perk = _perkFactory.perks[UnityEngine.Random.Range(0, _perkFactory.perks.Length)];
                yield return null;
            } while (perks.Contains(perk) || perk == null);

            perks.Add(perk);
        }

        _perk1.onClick.RemoveAllListeners();
        _perk1.onClick.AddListener(() => SelectPerk(perks[0]));
        _perk1.transform.GetChild(0).GetComponent<Image>().sprite = perks[0].icon;

        _perk2.onClick.RemoveAllListeners();
        _perk2.onClick.AddListener(() => SelectPerk(perks[1]));
        _perk2.transform.GetChild(0).GetComponent<Image>().sprite = perks[1].icon;

        _perk3.onClick.RemoveAllListeners();
        _perk3.onClick.AddListener(() => SelectPerk(perks[2]));
        _perk3.transform.GetChild(0).GetComponent<Image>().sprite = perks[2].icon;
    }

    private void SelectPerk(Perk perk)
    {
        perk.Execute(player);

        _perkHud.interactable = false;
        _perkHud.LeanAlpha(0, 0.3f);

        _gameplayHud.interactable = true;
        _gameplayHud.blocksRaycasts = true;
        _gameplayHud.LeanAlpha(1, 0.3f).setOnComplete(() =>
        {
            StartCoroutine(InitLevel());
        });   
    }

    public void Gameover()
    {
        isPlay = false;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        _gameplayHud.interactable = false;
        _gameplayHud.LeanAlpha(0, 0.3f);

        _resultHud.interactable = true;
        _resultHud.blocksRaycasts = true;
        _resultHud.LeanAlpha(1, 0.3f);

        _lastLevelText.text = "Wave " + _currentLevel.ToString();
        _totalDamageText.text = "Total damage : " + _totalDamage.ToString("0");
    }

    private void Replay()
    {
        Transition.instance.FadeIn("Game");
    }

    #region EVENT HANDLER
    public void AddProgress(float progress)
    {
        _currentProgress += progress;
        UpdateProgressBar();
    }

    public void AddTotalDamage(float damage)
    {
        _totalDamage += damage;
    }

    public void SpawnCheck()
    {
        if (!isPlay) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length > 0)
        {
            int spawnCount = _maxEnemies - enemies.Length;
            for (int i = 0; i < spawnCount; i++)
            {
                Spawn();
            }
        }
        else
        {
            for (int i = 0; i < _maxEnemies; i++)
            {
                Spawn();
            }
        }
    }

    private void Spawn()
    {
        //Random Spawn Position
        Vector3 spawnPos = Vector3.zero;
        spawnPos.x = UnityEngine.Random.Range(-4f, 4f);
        spawnPos.y = UnityEngine.Random.Range(-2.5f, 2.5f);

        //Random Enemy
        GameObject enemy = _enemies[GetRandomEnemy()].prefab;

        Instantiate(enemy, spawnPos, Quaternion.identity);
    }

    private int GetRandomEnemy()
    {
        double r = rand.NextDouble() * currentWeights;
        for (int i = 0; i < _enemies.Length; i++)
        {
            if (_enemies[i]._weight >= r)
                return i;
        }

        return 0;
    }

    private void CalculateWeights()
    {
        accumulatedWeights = 0f;
        foreach (var enemy in _enemies)
        {
            accumulatedWeights += enemy.spawnRate;
            enemy._weight = accumulatedWeights;
        }
    }
    #endregion

    #region HUD
    private void UpdateTimerHud()
    {
        var minutes = (int)(_currentDuration / 60);
        var seconds = (int)(_currentDuration - minutes * 60);

        _timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00"); 
    }

    private void UpdateProgressBar()
    {
        _progressBar.fillAmount = _currentProgress / _maxProgress;
    }

    public void UpdateHPIcon(int maxHp, int hp)
    {
        foreach (Transform item in _hpRoot)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < maxHp; i++)
        {
            Image icon = Instantiate(_hpIcon.gameObject, _hpRoot).GetComponent<Image>();
            if (i < hp)
            {
                icon.sprite = _hpSprite;
            }

            icon.gameObject.SetActive(true);
        }
    }

    public void UpdatePassivePerk(float criticalRate, float flameRate, float iceRate, float bombRate)
    {
        _criticalRateText.text = Math.Round(criticalRate * 100, 2).ToString() + "%";
        _flameRateText.text = Math.Round(flameRate * 100, 2).ToString() + "%";
        _iceRateText.text = Math.Round(iceRate * 100, 2).ToString() + "%";
        _bombRateText.text = Math.Round(bombRate * 100, 2).ToString() + "%";
    }

    public void ShowDamageEffect()
    {
        _damageEffect.LeanAlpha(1f, 0.1f).setOnComplete(() =>
        {
            _damageEffect.LeanAlpha(0f, 0.5f);
        });
    }
    #endregion
}

[Serializable]
public class EnemyFactory
{
    public string name;
    public GameObject prefab;
    [Range(0f, 100f)]
    public float spawnRate;
    public double _weight;
}