using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class carCreation : MonoBehaviour, ISerializationCallbackReceiver
{
    [Header("----- Shelf Items -----")]
    [SerializeField] Item[] DisplayedShelfItems;
    [Range(1,100), SerializeField] int ChasisCapacity;
    [Range(1, 100), SerializeField] int EngineCapacity;
    [Range(1, 100), SerializeField] int TiresCapacity;
    [Range(1, 100), SerializeField] int PowerSteeringCapacity;
    [Range(1, 100), SerializeField] int ShockAbsorbersCapacity;
    [Range(1, 100), SerializeField] int BrakesCapacity;
    [Header(" ----- Starting Assets -----")]
    [SerializeField] int InitialDebtCredits;
    [SerializeField] int RemainingDebtCredits;
    [SerializeField] TextMeshProUGUI RemainingDebtText;
    [Header("----- Car Stats -----")]
    [SerializeField] TextMeshProUGUI ItemName;
    [SerializeField] GameObject ItemPreviewStats;

    [Header("----- Previewed Item -----")]
    [SerializeField] uint SelectedID;
    [SerializeField] Item SelectedPreview;

    [Header("----- Chasis -----")]
    [SerializeField] List<uint> ChasisShelf;
    [SerializeField] uint PurchasedChasisID;
    [SerializeField] Chasis PurchasedChasis;
    [SerializeField] TextMeshProUGUI EquippedChasisText;
    [Header("----- Engine -----")]
    [SerializeField] List<uint> EngineShelf;
    [SerializeField] uint PurchasedEngineID;
    [SerializeField] Engine PurchasedEngine;
    [SerializeField] TextMeshProUGUI EquippedEngineText;
    [Header("----- Tires -----")]
    [SerializeField] List<uint> TiresShelf;
    [SerializeField] uint PurchasedTiresID;
    [SerializeField] Tires PurchasedTires;
    [SerializeField] TextMeshProUGUI EquippedTiresText;
    [Header("----- Power Steering -----")]
    [SerializeField] List<uint> PowerSteeringShelf;
    [SerializeField] uint PurchasedPowerSteeringID;
    [SerializeField] PowerSteering PurchasedPowerSteering;
    [SerializeField] TextMeshProUGUI EquippedPowerSteeringText;
    [Header("----- Shock Absorbers -----")]
    [SerializeField] List<uint> ShockAbsorbersShelf;
    [SerializeField] uint PurchasedShockAbsorbersID;
    [SerializeField] ShockAbsorbers PurchasedShockAbsorbers;
    [SerializeField] TextMeshProUGUI EquippedShockAbsorbersText;
    [Header("----- Brakes -----")]
    [SerializeField] List<uint> BrakesShelf;
    [SerializeField] uint PurchasedBrakesID;
    [SerializeField] Brakes PurchasedBrakes;
    [SerializeField] TextMeshProUGUI EquippedBrakesText;

    [Header("----- Drag & Drop -----")]
    [SerializeField] public Button ItemthumbnailPrefab;
    [SerializeField] public GameObject TextLinePrefab;
    [SerializeField] public ScrollRect ShopInventoryObject;
    [SerializeField] GameObject ShopShelf;
    //[SerializeField] GameObject ItemData;

    public void OnBeforeSerialize()
    {
        //throw new System.NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        PurchasedChasis = gameManager.instance.DataManager().Chasies().GetItem(PurchasedChasisID) as Chasis;        
        PurchasedEngine = gameManager.instance.DataManager().Engines().GetItem(PurchasedEngineID) as Engine;       
        PurchasedTires = gameManager.instance.DataManager().Tires().GetItem(PurchasedTiresID) as Tires;
        PurchasedPowerSteering = gameManager.instance.DataManager().PowerSteering().GetItem(PurchasedPowerSteeringID) as PowerSteering;
        PurchasedShockAbsorbers = gameManager.instance.DataManager().ShockAbsorbers().GetItem(PurchasedShockAbsorbersID) as ShockAbsorbers;
        PurchasedBrakes = gameManager.instance.DataManager().Brakes().GetItem(PurchasedBrakesID) as Brakes;
    }


    private void OnEnable()
    {
        ResetCar();
    }

    private void OnDisable()
    {
        
    }

    public void ClearShopList()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform thumbail in ShopInventoryObject.content.transform)
        {
            children.Add(thumbail.gameObject);
        }
        foreach (GameObject child in children)
        {
            Destroy(child);
        }

    }

    private void ClearPreviewText()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform text in ItemPreviewStats.transform)
        {
            children.Add(text.gameObject);
        }
        foreach (GameObject child in children)
        {
            Destroy(child);
        }
    }

    public void InitializeRandomChasisShelf()
    {
        for (int i = 0; i < ChasisCapacity; i++)
        {
            int rng = Random.Range(1, gameManager.instance.DataManager().Chasies().GetItemCounts() - 1);
            ChasisShelf.Add((uint)rng);
        }
    }

    public void InitializeRandomEngineShelf()
    {
        for (int i = 0; i < EngineCapacity; i++)
        {
            int rng = Random.Range(1, gameManager.instance.DataManager().Engines().GetItemCounts() - 1);
            EngineShelf.Add((uint)rng);
        }
    }

    public void InitializeRandomTiresShelf()
    {
        for(int i = 0; i < TiresCapacity; i++)
        {
            int rng = Random.Range(1, gameManager.instance.DataManager().Tires().GetItemCounts() - 1);
            TiresShelf.Add((uint)rng);
        }
    }

    public void InitializeRandomPowerSteeringShelf()
    {
        for (int i = 0; i < PowerSteeringCapacity; i++)
        {
            int rng = Random.Range(1, gameManager.instance.DataManager().PowerSteering().GetItemCounts() - 1);
            PowerSteeringShelf.Add((uint)rng);
        }
    }

    public void InitializeRandomShockAbsorbersShelf()
    {
        for (int i = 0; i < ShockAbsorbersCapacity; i++)
        {
            int rng = Random.Range(1, gameManager.instance.DataManager().ShockAbsorbers().GetItemCounts() - 1);
            ShockAbsorbersShelf.Add((uint)rng);
        }
    }

    public void InitializeRandomBrakesShelf()
    {
        for (int i = 0; i < BrakesCapacity; i++)
        {
            int rng = Random.Range(1, gameManager.instance.DataManager().Brakes().GetItemCounts() - 1);
            BrakesShelf.Add((uint)rng);
        }
    }

    public void DisplayChasis()
    {
        ClearShopList();

        for (int i = 0; i < ChasisShelf.Count; i++)
        {
            uint id = ChasisShelf[i];
            Button button = Instantiate(ItemthumbnailPrefab, ShopInventoryObject.content);
            SetButtonThumbnail(ref button, gameManager.instance.DataManager().Chasies().GetItem(id));
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i * 128) + 66, -64);
            button.onClick.AddListener(() => { SetSelectedChasis(id); });
        }
    }

    public void DisplayEngines()
    {
        ClearShopList();
        for (int i = 0; i < EngineShelf.Count; i++)
        {
            uint id = EngineShelf[i];
            Button button = Instantiate(ItemthumbnailPrefab, ShopInventoryObject.content);
            SetButtonThumbnail(ref button, gameManager.instance.DataManager().Engines().GetItem(id));
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i * 128) + 66, -64);
            button.onClick.AddListener(() => { SetSelectedEngine(id); });
        }
    }

    public void DisplayTires()
    {
        ClearShopList();
        for (int i = 0; i < TiresShelf.Count; i++)
        {
            uint id = TiresShelf[i];
            Button button = Instantiate(ItemthumbnailPrefab, ShopInventoryObject.content);
            SetButtonThumbnail(ref button, gameManager.instance.DataManager().Tires().GetItem(id));
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i * 128) + 66, -64);
            button.onClick.AddListener(() => { SetSelectedTires(id); });
        }
    }

    public void DisplayPowerSteering()
    {
        ClearShopList();
        for (int i = 0; i < PowerSteeringShelf.Count; i++)
        {
            uint id = PowerSteeringShelf[i];
            Button button = Instantiate(ItemthumbnailPrefab, ShopInventoryObject.content);
            SetButtonThumbnail(ref button, gameManager.instance.DataManager().PowerSteering().GetItem(id));
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i * 128) + 66, -64);
            button.onClick.AddListener(() => { SetSelectedPowerSteering(id); });
        }
    }

    public void DisplayShockAbsorbers()
    {
        ClearShopList();
        for (int i = 0; i < ShockAbsorbersShelf.Count; i++)
        {
            uint id = ShockAbsorbersShelf[i];
            Button button = Instantiate(ItemthumbnailPrefab, ShopInventoryObject.content);
            SetButtonThumbnail(ref button, gameManager.instance.DataManager().ShockAbsorbers().GetItem(id));
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i * 128) + 66, -64);
            button.onClick.AddListener(() => { SetSelectedShockAbsorbers(id); });
        }
    }

    public void DisplayBrakes()
    {
        ClearShopList();
        for (int i = 0; i < BrakesShelf.Count; i++)
        {
            uint id = BrakesShelf[i];
            Button button = Instantiate(ItemthumbnailPrefab, ShopInventoryObject.content);
            SetButtonThumbnail(ref button, gameManager.instance.DataManager().Brakes().GetItem(id));
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2((i * 128) + 66, -64);
            button.onClick.AddListener(() => { SetSelectedBrakes(id); });
        }
    }

    public void SetButtonThumbnail(ref Button button, Item item)
    {
        button.GetComponent<Image>().sprite = item.Background;
        button.transform.GetChild(0).GetComponent<Image>().sprite = item.Icon;
        button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.Cost.ToString();
    }

    private void SetSelectedChasis(uint chasisID)
    {
        Chasis SelectedChasis = gameManager.instance.DataManager().Chasies().GetItem(chasisID) as Chasis;
        //SelectedChasisID = chasisID;
        SelectedPreview = SelectedChasis;
        SelectedID = chasisID;
        ItemName.text = SelectedChasis.Name;
        ClearPreviewText();
        CreateText($"Chasis Cost: {SelectedChasis.Cost}", 0);
        CreateText($"Chasis Mass: {SelectedChasis.Mass}", 1);
        CreateText($"Chasis Durability: {SelectedChasis.Durability}", 2);
        CreateText($"Chasis Drag Coefficient: {SelectedChasis.DragCoefficient}", 3);
    }

    private void SetSelectedEngine(uint engineID)
    {
        Engine SelectedEngine = gameManager.instance.DataManager().Engines().GetItem(engineID) as Engine;
        //SelectedEngineID = engineID;
        SelectedPreview = SelectedEngine;
        SelectedID = engineID;

        ItemName.text = SelectedEngine.Name;
        ClearPreviewText();
        CreateText($"Engine Cost: {SelectedEngine.Cost}", 0);
        CreateText($"Motor Mass: {SelectedEngine.motorMass}", 1);
        CreateText($"Motor Power: {SelectedEngine.motorPower}", 2);
    }

    private void SetSelectedTires(uint tiresID)
    {
        Tires SelectedTires = gameManager.instance.DataManager().Tires().GetItem(tiresID) as Tires;
        //SelectedTiresID = tiresID;
        SelectedPreview = SelectedTires;
        SelectedID = tiresID;

        ItemName.text = SelectedTires.Name;
        ClearPreviewText();
        CreateText($"Tires Cost: {SelectedTires.Cost}", 0);
        //CreateText($"Tires Mass: {SelectedTires.Mass}", 1);
        //CreateText($"Tires Durability: {SelectedTires.Durability}", 2);
        //CreateText($"Tires Friction Coefficient: {SelectedTires.FrictionCoefficient}", 3);
    }

    private void SetSelectedPowerSteering(uint powerSteeringID)
    {
        PowerSteering SelectedPowerSteering = gameManager.instance.DataManager().PowerSteering().GetItem(powerSteeringID) as PowerSteering;
        //SelectedPowerSteeringID = powerSteeringID;
        SelectedPreview = SelectedPowerSteering;
        SelectedID = powerSteeringID;

        ItemName.text = SelectedPowerSteering.Name;
        ClearPreviewText();
        CreateText($"Power Steering Cost: {SelectedPowerSteering.Cost}", 0);
        //CreateText($"Power Steering Mass: {SelectedPowerSteering.Mass}", 1);
        //CreateText($"Power Steering Durability: {SelectedPowerSteering.Durability}", 2);
        //CreateText($"Power Steering Friction Coefficient: {SelectedPowerSteering.FrictionCoefficient}", 3);
    }

    private void SetSelectedShockAbsorbers(uint shockAbsorberID)
    {
        ShockAbsorbers SelectedShockAbsorbers = gameManager.instance.DataManager().ShockAbsorbers().GetItem(SelectedID) as ShockAbsorbers;
        SelectedID = shockAbsorberID;
        SelectedPreview = SelectedShockAbsorbers;
        ClearPreviewText();
        CreateText($"Shock Absorbers Cost: {SelectedShockAbsorbers.Cost}", 0);
    }

    private void SetSelectedBrakes(uint brakesID)
    {
        Brakes SelectedBrakes = gameManager.instance.DataManager().Brakes().GetItem(SelectedID) as Brakes;
        SelectedID = brakesID;
        SelectedPreview = SelectedBrakes;
        ClearPreviewText();
        CreateText($"Brakes Cost: {SelectedBrakes.Cost}", 0);
    }

    private void CreateText(string text, int index)
    {

        GameObject obj = Instantiate(TextLinePrefab, ItemPreviewStats.transform);
        obj.transform.SetAsLastSibling();
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, -rt.sizeDelta.y * index);

        TextMeshProUGUI newText = obj.GetComponent<TextMeshProUGUI>();
        newText.text = text;
    }

    public void PurchaseSelectedPreview()
    {
        if (SelectedPreview.Cost <= RemainingDebtCredits)
        {
            RemainingDebtCredits -= SelectedPreview.Cost;
            RemainingDebtText.text = "$" + RemainingDebtCredits.ToString();
            switch (SelectedPreview.Type)
            {
                case Item.ItemType.Chasis:
                    PurchasedChasisID = SelectedID;
                    PurchasedChasis = SelectedPreview as Chasis;
                    EquippedChasisText.text = "Chasis: " + PurchasedChasis.Name;
                    break;
                case Item.ItemType.Engine:
                    PurchasedEngineID = SelectedID;
                    PurchasedEngine = SelectedPreview as Engine;
                    EquippedEngineText.text = "Engine: " + PurchasedEngine.Name;
                    break;
                case Item.ItemType.Tires:
                    PurchasedTiresID = SelectedID;
                    PurchasedTires = SelectedPreview as Tires;
                    EquippedTiresText.text = "Tires: " + PurchasedTires.Name;
                    break;
                case Item.ItemType.PowerSteering:
                    PurchasedPowerSteeringID = SelectedID;
                    PurchasedPowerSteering = SelectedPreview as PowerSteering;
                    EquippedPowerSteeringText.text = "Power Steering: " + PurchasedPowerSteering.Name;
                    break;
                case Item.ItemType.ShockAbsorbers:
                    PurchasedShockAbsorbersID = SelectedID;
                    PurchasedShockAbsorbers = SelectedPreview as ShockAbsorbers;
                    EquippedShockAbsorbersText.text = "Shock Absorbers: " + PurchasedShockAbsorbers.Name;
                    break;
                case Item.ItemType.Brakes:
                    PurchasedBrakesID = SelectedID;
                    PurchasedBrakes = SelectedPreview as Brakes;
                    EquippedBrakesText.text = "Brakes: " + PurchasedBrakes.Name;
                    break;
            }
        }
    }

    private void ResetCar()
    {
        EquippedChasisText.text = "Chasis: " + PurchasedChasis.Name;
        EquippedEngineText.text = "Engine: " + PurchasedEngine.Name;
        EquippedTiresText.text = "Tires: " + PurchasedTires.Name;
        EquippedPowerSteeringText.text = "Power Steering: " + PurchasedPowerSteering.Name;
        EquippedShockAbsorbersText.text = "Shock Absorbers: " + PurchasedShockAbsorbers.Name;
        EquippedBrakesText.text = "Brakes: " + PurchasedBrakes.Name;
        RemainingDebtCredits = InitialDebtCredits;
        RemainingDebtText.text = "$" + RemainingDebtCredits.ToString();
        Random.InitState(System.DateTime.Now.Millisecond);
        InitializeRandomEngineShelf();
        InitializeRandomChasisShelf();
        InitializeRandomTiresShelf();
        InitializeRandomPowerSteeringShelf();
        InitializeRandomShockAbsorbersShelf();
        InitializeRandomBrakesShelf();

    }

    public BaseCar CreateNewCar()
    {
        BaseCar car = new BaseCar();
        car.chasisID = PurchasedChasisID;
        car.engineID = PurchasedEngineID;
        car.FrontTiresID = PurchasedTiresID;
        car.RearTiresID = PurchasedTiresID;
        car.powerSteeringID = PurchasedPowerSteeringID;
        car.shockAbsorbersID = PurchasedShockAbsorbersID;
        car.brakesID = PurchasedBrakesID;

        car.chasis = PurchasedChasis;
        car.engine = PurchasedEngine;
        car.frontTires = PurchasedTires;
        car.rearTires = PurchasedTires;
        car.powerSteering = PurchasedPowerSteering;
        car.shockAbsorbers = PurchasedShockAbsorbers;
        car.brakes = PurchasedBrakes;

        return car;
    }
}
