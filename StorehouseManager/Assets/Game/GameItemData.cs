using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Random = System.Random;

namespace Assets.Game
{
    public static class ItemDataExtensions
    {
        public static Color ToColor(this ItemRarity rarity)
        {
            switch (rarity)
            {
                case ItemRarity.Common:
                    return new Color(000 / 255f, 255 / 255f, 000 / 255f);
                case ItemRarity.Rare:
                    return new Color(000 / 255f, 191 / 255f, 255 / 255f);
                case ItemRarity.Epic:
                    return new Color(148 / 255f, 000 / 255f, 211 / 255f);
                case ItemRarity.Legendary:
                    return new Color(255 / 255f, 165 / 255f, 000 / 255f);
                case ItemRarity.Artifact:
                    return new Color(255 / 255f, 192 / 255f, 203 / 255f);
                    //return new Color(255 / 255f, 228 / 255f, 196 / 255f);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rarity), rarity, null);
            }
        }

        public static bool IsStoreableItem(this ItemType type)
        {
            return type == ItemType.Armor || type == ItemType.Shield ||
                   type == ItemType.Weapon || type == ItemType.MagicalWeapon ||
                   type == ItemType.Jewel || type == ItemType.Beverage;
        }

        public static bool IsArtisanIngredient(this ItemType type)
        {
            return type == ItemType.Part || type == ItemType.Ingredient;
        }

        public static bool IsGear(this ItemType type)
        {
            return type == ItemType.Armor || type == ItemType.Shield ||
                   type == ItemType.Weapon || type == ItemType.MagicalWeapon ||
                   type == ItemType.Jewel;
        }
    }

    public class GameItemGenerator
    {
        private readonly GameItemData data;

        private readonly Dictionary<ItemRarity, int> RarityLastDropCount = new Dictionary<ItemRarity, int>();
        private readonly Dictionary<ItemRarity, RarityValue> RarityValues = new Dictionary<ItemRarity, RarityValue>();
        private IReadOnlyList<ItemRarity> RarityOrder;
        private readonly Random RarityRandom = new Random();
        private float MaxRarityRandomValue = 0;
        private readonly List<KeyValuePair<ItemRarity, float>> RarityMaxRanges = new List<KeyValuePair<ItemRarity, float>>();

        private readonly Random TypeRandom = new Random();
        private float MaxTypeRandomValue = 0;
        private readonly List<KeyValuePair<ItemType, float>> TypeMaxRanges = new List<KeyValuePair<ItemType, float>>();

        private readonly Random ItemRandom = new Random();

        public GameItemGenerator(GameItemData data)
        {
            this.data = data;
            RarityOrder = Enum.GetValues(typeof(ItemRarity)).Cast<ItemRarity>().OrderByDescending(r => r).ToList();
            foreach (ItemRarity rarity in RarityOrder)
            {
                RarityLastDropCount[rarity] = 0;
                RarityValue rarityData = data.Rarities.First(r => r.Rarity == rarity);
                RarityValues[rarity] = rarityData;
                MaxRarityRandomValue += rarityData.ChanceBase;
                RarityMaxRanges.Add(new KeyValuePair<ItemRarity, float>(rarity, MaxRarityRandomValue));
            }

            foreach (var typeData in data.Types.OrderByDescending(r => r.ChanceBase))
            {
                MaxTypeRandomValue += typeData.ChanceBase;
                TypeMaxRanges.Add(new KeyValuePair<ItemType, float>(typeData.Type, MaxTypeRandomValue));
            }

        }

        /// <summary>
        /// Returns copy of item definition.
        /// </summary>
        public ItemDefinition GenerateAny(ItemRarity? rarity = null, ItemType? type = null)
        {
            var items = new List<ItemDefinition>();

            rarity ??= GenerateRarity();
            type ??= GenerateType();
            items = data.TypeToRarityItemCollection[type.Value][rarity.Value];

            return items[ItemRandom.Next(items.Count)].CreateCopy();
        }

        private ItemRarity GenerateRarity()
        {
            // Increment all.
            foreach (ItemRarity rarity in RarityOrder)
            {
                RarityLastDropCount[rarity]++;
            }

            // Select 'random' rarity.
            ItemRarity choosenRarity = Generate(RarityRandom, MaxRarityRandomValue, RarityMaxRanges, RarityOrder.Last());
            
            // Find highest that is desired to drop by meantime waiter.
            foreach (ItemRarity rarity in RarityOrder)
            {
                if (rarity == choosenRarity)
                    break;
                if (RarityValues[rarity].MaxTimeToDrop <= RarityLastDropCount[rarity])
                {
                    choosenRarity = rarity;
                    break;
                }
            }
            RarityLastDropCount[choosenRarity] = 0;
            return choosenRarity;
        }

        private ItemType GenerateType()
        {
            return Generate(TypeRandom, MaxTypeRandomValue, TypeMaxRanges);
        }

        private T Generate<T>(Random random, float max, List<KeyValuePair<T, float>> ranges, T dv = default(T))
        {
            T choosenType = dv;
            var next = random.NextDouble() * max;

            foreach (KeyValuePair<T, float> typeRange in ranges)
            {
                if (next <= typeRange.Value)
                {
                    choosenType = typeRange.Key;
                    break;
                }
            }

            return choosenType;
        }
    }

    [Serializable]
    public enum ItemFlags
    {
        Stackable,
        Levelable,
        Sellable,
        Buyable,
    }

    [Serializable]
    public struct RarityValue
    {
        public ItemRarity Rarity;
        /// <summary>
        /// Experience when used for leveling.
        /// </summary>
        public float UpgradeExperienceBase;
        public uint MinLevelBase;
        public uint MaxLevelBase;
        public float PriceBase;
        public int MaxTimeToDrop;
        /// <summary>
        /// Value in range 0.0001 - 1.0%.
        /// </summary>
        public float ChanceBase;
        /// <summary>
        /// Base reputation given for this rarity
        /// </summary>
        public float ReputationBase;
    }

    [Serializable]
    public struct TypeValue
    {
        public ItemType Type;
        /// <summary>
        /// Experience percentage bonus when used for leveling.
        /// </summary>
        public float UpgradeExperiencePercentage;
        public uint MinStackBase;
        public uint MaxStackBase;
        /// <summary>
        /// Value in range 0.0 - *.0%.
        /// </summary>
        public float FinalSellPricePercentage;
        /// <summary>
        /// Value in range 0.0 - *.0%.
        /// </summary>
        public float FinalBuyPricePercentage;
        /// <summary>
        /// Value in range 0.0..1 - 1.0%.
        /// </summary>
        public float ChanceBase;
        public ItemFlags[] Flags;
        /// <summary>
        /// Base reputation given for this type
        /// </summary>
        public float RarityReputationPercentage;
    }

    [Serializable]
    public struct ArtisanEffectivity
    {
        public ItemType TargetType;
        /// <summary>
        /// Value in range 0.0 - *.0%.
        /// </summary>
        public float ExperienceBonus;
    }

    [Serializable]
    public struct TagValue
    {
        public string ItemTag;
        public ArtisanEffectivity[] ArtisanUseEffectivities;
    }

    [Serializable]
    public class GameItemData : MonoBehaviour
    {
        public GameObject ItemFramePrefab;
        public ItemCategory[] Categories;
        public RarityValue[] Rarities =
        {
            new RarityValue()
            {
                Rarity = ItemRarity.Common,
                UpgradeExperienceBase = 25.00f,
                MinLevelBase = 0,
                MaxLevelBase = 2,
                PriceBase = 33.00f,
                MaxTimeToDrop = 0,
                ChanceBase = 1.00f,
                ReputationBase = 50.00f,
            },
            new RarityValue()
            {
                Rarity = ItemRarity.Rare,
                UpgradeExperienceBase = 50.00f,
                MinLevelBase = 0,
                MaxLevelBase = 4,
                PriceBase = 75.00f,
                MaxTimeToDrop = 5,
                ChanceBase = 0.33f,
                ReputationBase = 100.00f,
            },
            new RarityValue()
            {
                Rarity = ItemRarity.Epic,
                UpgradeExperienceBase = 100.00f,
                MinLevelBase = 0,
                MaxLevelBase = 6,
                PriceBase = 150.00f,
                MaxTimeToDrop = 10,
                ChanceBase = 0.10f,
                ReputationBase = 200.00f,
            },
            new RarityValue()
                {
                Rarity = ItemRarity.Legendary,
                UpgradeExperienceBase = 250.00f,
                MinLevelBase = 2,
                MaxLevelBase = 8,
                PriceBase = 250.00f,
                MaxTimeToDrop = 25,
                ChanceBase = 0.04f,
                ReputationBase = 400.00f,
            },
            new RarityValue()
                {
                Rarity = ItemRarity.Artifact,
                UpgradeExperienceBase = 1000.00f,
                MinLevelBase = 4,
                MaxLevelBase = 10,
                PriceBase = 1000.00f,
                MaxTimeToDrop = 100,
                ChanceBase = 0.01f,
                ReputationBase = 2000.00f,
            }
        };
        public TypeValue[] Types =
        {
            new TypeValue()
            {
                Type = ItemType.Armor,
                MinStackBase = 1,
                MaxStackBase = 1,
                FinalSellPricePercentage = 0.60f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.20f,
                RarityReputationPercentage = 0.70f,
                UpgradeExperiencePercentage = 1.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Levelable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Shield,
                MinStackBase = 1,
                MaxStackBase = 1,
                FinalSellPricePercentage = 0.60f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.20f,
                RarityReputationPercentage = 0.50f,
                UpgradeExperiencePercentage = 1.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Levelable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Weapon,
                MinStackBase = 1,
                MaxStackBase = 1,
                FinalSellPricePercentage = 0.40f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.20f,
                RarityReputationPercentage = 0.60f,
                UpgradeExperiencePercentage = 1.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Levelable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.MagicalWeapon,
                MinStackBase = 1,
                MaxStackBase = 1,
                FinalSellPricePercentage = 1.00f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.10f,
                RarityReputationPercentage = 1.00f,
                UpgradeExperiencePercentage = 1.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Levelable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Jewel,
                MinStackBase = 1,
                MaxStackBase = 1,
                FinalSellPricePercentage = 0.80f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.10f,
                RarityReputationPercentage = 0.80f,
                UpgradeExperiencePercentage = 1.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Levelable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Beverage,
                MinStackBase = 1,
                MaxStackBase = 3,
                FinalSellPricePercentage = 0.25f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.25f,
                RarityReputationPercentage = 0.25f,
                UpgradeExperiencePercentage = 1.5f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Stackable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Part,
                MinStackBase = 1,
                MaxStackBase = 5,
                FinalSellPricePercentage = 1.50f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.33f,
                RarityReputationPercentage = 0.00f,
                UpgradeExperiencePercentage = 2.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Stackable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Ingredient,
                MinStackBase = 1,
                MaxStackBase = 10,
                FinalSellPricePercentage = 0.75f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 0.50f,
                RarityReputationPercentage = 0.00f,
                UpgradeExperiencePercentage = 1.5f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Stackable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Junk,
                MinStackBase = 1,
                MaxStackBase = 20,
                FinalSellPricePercentage = 0.10f,
                FinalBuyPricePercentage = 0.0f,
                ChanceBase = 1.00f,
                RarityReputationPercentage = 0.00f,
                UpgradeExperiencePercentage = 0.5f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Stackable,
                    ItemFlags.Sellable,
                }
            },
            new TypeValue()
            {
                Type = ItemType.Container,
                MinStackBase = 1,
                MaxStackBase = 1,
                FinalSellPricePercentage = 0.0f,
                FinalBuyPricePercentage = 5.00f,
                ChanceBase = 0.00f,
                RarityReputationPercentage = 0.00f,
                UpgradeExperiencePercentage = 0.0f,
                Flags = new ItemFlags[]
                {
                    ItemFlags.Buyable,
                },
            },
        };
        public TagValue[] Tags =
        {
            new TagValue()
            {
                ItemTag = "Offense",
                ArtisanUseEffectivities = new ArtisanEffectivity[]
                {
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Armor,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Shield,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Weapon,
                        ExperienceBonus = 1.00f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.MagicalWeapon,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Jewel,
                        ExperienceBonus = 0.50f,
                    },
                }
            },
            new TagValue()
            {
                ItemTag = "Defense",
                ArtisanUseEffectivities = new ArtisanEffectivity[]
                {
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Armor,
                        ExperienceBonus = 1.00f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Shield,
                        ExperienceBonus = 1.00f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Weapon,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.MagicalWeapon,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Jewel,
                        ExperienceBonus = 0.50f,
                    },
                }
            },
            new TagValue()
            {
                ItemTag = "Magical",
                ArtisanUseEffectivities = new ArtisanEffectivity[]
                {
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Armor,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Shield,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Weapon,
                        ExperienceBonus = 0.25f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.MagicalWeapon,
                        ExperienceBonus = 1.00f,
                    },
                    new ArtisanEffectivity()
                    {
                        TargetType = ItemType.Jewel,
                        ExperienceBonus = 0.75f,
                    },
                }
            },
        };

        public GameItemGenerator Generator;
        /// <summary>
        /// Items default values for type -> rarity -> stack
        /// </summary>
        public readonly Dictionary<ItemType, Dictionary<ItemRarity, ItemStack>> TypeToRarityStackSizes 
            = new Dictionary<ItemType, Dictionary<ItemRarity, ItemStack>>(); 
        /// <summary>
        /// Items default values for type -> rarity -> level
        /// </summary>
        public readonly Dictionary<ItemType, Dictionary<ItemRarity, ItemLevel>> TypeToRarityItemLevels 
            = new Dictionary<ItemType, Dictionary<ItemRarity, ItemLevel>>();
        /// <summary>
        /// Items default values for type -> rarity -> data
        /// </summary>
        public readonly Dictionary<ItemType, Dictionary<ItemRarity, ItemData>> TypeToRarityItemDatas
            = new Dictionary<ItemType, Dictionary<ItemRarity, ItemData>>();
        /// <summary>
        /// items grouped by type and then by rarity
        /// </summary>
        public readonly Dictionary<ItemType, Dictionary<ItemRarity, List<ItemDefinition>>> TypeToRarityItemCollection
            = new Dictionary<ItemType, Dictionary<ItemRarity, List<ItemDefinition>>>();
        /// <summary>
        /// Items grouped by tag
        /// </summary>
        public readonly Dictionary<string, List<ItemDefinition>> TagToItemCollection
            = new Dictionary<string, List<ItemDefinition>>();

        private void InitializeTypeDictionaries()
        {
            foreach (TypeValue type in Types)
            {
                TypeToRarityStackSizes[type.Type] = new Dictionary<ItemRarity, ItemStack>();
                TypeToRarityItemLevels[type.Type] = new Dictionary<ItemRarity, ItemLevel>();
                TypeToRarityItemDatas[type.Type] = new Dictionary<ItemRarity, ItemData>();
                foreach (RarityValue rarity in Rarities)
                {
                    DefineStackSize(type, rarity);
                    DefineItemLevel(type, rarity);
                    DefineItemData(type, rarity);
                }
            }

            foreach (TypeValue type in Types)
            {
                TypeToRarityItemCollection[type.Type] = new Dictionary<ItemRarity, List<ItemDefinition>>();
                DefineItems(type.Type);
            }
        }
        
        private void DefineItems(ItemType type)
        {
            var allItems = new List<ItemDefinition>();
            foreach (var categoryList in Categories.Where(ct => ct.CategoryType == type).Select(ct => ct.Items))
            {
                allItems.AddRange(categoryList);
            }
            
            foreach (var rarityValue in Rarities)
            {
                DefineItemsPerRarity(type, rarityValue.Rarity, allItems);
            }
        }

        private void DefineItemsPerRarity(ItemType type, ItemRarity rarity, IReadOnlyList<ItemDefinition> allItems)
        {
            var rarityItems = new List<ItemDefinition>();
            foreach (ItemDefinition item in allItems.Where(itm => itm.Rarity == rarity))
            {
                UpdateItem(type, rarity, item);
                rarityItems.Add(item);
            }
            TypeToRarityItemCollection[type][rarity] = rarityItems;
        }

        private void UpdateItem(ItemType type, ItemRarity rarity, ItemDefinition item)
        {
            item.StackSize = new ItemStack(TypeToRarityStackSizes[type][rarity]);
            item.ItemLevel = new ItemLevel(TypeToRarityItemLevels[type][rarity]);
            item.ItemData = new ItemData(TypeToRarityItemDatas[type][rarity]);

            ExtendSetCollection(item);
        }

        private void ExtendSetCollection(ItemDefinition item)
        {
            foreach (string itemTag in item.SetTags)
            {
                if (TagToItemCollection.TryGetValue(itemTag, out var list))
                {
                    list.Add(item);
                }
                else
                {
                    TagToItemCollection[itemTag] = new List<ItemDefinition>() {item};
                }
            }
        }

        private void DefineItemLevel(TypeValue type, RarityValue rarity)
        {
            var minLevel = rarity.MinLevelBase;
            var maxLevel = rarity.MaxLevelBase;
            TypeToRarityItemLevels[type.Type][rarity.Rarity] = new ItemLevel(minLevel, maxLevel);
        }

        private void DefineStackSize(TypeValue type, RarityValue rarity)
        {
            var minStack = type.MinStackBase;
            var maxStack = type.MaxStackBase;
            TypeToRarityStackSizes[type.Type][rarity.Rarity] = new ItemStack(minStack, maxStack);
        }

        private void DefineItemData(TypeValue type, RarityValue rarity)
        {
            TypeToRarityItemDatas[type.Type][rarity.Rarity] = new ItemData(
                typeReputationPercentage: type.RarityReputationPercentage,
                rarityReputation: rarity.ReputationBase,
                flags: new HashSet<ItemFlags>(type.Flags),
                upgradeExp: rarity.UpgradeExperienceBase,
                upgradeExpPercentage: type.UpgradeExperiencePercentage,
                priceBase: rarity.PriceBase,
                finalSellPricePercentage: type.FinalSellPricePercentage,
                finalBuyPricePercentage: type.FinalBuyPricePercentage
            );
        }

        void Start()
        {
            foreach (ItemCategory itemCategory in Categories)
            {
                itemCategory.Populate();
            }

            InitializeTypeDictionaries();

            Generator = new GameItemGenerator(this);
        }

        public void CreateAnyFor(InventorySlot slot)
        {
            var newItem = Generator.GenerateAny();
            Assign(slot, newItem);
        }

        public void Assign(InventorySlot slot, ItemDefinition item)
        {
            var inventoryFrame = Instantiate(ItemFramePrefab).GetComponent<InventoryItem>();
            inventoryFrame.SetItem(item);
            slot.AdoptChild(inventoryFrame);
        }

        public void CreateSpecificFor(InventorySlot slot, ItemType type)
        {
            var newItem = Generator.GenerateAny(type: type);
            Assign(slot, newItem);
        }
        public void CreateSpecificFor(InventorySlot slot, ItemType? type, ItemRarity rarity)
        {
            var newItem = Generator.GenerateAny(rarity, type);
            Assign(slot, newItem);
        }
    }
}