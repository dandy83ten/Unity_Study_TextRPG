using TextRPG.Models;
using System;
using System.Text.Json;
using TextRPG.Data;

namespace TextRPG.Systems;

public class SaveLoadSystem
{
    // 저장 경로 및 파일명
    private const string SaveFilePath = "savegame.json";
    
    // JSON 직렬화 옵션
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping //한글지원
    };

    #region 저장기능

    public static bool SaveGame(Player player, InventorySystem inventory)
    {
        try
        {
            // 1. 게임 객체 (클래스) > DTO (Data Transfer Object) 변환
            var saveData = new GameSaveData
            {
                Player = ConvertToPlayerData(player),
                Inventory = ConvertToItemData(inventory)
            };
             
            // 2. DTO 객체 -> JSON 문자열로 변환
            string jsonString = JsonSerializer.Serialize(saveData, jsonOptions);
            
            // 3. JSON 문자열 -> 파일로 저장
            System.IO.File.WriteAllText(SaveFilePath, jsonString);

            Console.WriteLine("\n게임이 저장되었습니다.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    // Player -> PlayerData로 변호나
    public static PlayerData ConvertToPlayerData(Player player)
    {
        return new PlayerData
        {
            Name = player.Name,
            Job = player.Job.ToString(),
            
            Level = player.Level,
            CurrentHp = player.CurrentHP,
            MaxHp = player.MaxHP,
            CurrentMp = player.CurrentMP,
            MaxMp = player.MaxMP,
            AttackPower = player.AttackPower,
            Defense = player.Defense,
            Gold = player.Gold,
            EquipedWeaponName = player.EquipedWeapon?.Name,
            EquipedArmorName = player.EquipedArmor?.Name
                
        };
    }
    
    // Inventory -> ItemData로 변환
    private static List<ItemData> ConvertToItemData(InventorySystem inventory)
    {
        var itemDataList = new List<ItemData>();
        
        for (int i = 0; i < inventory.Count; ++i)
        {
            var item = inventory.GetItem(i);
            if (item == null) continue;

            var itemData = new ItemData
            {
                Name = item.Name
            };

            if (item is Equipment equipment)
            {
                itemData.ItemType = "Equipment";
                itemData.Slot = equipment.Slot.ToString();
            }
            else if (item is Consumable consumable)
            {
                itemData.ItemType = "Consumable";
            }

            itemDataList.Add(itemData);
        }

        return itemDataList;
    }

    #endregion

    #region 불러오기 기능
    // 저장 파일 여부 확인
    public static bool IsSaveFileExist()
    {
        return System.IO.File.Exists(SaveFilePath);
    }
    public static GameSaveData? LoadGame()
    {
        try
        {
            // 1. JSON 파일에서 문자열 읽기
            string jsonString = System.IO.File.ReadAllText(SaveFilePath);
            
            // 2. JSON 문자열 -> DTO 변환 (역직렬화)
            var saveData = JsonSerializer.Deserialize<GameSaveData>(jsonString, jsonOptions);
            
            Console.WriteLine("\n게임 데이터가 로드되었습니다.");
            return saveData;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
    
    // PlayerData DTO를 Player 클래스로 변환 메서드
    public static Player LoadPlayer(PlayerData data)
    {
        // JobType를 문자열 -> 열거형(Enum)
        var job = Enum.Parse<JobType>(data.Job);
        // Player 객체 생성
        var player = new Player(data.Name, job);
        
        // 스텟 설정
        player.Level = data.Level;
        player.CurrentHP = data.CurrentHp;
        player.MaxHP = data.MaxHp;
        
        player.CurrentMP = data.CurrentMp;
        player.MaxMP = data.MaxMp;
        player.AttackPower = data.AttackPower;
        player.Defense = data.Defense;
        
        player.Gold = data.Gold;
        return player;
    }
    
    // ItemData DTO를 Inventory 클래스로 변환 메서드
    public static InventorySystem LoadInventory(List<ItemData> itemDataList, Player player)
    {
        var inventory = new InventorySystem();

        foreach (var itemData in itemDataList)
        {
            Item? item = null;

            if (itemData.ItemType == "Equipment")
            {
                // 장착 슬롯 확인
                var slot = Enum.Parse<EquipmentSlot>(itemData.Slot);
                if (slot == EquipmentSlot.Weapon)
                {
                    item = Equipment.CreateWeapon(itemData.Name);
                }
                else if (slot == EquipmentSlot.Armor)
                {
                    item = Equipment.CreateArmor(itemData.Name);
                }
            }
            else if (itemData.ItemType == "Consumable")
            {
                item = Consumable.CreatePotion(itemData.Name);
            }
                
            if (item != null)
                inventory.AddItem(item);
        }
        
        return inventory;
    }
    
    // 저장된 장착 아이템을 복원 메서드(무기/방어구)
    public static void LoadEquippedItems(Player player, PlayerData data, InventorySystem inventory)
    {
        // 무기 장착 복원
        if (!string.IsNullOrEmpty(data.EquipedWeaponName))
        {
            // 인벤토리에서 같은 무기를 찾기
            for (int i = 0; inventory.Count > i; ++i)
            {
                var item = inventory.GetItem(i);
                if (item is Equipment equipment && equipment.Slot == EquipmentSlot.Weapon && equipment.Name == data.EquipedWeaponName)
                {
                    player.EquipItem(equipment);
                    break;
                }
            }
        }

        // 방어구 장착 복원
        if (!string.IsNullOrEmpty(data.EquipedArmorName))
        {
            for (int i = 0; inventory.Count > i; ++i)
            {
                var item = inventory.GetItem(i);
                if (item is Equipment equipment && equipment.Slot == EquipmentSlot.Armor && equipment.Name == data.EquipedArmorName)
                {
                    player.EquipItem(equipment);
                    break;
                }
            }
        }
    }
    
    
    #endregion
}