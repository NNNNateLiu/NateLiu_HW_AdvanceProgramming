using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PackManager : MonoBehaviour
{
    [SerializeField] 
    private JSONLoader jsonLoader;
    [SerializeField]
    private GameObject cardTemplate;
    [SerializeField]
    private List<Transform> cardSlots;
    [SerializeField] 
    private RareTireData rareTireData;

    private List<string> selectedWeapon;
    private int[] weaponRareTireArray;

    private void Awake()
    {
        weaponRareTireArray = new int[4];
        selectedWeapon = new List<string>(4);
        rareTireData.Initialize();
        jsonLoader.jsonRefreshed += LoadRareTireFromJSON;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            OpenThePack();
        }
    }

    private void LoadRareTireFromJSON()
    {
        jsonLoader.jsonRefreshed -= LoadRareTireFromJSON;
        Debug.Log("Weapon Number: " + jsonLoader.currentJSON.Count);
        for (int i = 0; i < jsonLoader.currentJSON.Count; i++)
        {
            //Debug.Log("Weapon Id:" + jsonLoader.currentJSON[i]["Id"] + " " +
                      //"Weapon Name:" + jsonLoader.currentJSON[i]["Name"] + " " +
                      //"Weapon Rare:" + jsonLoader.currentJSON[i]["Rare"]);
            
            switch (int.Parse(jsonLoader.currentJSON[i]["Rare"]))
            {
                case 4:
                    rareTireData.Tire4WeaponIDPool.Add(jsonLoader.currentJSON[i]["Id"]);
                    break;
                case 3:
                    rareTireData.Tire3WeaponIDPool.Add(jsonLoader.currentJSON[i]["Id"]);
                    break;
                case 2:
                    rareTireData.Tire2WeaponIDPool.Add(jsonLoader.currentJSON[i]["Id"]);
                    break;
                case 1:
                    rareTireData.Tire1WeaponIDPool.Add(jsonLoader.currentJSON[i]["Id"]);
                    break;
                default:
                    Debug.Log("unsorted weapon");
                    break;
            }
        }

        weaponRareTireArray[0] = rareTireData.Tire1Rate;
        weaponRareTireArray[1] = rareTireData.Tire2Rate;
        weaponRareTireArray[2] = rareTireData.Tire3Rate;
        weaponRareTireArray[3] = rareTireData.Tire4Rate;
    }
    
    public void OpenThePack()
    {
        ResetBeforeOpenPack();
        SetSelectedWeapon();
        int currentSlotIndex = 0;
        foreach (var slot in cardSlots)
        {
            //Debug.Log("");
            GenerateTheCard(selectedWeapon[currentSlotIndex],slot);
            currentSlotIndex++;
        }
    }

    private GameObject GenerateTheCard(string WeaponId, Transform parent)
    {
        GameObject thisCard = Instantiate(cardTemplate, parent);
        Card card = thisCard.GetComponent<Card>();
        int weaponJSONIndex = int.Parse(WeaponId);
        Debug.Log(weaponJSONIndex);

        card.txt_Damage.text = jsonLoader.currentJSON[weaponJSONIndex]["Damage"];
        card.txt_Price.text = jsonLoader.currentJSON[weaponJSONIndex]["Price"];
        card.txt_Descriptions.text = jsonLoader.currentJSON[weaponJSONIndex]["Descriotion"];
        card.txt_WeaponName.text = jsonLoader.currentJSON[weaponJSONIndex]["Name"];
        
        return thisCard;
    }

    private int GetWeaponRareTireIndex()
    {
        int rand = Random.Range(1, 101);
        int tmp = 0;

        for (int i = 0; i < weaponRareTireArray.Length; i++)
        {
            tmp += weaponRareTireArray[i];
            if (rand < tmp)
            {
                return i;
            }
        }
        return 0;
    }

    private void SetSelectedWeapon()
    {
        for (int i = 0; i < 3 ; i++)
        {
            string selectedWeaponID = "";
            int tireIndex = GetWeaponRareTireIndex();
            Debug.Log("rare: " + tireIndex);
            switch (tireIndex)
            {
                case 0:
                    selectedWeaponID = rareTireData.Tire1WeaponIDPool[Random.Range(0, rareTireData.Tire1WeaponIDPool.Count)];
                    break;
                case 1:
                    selectedWeaponID = rareTireData.Tire2WeaponIDPool[Random.Range(0, rareTireData.Tire2WeaponIDPool.Count)];
                    break;
                case 2:
                    selectedWeaponID = rareTireData.Tire3WeaponIDPool[Random.Range(0, rareTireData.Tire3WeaponIDPool.Count)];
                    break;
                case 3:
                    selectedWeaponID = rareTireData.Tire4WeaponIDPool[Random.Range(0, rareTireData.Tire4WeaponIDPool.Count)];
                    break;
                default:
                    break;
            }
            Debug.Log(selectedWeaponID);
            selectedWeapon.Add(selectedWeaponID);
        }
    }

    private void ResetBeforeOpenPack()
    {
        foreach (var slot in cardSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }
        }
        selectedWeapon.Clear();
    }
}
